using System;
using System.Threading.Tasks;
using ComparadorTasasCambio.App.Dominio.Interfaces;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;

namespace ComparadorTasasCambio.App.Aplicacion.CasosDeUso;

public class ObtenerMejorTasaCambioUseCase
{
    private readonly IComparadorTasas _comparador;
    private readonly ServicioLogger _logger;

    public ObtenerMejorTasaCambioUseCase(
        IComparadorTasas comparador,
        ServicioLogger logger)
    {
        _comparador = comparador;
        _logger = logger;
    }

    public async Task<ResultadoComparacion> EjecutarAsync(
        string monedaOrigen,
        string monedaDestino,
        decimal monto)
    {
        _logger.Informacion("iniciar busqueda de la mejor tasa");

        try
        {
            var solicitud = new SolicitudCambio(monedaOrigen, monedaDestino, monto);
            var resultado = await _comparador.CompararTasasAsync(solicitud);

            LogResumen(resultado);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "error en caso de uso");
            throw;
        }
    }

    private void LogResumen(ResultadoComparacion resultado)
    {
        _logger.Informacion($"apis consultadas: {resultado.TotalApisConsultadas} (exitosas: {resultado.ApisExitosas}, fallidas: {resultado.ApisFallidas})");
        _logger.Informacion($"tiempo: {resultado.TiempoTotalMs:F2}ms");

        if (resultado.TieneOfertaValida)
        {
            var mejor = resultado.MejorOferta;
            _logger.Informacion($"mejor oferta {mejor.NombreApi} - {mejor.MontoConvertido} {resultado.Solicitud.MonedaDestino} (tasa: {mejor.TasaCambio:F6})");
        }
        else
        {
            _logger.Advertencia("no se encontro oferta valida");
        }
    }
}