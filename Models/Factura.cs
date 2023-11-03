namespace Api.Models

{
    public class Factura
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string cif { get; set; }
        public string nombre { get; set; }
        public decimal importe { get; set; }
        public decimal importe_iva { get; set; }
        public string moneda { get; set; }
        public DateTime fecha_cobro { get; set; }
        public bool estado { get; set; }
    }
}