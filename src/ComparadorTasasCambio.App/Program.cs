using ComparadorTasasCambio.App.Aplicacion.CasosDeUso;
using ComparadorTasasCambio.App.Aplicacion.Servicios;
using ComparadorTasasCambio.App.Dominio.Interfaces;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.ClientesApi;
using ComparadorTasasCambio.App.Infraestructura.Configuracion;
using ComparadorTasasCambio.App.Infraestructura.Logging;
using Microsoft.Extensions.Logging;

namespace ComparadorTasasCambio.App;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("comparador de tasa\n");

        try
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            var logger = new ServicioLogger(loggerFactory);
            var config = new ProveedorConfiguracion();

            var proveedores = CrearProveedores(config, logger);
            IComparadorTasas comparador = new ServicioComparadorTasas(proveedores, logger);
            var useCase = new ObtenerMejorTasaCambioUseCase(comparador, logger);

            string monedaOrigen, monedaDestino;
            decimal monto;
            var startTime = DateTime.Now; 

            if (args.Length >= 3)
            {
                monedaOrigen = args[0];
                monedaDestino = args[1];
                monto = decimal.Parse(args[2]); 
            }
            else
            {
                monedaOrigen = "USD";
                monedaDestino = "DOP";
                monto = 100m;
                Console.WriteLine($"valores por defecto {monto} {monedaOrigen} -> {monedaDestino}\n");
            }

            var resultado = await useCase.EjecutarAsync(monedaOrigen, monedaDestino, monto);
            MostrarResultado(resultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            Environment.Exit(1);
        }
       
    }

    static List<IProveedorTasaCambio> CrearProveedores(ProveedorConfiguracion config, ServicioLogger logger)
    {
        var proveedores = new List<IProveedorTasaCambio>();


        var api1Url = config.ObtenerValor("api1_url", "https://api1.ejemplo.com");
        var api1Key = config.ObtenerValor("api1_key", "key1");
        var api1Timeout = config.ObtenerEntero("api1_timeout", 5);
        proveedores.Add(new ClienteApi1(api1Url, api1Key, api1Timeout, logger));

        var api2Url = config.ObtenerValor("api2_url", "https://api2.ejemplo.com");
        var api2Key = config.ObtenerValor("api2_key", "key2");
        var api2Timeout = config.ObtenerEntero("api2_timeout", 5);
        proveedores.Add(new ClienteApi2(api2Url, api2Key, api2Timeout, logger));

        var api3Url = config.ObtenerValor("api3_url", "https://api3.ejemplo.com");
        var api3Key = config.ObtenerValor("api3_key", "key3");
        var api3Timeout = config.ObtenerEntero("api3_timeout", 5);
        proveedores.Add(new ClienteApi3(api3Url, api3Key, api3Timeout, logger));

        return proveedores;
    }

    static void MostrarResultado(ResultadoComparacion resultado)
    {
        Console.WriteLine("\n resultado ");
        Console.WriteLine($"conversion: {resultado.Solicitud.Monto} {resultado.Solicitud.MonedaOrigen} -> {resultado.Solicitud.MonedaDestino}");
        Console.WriteLine($"apis consultadas: {resultado.TotalApisConsultadas} (exitosas: {resultado.ApisExitosas}, fallidas: {resultado.ApisFallidas})");
        Console.WriteLine($"Tiempo: {resultado.TiempoTotalMs:F2} ms\n");

        if (resultado.TieneOfertaValida)
        {
            var mejor = resultado.MejorOferta!; 
            Console.WriteLine("Mejor oferta:");
            Console.WriteLine($"  api: {mejor.NombreApi}");
            Console.WriteLine($"  recibiremos: {mejor.MontoConvertido} {resultado.Solicitud.MonedaDestino}");
            Console.WriteLine($"  tasa: {mejor.TasaCambio:F6}");
            Console.WriteLine($"  tiempo: {mejor.TiempoRespuestaMs:F2} ms");
        }
        else
        {
            Console.WriteLine("no se pudo obtener ninguna oferta valida");
        }

        Console.WriteLine("\nDetalle:");
        var contador = 0; 
        foreach (var oferta in resultado.TodasLasOfertas)
        {
            if (oferta.Exitosa)
            {
                
                Console.WriteLine($"  [{oferta.NombreApi}] {oferta.MontoConvertido} {resultado.Solicitud.MonedaDestino} ({oferta.TiempoRespuestaMs:F2} ms)");
            }
            else
            {
                Console.WriteLine($"  [{oferta.NombreApi}] Error: {oferta.MensajeError}");
            }
            contador++;
        }
       
        Console.WriteLine();
    }
}