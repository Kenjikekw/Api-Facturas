using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class FacturasService : IFacturasService
    {
        private readonly DataContext _context;

        public FacturasService(DataContext context)
        {
            _context = context;
        }
        public async Task<Factura> CreateFactura(Factura factura)
        {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
            return factura;
        }

        public Task<Factura> DeleteFactura(int Id)
        {
                        throw new NotImplementedException();

        }

        public async Task<Factura> EditFactura(Factura factura)
        {
            _context.Facturas.Update(factura);
            await _context.SaveChangesAsync();
            return factura;
        }

        public List<Factura> GetAllFactura()
        {
          List<Factura> factura =  _context.Facturas.ToList();
          return factura;
        }

        public Task<Factura> GetFactura(int Id)
        {
            throw new NotImplementedException();
        }
    }





}