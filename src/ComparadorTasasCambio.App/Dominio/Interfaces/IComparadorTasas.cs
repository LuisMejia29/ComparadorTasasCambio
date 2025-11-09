using ComparadorTasasCambio.App.Dominio.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorTasasCambio.App.Dominio.Interfaces;


/// compara ofertas.

public interface IComparadorTasas
{
  
    /// compara las tasas y devuelve la mejor tasa.

    Task<ResultadoComparacion> CompararTasasAsync(SolicitudCambio solicitud);
}