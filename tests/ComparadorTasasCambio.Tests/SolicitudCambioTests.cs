using ComparadorTasasCambio.App.Dominio.Modelos;

namespace ComparadorTasasCambio.Tests;

/// <summary>
/// Tests unitarios para la clase SolicitudCambio.
/// 
/// Cada método decorado con [Fact] es un test independiente que verifica
/// un aspecto específico del comportamiento de SolicitudCambio.
/// 
/// Los tests siguen el patrón AAA (Arrange-Act-Assert):
/// - Arrange: Preparar los datos y condiciones necesarias
/// - Act: Ejecutar el código que queremos probar
/// - Assert: Verificar que el resultado es el esperado
/// </summary>
public class SolicitudCambioTests
{
    /// <summary>
    /// Test que verifica el caso feliz: crear una solicitud con datos válidos.
    /// Este test confirma que cuando proporcionamos datos correctos, el objeto
    /// se crea exitosamente con esos valores.
    /// </summary>
    [Fact]
    public void DebeCrearSolicitudValida()
    {
        // Arrange: Preparar los datos de prueba
        var monedaOrigen = "USD";
        var monedaDestino = "EUR";
        var monto = 100m; // La 'm' al final indica que es un decimal

        // Act: Ejecutar el código que queremos probar
        var solicitud = new SolicitudCambio(monedaOrigen, monedaDestino, monto);

        // Assert: Verificar que el resultado es el esperado
        // Las propiedades del objeto deben tener exactamente los valores que pasamos
        Assert.Equal("USD", solicitud.MonedaOrigen);
        Assert.Equal("EUR", solicitud.MonedaDestino);
        Assert.Equal(100m, solicitud.Monto);
    }

    /// <summary>
    /// Test que verifica la normalización de códigos de moneda a mayúsculas.
    /// Esto es importante porque queremos aceptar códigos en minúsculas o mixtas
    /// del usuario pero siempre almacenarlos de forma consistente en mayúsculas.
    /// </summary>
    [Fact]
    public void DebeNormalizarMonedasAMayusculas()
    {
        // Arrange & Act: Crear solicitud con códigos en minúsculas
        var solicitud = new SolicitudCambio("usd", "eur", 100m);

        // Assert: Los códigos deben haberse convertido a mayúsculas automáticamente
        Assert.Equal("USD", solicitud.MonedaOrigen);
        Assert.Equal("EUR", solicitud.MonedaDestino);
    }

    /// <summary>
    /// Test que verifica que se lanza excepción si el monto es cero.
    /// No tiene sentido convertir cero pesos, así que esto debe ser rechazado.
    /// </summary>
    [Fact]
    public void DebeLanzarExcepcionSiMontoEsCero()
    {
        // Arrange & Act & Assert combinados
        // Assert.Throws verifica que el código dentro del lambda lanza la excepción esperada
        Assert.Throws<ArgumentException>(() =>
            new SolicitudCambio("USD", "EUR", 0m));
    }

    /// <summary>
    /// Test que verifica que se lanza excepción si el monto es negativo.
    /// Cantidades negativas no tienen sentido en este contexto.
    /// </summary>
    [Fact]
    public void DebeLanzarExcepcionSiMontoEsNegativo()
    {
        // Verificar que lanza excepción con monto negativo
        Assert.Throws<ArgumentException>(() =>
            new SolicitudCambio("USD", "EUR", -100m));
    }

    /// <summary>
    /// Test que verifica que se lanza excepción si la moneda origen es inválida.
    /// Los códigos de moneda deben tener exactamente 3 caracteres según ISO 4217.
    /// </summary>
    [Fact]
    public void DebeLanzarExcepcionSiMonedaOrigenEsInvalida()
    {
        // Verificar que lanza excepción si la moneda no tiene 3 caracteres
        Assert.Throws<ArgumentException>(() =>
            new SolicitudCambio("US", "EUR", 100m)); // Solo 2 letras
    }

    /// <summary>
    /// Test que verifica que se lanza excepción si la moneda origen está vacía.
    /// </summary>
    [Fact]
    public void DebeLanzarExcepcionSiMonedaOrigenEstaVacia()
    {
        // Verificar que lanza excepción con string vacío
        Assert.Throws<ArgumentException>(() =>
            new SolicitudCambio("", "EUR", 100m));
    }

    /// <summary>
    /// Test que verifica que se lanza excepción si la moneda destino es inválida.
    /// </summary>
    [Fact]
    public void DebeLanzarExcepcionSiMonedaDestinoEsInvalida()
    {
        // Verificar que lanza excepción si la moneda no tiene 3 caracteres
        Assert.Throws<ArgumentException>(() =>
            new SolicitudCambio("USD", "E", 100m)); // Solo 1 letra
    }

    /// <summary>
    /// Test que verifica la inmutabilidad usando códigos mixtos.
    /// También confirma que la normalización funciona con mayúsculas y minúsculas mezcladas.
    /// </summary>
    [Fact]
    public void DebeNormalizarCodigosConMayusculasYMinusculasMezcladas()
    {
        // Arrange & Act: Crear con códigos en formato mixto
        var solicitud = new SolicitudCambio("UsD", "EuR", 250.50m);

        // Assert: Todo debe estar en mayúsculas
        Assert.Equal("USD", solicitud.MonedaOrigen);
        Assert.Equal("EUR", solicitud.MonedaDestino);
        Assert.Equal(250.50m, solicitud.Monto);
    }
}