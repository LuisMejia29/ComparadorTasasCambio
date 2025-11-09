using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;

namespace ComparadorTasasCambio.App.Infraestructura.ClientesApi;

public class ClienteApi3 : ClienteApiBase
{
    public ClienteApi3(string urlApi, string apiKey, int timeout, ServicioLogger logger)
        : base(urlApi, apiKey, timeout, logger)
    {
    }

    public override string ObtenerNombreProveedor()
    {
        return "api3_json_anidado";
    }

    protected override async Task<OfertaCambio> ConsultarApiAsync(SolicitudCambio solicitud)
    {
        
        var requestData = new
        {
            exchange = new
            {
                sourceCurrency = solicitud.MonedaOrigen,
                targetCurrency = solicitud.MonedaDestino,
                quantity = solicitud.Monto
            }
        };

        var jsonContent = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);

        var timer = System.Diagnostics.Stopwatch.StartNew();
        HttpResponseMessage response = null;

        try
        {
            response = await _httpClient.PostAsync(_urlApi, content);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonSerializer.Deserialize<JsonElement>(responseText);

            
            var status = jsonDoc.GetProperty("statusCode").GetInt32();

            if (status != 200)
            {
                var errorMsg = jsonDoc.GetProperty("message").GetString();
                throw new Exception($"Error en API3: {errorMsg}");
            }

            var data = jsonDoc.GetProperty("data");
            var totalAmount = data.GetProperty("total").GetDecimal();

            var tasa = totalAmount / solicitud.Monto;

            return new OfertaCambio(
                ObtenerNombreProveedor(),
                totalAmount,
                tasa,
                timer.ElapsedMilliseconds,
                true
            );
        }
        catch (HttpRequestException ex)
        {
          
            _logger.Error($"Error HTTP en API3: {ex.Message}");
            throw;
        }
        finally
        {
            timer.Stop();
         
        }
    }
}