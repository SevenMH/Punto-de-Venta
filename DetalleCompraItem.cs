namespace P1
{
    public class DetalleCompraItem
    {
        public int IdLibro { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal => Cantidad * PrecioCompra;
    }
}
