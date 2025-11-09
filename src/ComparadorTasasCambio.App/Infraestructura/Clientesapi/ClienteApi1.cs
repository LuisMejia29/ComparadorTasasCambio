using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;
using System.Text.Json;

namespace ComparadorTasasCambio.App.Infraestructura.ClientesApi;


public class ClienteApi1 : ClienteApiBase
{
    public ClienteApi1(string urlApi, string apiKey, int timeout, ServicioLogger logger)
        : base(urlApi, apiKey, timeout, logger)
    {
    }

    public override string ObtenerNombreProveedor()
    {
        return "api1_json";
    }

    protected override async Task<OfertaCambio> ConsultarApiAsync(SolicitudCambio solicitud)
    {
        var requestBody = new
        {
            from = solicitud.MonedaOrigen,
            to = solicitud.MonedaDestino,
            value = solicitud.Monto
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await _httpClient.PostAsync(_urlApi, content);
        stopwatch.Stop();

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

        var rate = responseObj.GetProperty("rate").GetDecimal();

        // el rate es el monto convertido directamente
        var montoConvertido = rate;
        var tasaCambio = montoConvertido / solicitud.Monto;

        return new OfertaCambio(
            ObtenerNombreProveedor(),
            montoConvertido,
            tasaCambio,
            stopwatch.ElapsedMilliseconds,
            true);
    }
}