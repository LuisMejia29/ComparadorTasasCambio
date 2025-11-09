using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ComparadorTasasCambio.App.Dominio.Modelos;

namespace ComparadorTasasCambio.Tests;

public class OfertaCambioTests
{
    [Fact]
    public void DebeCrearOfertaExitosa()
    {
        var oferta = new OfertaCambio("API1", 95.5m, 0.955m, 200, true);

        Assert.True(oferta.Exitosa);
        Assert.Equal("API1", oferta.NombreApi);
        Assert.Equal(95.5m, oferta.MontoConvertido);
        Assert.Null(oferta.MensajeError);
    }

    [Fact]
    public void DebeCrearOfertaFallida()
    {
        var oferta = new OfertaCambio("API2", null, null, 100, false, "Timeout");

        Assert.False(oferta.Exitosa);
        Assert.Null(oferta.MontoConvertido);
        Assert.Equal("Timeout", oferta.MensajeError);
    }

    [Fact]
    public void DebeLanzarExcepcion_SiOfertaExitosaSinMonto()
    {
        Assert.Throws<ArgumentException>(() =>
            new OfertaCambio("API1", null, null, 100, true));
    }

    [Fact]
    public void DebeLanzarExcepcion_SiOfertaFallidaSinError()
    {
        Assert.Throws<ArgumentException>(() =>
            new OfertaCambio("API1", null, null, 100, false));
    }
}

public class ResultadoComparacionTests
{
    [Fact]
    public void DebeCalcularEstadisticasCorrectamente()
    {
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);
        var ofertas = new List<OfertaCambio>
        {
            new OfertaCambio("API1", 95.5m, 0.955m, 100, true),
            new OfertaCambio("API2", null, null, 0, false, "Error"),
            new OfertaCambio("API3", 96.0m, 0.960m, 120, true)
        };

        var resultado = new ResultadoComparacion(solicitud, ofertas, 300);

        Assert.Equal(3, resultado.TotalApisConsultadas);
        Assert.Equal(2, resultado.ApisExitosas);
        Assert.Equal(1, resultado.ApisFallidas);
        Assert.Equal(66.7, resultado.PorcentajeExito, 1);
    }

    [Fact]
    public void DebeSeleccionarMejorOferta()
    {
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);
        var ofertas = new List<OfertaCambio>
        {
            new OfertaCambio("API1", 95.5m, 0.955m, 100, true),
            new OfertaCambio("API2", 96.5m, 0.965m, 150, true), // MEJOR
            new OfertaCambio("API3", 95.0m, 0.950m, 120, true)
        };

        var resultado = new ResultadoComparacion(solicitud, ofertas, 300);

        Assert.True(resultado.TieneOfertaValida);
        Assert.Equal("API2", resultado.MejorOferta!.NombreApi);
        Assert.Equal(96.5m, resultado.MejorOferta.MontoConvertido);
    }

    [Fact]
    public void NoDebeHaberMejorOferta_CuandoTodasFallan()
    {
        var solicitud = new SolicitudCambio("USD", "EUR", 100m);
        var ofertas = new List<OfertaCambio>
        {
            new OfertaCambio("API1", null, null, 0, false, "Error 1"),
            new OfertaCambio("API2", null, null, 0, false, "Error 2")
        };

        var resultado = new ResultadoComparacion(solicitud, ofertas, 300);

        Assert.False(resultado.TieneOfertaValida);
        Assert.Null(resultado.MejorOferta);
    }
}