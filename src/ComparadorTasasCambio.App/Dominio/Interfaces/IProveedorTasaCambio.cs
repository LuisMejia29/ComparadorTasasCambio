using ComparadorTasasCambio.App.Dominio.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorTasasCambio.App.Dominio.Interfaces;

public interface IProveedorTasaCambio
{
  
    Task<OfertaCambio> ObtenerTasaCambioAsync(SolicitudCambio solicitud);
 

    string ObtenerNombreProveedor();

    Task<bool> EstaDisponibleAsync();
}