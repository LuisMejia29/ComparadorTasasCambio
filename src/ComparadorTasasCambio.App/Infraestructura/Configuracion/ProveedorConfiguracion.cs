using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ComparadorTasasCambio.App.Infraestructura.Configuracion;


public class ProveedorConfiguracion
{
    private readonly IConfiguration _configuration;

    public ProveedorConfiguracion()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true);

        _configuration = builder.Build();
    }

    public string ObtenerValor(string clave, string valorPorDefecto = "")
    {
        return _configuration[clave] ?? valorPorDefecto;
    }

    public int ObtenerEntero(string clave, int valorPorDefecto = 0)
    {
        var valor = _configuration[clave];
        return int.TryParse(valor, out var resultado) ? resultado : valorPorDefecto;
    }

    public bool ObtenerBooleano(string clave, bool valorPorDefecto = false)
    {
        var valor = _configuration[clave];
        return bool.TryParse(valor, out var resultado) ? resultado : valorPorDefecto;
    }
}