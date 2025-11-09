using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorTasasCambio.App.Dominio.Modelos;


public record ResultadoComparacion
{
   
    public SolicitudCambio Solicitud { get; init; }

 
    public OfertaCambio? MejorOferta { get; init; }

   
    public List<OfertaCambio> TodasLasOfertas { get; init; }

 
    public int TotalApisConsultadas { get; init; }

    
    public int ApisExitosas { get; init; }

    
    public int ApisFallidas { get; init; }

 
    public double TiempoTotalMs { get; init; }

  
    public DateTime FechaHora { get; init; }

  
    public bool TieneOfertaValida => MejorOferta != null && MejorOferta.Exitosa;

    public double PorcentajeExito => TotalApisConsultadas == 0 ? 0 : (ApisExitosas * 100.0) / TotalApisConsultadas;

    public ResultadoComparacion(
        SolicitudCambio solicitud,
        List<OfertaCambio> todasLasOfertas,
        double tiempoTotalMs)
    {
        if (solicitud == null)
        {
            throw new ArgumentNullException(nameof(solicitud));
        }

        if (todasLasOfertas == null)
        {
            throw new ArgumentNullException(nameof(todasLasOfertas));
        }

        if (tiempoTotalMs < 0)
        {
            throw new ArgumentException("el tiempo total no puede ser negativo", nameof(tiempoTotalMs));
        }

        Solicitud = solicitud;
        TodasLasOfertas = todasLasOfertas;
        TiempoTotalMs = tiempoTotalMs;
        FechaHora = DateTime.UtcNow;

        // calcular estadísticas
        TotalApisConsultadas = todasLasOfertas.Count;
        ApisExitosas = todasLasOfertas.Count(o => o.Exitosa);
        ApisFallidas = TotalApisConsultadas - ApisExitosas;

        // seleccionar la mejor oferta
        MejorOferta = todasLasOfertas
            .Where(o => o.Exitosa && o.MontoConvertido.HasValue)
            .OrderByDescending(o => o.MontoConvertido!.Value)
            .FirstOrDefault();
    }
}