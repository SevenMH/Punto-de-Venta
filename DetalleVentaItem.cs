namespace P1
{
    public class DetalleVentaItem
    {
        public int IdLibro { get; set; }
        public int IdCategoria { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int StockDisponible { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
