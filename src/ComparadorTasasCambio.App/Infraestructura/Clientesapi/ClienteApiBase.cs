using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparadorTasasCambio.App.Dominio.Interfaces;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;
using System.Diagnostics;

namespace ComparadorTasasCambio.App.Infraestructura.ClientesApi;

/// Clase base para todos los clientes de api.

public abstract class ClienteApiBase : IProveedorTasaCambio
{
    protected readonly HttpClient _httpClient;
    protected readonly ServicioLogger _logger;
    protected readonly string _urlApi;
    protected readonly string _apiKey;
    protected readonly int _timeout;

    protected ClienteApiBase(
        string urlApi,
        string apiKey,
        int timeout,
        ServicioLogger logger)
    {
        _urlApi = urlApi;
        _apiKey = apiKey;
        _timeout = timeout;
        _logger = logger;

        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(timeout)
        };
    }

    public async Task<OfertaCambio> ObtenerTasaCambioAsync(SolicitudCambio solicitud)
    {
        var stopwatch = Stopwatch.StartNew();
        var nombreApi = ObtenerNombreProveedor();

        try
        {
            _logger.Informacion($"consultando {nombreApi} para {solicitud.MonedaOrigen} -> {solicitud.MonedaDestino}");

            var respuesta = await ConsultarApiAsync(solicitud);

            stopwatch.Stop();

            _logger.Informacion($"{nombreApi} respondio exitosamente en {stopwatch.ElapsedMilliseconds}ms");

            return respuesta;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.Error(ex, $"error validando {nombreApi}");

            return new OfertaCambio(
                nombreApi,
                null,
                null,
                stopwatch.ElapsedMilliseconds,
                false,
                $"Error: {ex.Message}");
        }
    }

    protected abstract Task<OfertaCambio> ConsultarApiAsync(SolicitudCambio solicitud);

    public abstract string ObtenerNombreProveedor();

    public virtual async Task<bool> EstaDisponibleAsync()
    {
        return await Task.FromResult(true);
    }
}