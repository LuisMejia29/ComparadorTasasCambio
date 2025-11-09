using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;
using System.Xml.Linq;

namespace ComparadorTasasCambio.App.Infraestructura.ClientesApi;

public class ClienteApi2 : ClienteApiBase
{
    public ClienteApi2(string urlApi, string apiKey, int timeout, ServicioLogger logger)
        : base(urlApi, apiKey, timeout, logger)
    {
    }

    public override string ObtenerNombreProveedor()
    {
        return "api2_xml";
    }

    protected override async Task<OfertaCambio> ConsultarApiAsync(SolicitudCambio solicitud)
    {
        var xmlBody = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<xml>
    <from>{solicitud.MonedaOrigen}</from>
    <to>{solicitud.MonedaDestino}</to>
    <amount>{solicitud.Monto}</amount>
</xml>";

        var content = new StringContent(xmlBody, Encoding.UTF8, "application/xml");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await _httpClient.PostAsync(_urlApi, content);
        stopwatch.Stop();

        response.EnsureSuccessStatusCode();

        var responseXml = await response.Content.ReadAsStringAsync();
        var xdoc = XDocument.Parse(responseXml);

        var result = decimal.Parse(xdoc.Root?.Element("Result")?.Value ?? "0");

        var montoConvertido = result;
        var tasaCambio = montoConvertido / solicitud.Monto;

        return new OfertaCambio(
            ObtenerNombreProveedor(),
            montoConvertido,
            tasaCambio,
            stopwatch.ElapsedMilliseconds,
            true);
    }
}