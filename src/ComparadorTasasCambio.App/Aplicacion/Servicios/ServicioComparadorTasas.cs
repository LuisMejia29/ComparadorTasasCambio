using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparadorTasasCambio.App.Dominio.Interfaces;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;
using System.Diagnostics;

namespace ComparadorTasasCambio.App.Aplicacion.Servicios;

public class ServicioComparadorTasas : IComparadorTasas
{
    private readonly List<IProveedorTasaCambio> _proveedores;
    private readonly ServicioLogger _logger;

    public ServicioComparadorTasas(
        List<IProveedorTasaCambio> proveedores,
        ServicioLogger logger)
    {
        _proveedores = proveedores ?? throw new ArgumentNullException(nameof(proveedores));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (_proveedores.Count == 0)
        {
            throw new ArgumentException("debe haber por lo menos un proveedor disponible", nameof(proveedores));
        }
    }

    public async Task<ResultadoComparacion> CompararTasasAsync(SolicitudCambio solicitud)
    {
        _logger.Informacion($"iniciar comparacion: {solicitud.Monto} {solicitud.MonedaOrigen} -> {solicitud.MonedaDestino}");
        _logger.Informacion($"validadando {_proveedores.Count} todos los proveedores...");

        var stopwatch = Stopwatch.StartNew();

        //  task.whenall para validar todos los proveedroes
        var tareas = _proveedores.Select(p => p.ObtenerTasaCambioAsync(solicitud)).ToList();
        var ofertas = await Task.WhenAll(tareas);

        stopwatch.Stop();

        var listaOfertas = ofertas.ToList();

        _logger.Informacion($"comparacion lista {stopwatch.ElapsedMilliseconds}milsegundos");
        _logger.Informacion($"ofertas: {listaOfertas.Count(o => o.Exitosa)}/{listaOfertas.Count}");

        var resultado = new ResultadoComparacion(
            solicitud,
            listaOfertas,
            stopwatch.ElapsedMilliseconds);

        if (resultado.TieneOfertaValida)
        {
            _logger.Informacion($"mejor oferta: {resultado.MejorOferta!.NombreApi} - {resultado.MejorOferta.MontoConvertido} {solicitud.MonedaDestino}");
        }
        else
        {
            _logger.Advertencia("no se obtuvo una oferta validad de los proveedores");
        }

        return resultado;
    }
}