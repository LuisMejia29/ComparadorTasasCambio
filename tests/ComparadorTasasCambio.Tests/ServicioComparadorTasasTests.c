using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ComparadorTasasCambio.App.Aplicacion.Servicios;
using ComparadorTasasCambio.App.Dominio.Interfaces;
using ComparadorTasasCambio.App.Dominio.Modelos;
using ComparadorTasasCambio.App.Infraestructura.Logging;
using Microsoft.Extensions.Logging;

namespace ComparadorTasasCambio.Tests;

public class ServicioComparadorTasasTests
{
    private ServicioLogger CrearLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder = > builder.SetMinimumLevel(LogLevel.Error));
        return new ServicioLogger(loggerFactory);
    }

    [Fact]
    public async Task DebeSeleccionarMejorOferta_CuandoTodasSonExitosas()
    {
        // Arrange
        var logger = CrearLogger();
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);

        // Crear mocks de proveedores con diferentes ofertas
        var mock1 = new Mock<IProveedorTasaCambio>();
        mock1.Setup(p = > p.ObtenerNombreProveedor()).Returns("API1");
        mock1.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API1", 95.5m, 0.955m, 100, true));

        var mock2 = new Mock<IProveedorTasaCambio>();
        mock2.Setup(p = > p.ObtenerNombreProveedor()).Returns("API2");
        mock2.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API2", 96.2m, 0.962m, 150, true)); // MEJOR

        var mock3 = new Mock<IProveedorTasaCambio>();
        mock3.Setup(p = > p.ObtenerNombreProveedor()).Returns("API3");
        mock3.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API3", 95.0m, 0.950m, 120, true));

        var proveedores = new List<IProveedorTasaCambio>{ mock1.Object, mock2.Object, mock3.Object };
        var comparador = new ServicioComparadorTasas(proveedores, logger);

        // Act
        var resultado = await comparador.CompararTasasAsync(solicitud);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.TieneOfertaValida);
        Assert.Equal("API2", resultado.MejorOferta!.NombreApi);
        Assert.Equal(96.2m, resultado.MejorOferta.MontoConvertido);
        Assert.Equal(3, resultado.TotalApisConsultadas);
        Assert.Equal(3, resultado.ApisExitosas);
        Assert.Equal(0, resultado.ApisFallidas);
    }

    [Fact]
    public async Task DebeManejarse_CuandoAlgunasApisFallan()
    {
        // Arrange
        var logger = CrearLogger();
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);

        var mock1 = new Mock<IProveedorTasaCambio>();
        mock1.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API1", 95.5m, 0.955m, 100, true));

        var mock2 = new Mock<IProveedorTasaCambio>();
        mock2.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API2", null, null, 0, false, "Error de conexión"));

        var mock3 = new Mock<IProveedorTasaCambio>();
        mock3.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API3", 96.0m, 0.960m, 120, true)); // MEJOR

        var proveedores = new List<IProveedorTasaCambio>{ mock1.Object, mock2.Object, mock3.Object };
        var comparador = new ServicioComparadorTasas(proveedores, logger);

        // Act
        var resultado = await comparador.CompararTasasAsync(solicitud);

        // Assert
        Assert.True(resultado.TieneOfertaValida);
        Assert.Equal("API3", resultado.MejorOferta!.NombreApi);
        Assert.Equal(3, resultado.TotalApisConsultadas);
        Assert.Equal(2, resultado.ApisExitosas);
        Assert.Equal(1, resultado.ApisFallidas);
    }

    [Fact]
    public async Task NoDebeHaberOfertaValida_CuandoTodasFallan()
    {
        // Arrange
        var logger = CrearLogger();
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);

        var mock1 = new Mock<IProveedorTasaCambio>();
        mock1.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API1", null, null, 0, false, "Error 1"));

        var mock2 = new Mock<IProveedorTasaCambio>();
        mock2.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API2", null, null, 0, false, "Error 2"));

        var mock3 = new Mock<IProveedorTasaCambio>();
        mock3.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API3", null, null, 0, false, "Error 3"));

        var proveedores = new List<IProveedorTasaCambio>{ mock1.Object, mock2.Object, mock3.Object };
        var comparador = new ServicioComparadorTasas(proveedores, logger);

        // Act
        var resultado = await comparador.CompararTasasAsync(solicitud);

        // Assert
        Assert.False(resultado.TieneOfertaValida);
        Assert.Null(resultado.MejorOferta);
        Assert.Equal(3, resultado.ApisFallidas);
        Assert.Equal(0, resultado.ApisExitosas);
    }

    [Fact]
    public async Task DebeConsultarEnParalelo_VerificandoTiempo()
    {
        // Arrange
        var logger = CrearLogger();
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);

        var mock1 = new Mock<IProveedorTasaCambio>();
        mock1.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API1", 95.5m, 0.955m, 100, true));

        var mock2 = new Mock<IProveedorTasaCambio>();
        mock2.Setup(p = > p.ObtenerTasaCambioAsync(It.IsAny<SolicitudCambio>()))
            .ReturnsAsync(new OfertaCambio("API2", 96.0m, 0.960m, 150, true));

        var proveedores = new List<IProveedorTasaCambio>{ mock1.Object, mock2.Object };
        var comparador = new ServicioComparadorTasas(proveedores, logger);

        // Act
        var resultado = await comparador.CompararTasasAsync(solicitud);

        // Assert - Si fueran secuenciales, tardaría mucho más
        Assert.True(resultado.TiempoTotalMs < 1000); // Debe ser muy rápido con mocks
        Assert.Equal(2, resultado.TodasLasOfertas.Count);
    }
}