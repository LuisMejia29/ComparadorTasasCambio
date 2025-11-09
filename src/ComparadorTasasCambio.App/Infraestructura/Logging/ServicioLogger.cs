using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ComparadorTasasCambio.App.Infraestructura.Logging;


/// servicio de logging usando mmicrosoft.extensions.Logging

public class ServicioLogger
{
    private readonly ILogger _logger;

    public ServicioLogger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("comparadortasacambio");
    }

    public void Informacion(string mensaje, params object[] args)
    {
        _logger.LogInformation(mensaje, args);
    }

    public void Advertencia(string mensaje, params object[] args)
    {
        _logger.LogWarning(mensaje, args);
    }

    public void Error(string mensaje, params object[] args)
    {
        _logger.LogError(mensaje, args);
    }

    public void Error(Exception ex, string mensaje, params object[] args)
    {
        _logger.LogError(ex, mensaje, args);
    }

    public void Debug(string mensaje, params object[] args)
    {
        _logger.LogDebug(mensaje, args);
    }
}