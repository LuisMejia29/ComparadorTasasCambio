using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorTasasCambio.App.Dominio.Modelos;

public record OfertaCambio
{
   // primer provedor
    public string NombreApi { get; init; }

   
    public decimal? MontoConvertido { get; init; }


    public decimal? TasaCambio { get; init; }

   
    public double TiempoRespuestaMs { get; init; }


    public bool Exitosa { get; init; }

   
    public string? MensajeError { get; init; }

  
    public DateTime FechaHora { get; init; }

    public OfertaCambio(
        string nombreApi,
        decimal? montoConvertido,
        decimal? tasaCambio,
        double tiempoRespuestaMs,
        bool exitosa,
        string? mensajeError = null)
    {
        if (string.IsNullOrWhiteSpace(nombreApi))
        {
            throw new ArgumentException("el nombre del api no puede estar vacio", nameof(nombreApi));
        }

        if (tiempoRespuestaMs < 0)
        {
            throw new ArgumentException("el tiempo de respuesta no puede ser negativo", nameof(tiempoRespuestaMs));
        }

        // si es exitoso 
        if (exitosa && (montoConvertido == null || tasaCambio == null))
        {
            throw new ArgumentException("oferta exitosa debe tener monto convertido y tasa de cambio");
        }

        // Ssi falla
        if (!exitosa && string.IsNullOrWhiteSpace(mensajeError))
        {
            throw new ArgumentException("oferta fallo");
        }

        NombreApi = nombreApi;
        MontoConvertido = montoConvertido;
        TasaCambio = tasaCambio;
        TiempoRespuestaMs = tiempoRespuestaMs;
        Exitosa = exitosa;
        MensajeError = mensajeError;
        FechaHora = DateTime.UtcNow;
    }
}