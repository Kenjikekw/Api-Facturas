using Api.Models;

namespace Api.Services
{
    public interface IFacturasService
    {
        List<Factura> GetAllFactura();
        Task<Factura> GetFactura(int Id);
        Task<Factura> CreateFactura(Factura factura);
        Task<Factura> DeleteFactura(int Id);
        Task<Factura> EditFactura(Factura factura);
    }
}