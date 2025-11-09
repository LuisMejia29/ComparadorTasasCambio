namespace ComparadorTasasCambio.App.Dominio.Modelos;


public record SolicitudCambio
{
    public string MonedaOrigen { get; init; }
    public string MonedaDestino { get; init; }
    public decimal Monto { get; init; }

    public SolicitudCambio(string monedaOrigen, string monedaDestino, decimal monto)
    {
        // validar moneda origen
        if (string.IsNullOrWhiteSpace(monedaOrigen) || monedaOrigen.Length != 3)
        {
            throw new ArgumentException(
                "el código de moneda origen debe ser 3 letras (USD, EUR, DOP)",
                nameof(monedaOrigen));
        }

        // validad moneda destino
        if (string.IsNullOrWhiteSpace(monedaDestino) || monedaDestino.Length != 3)
        {
            throw new ArgumentException(
                "el código de moneda destino debe ser 3 letras (USD, EUR, DOP)",
                nameof(monedaDestino));
        }

        // Validar monto positivo
        if (monto <= 0)
        {
            throw new ArgumentException(
                $"el monto debe ser mayor que cero. Recibido: {monto}",
                nameof(monto));
        }

        // normalizar laa mayuscula
        MonedaOrigen = monedaOrigen.ToUpperInvariant();
        MonedaDestino = monedaDestino.ToUpperInvariant();
        Monto = monto;
    }
}