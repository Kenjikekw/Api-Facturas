using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturasService _facturasService;

        public FacturaController(IFacturasService facturasService)
        {
            _facturasService = facturasService;
        }

        [HttpGet]
        public ActionResult<List<Factura>> GetAllFacturas()
        {
            var facturas = _facturasService.GetAllFactura();
            if(!facturas.Any()){
                return NotFound("No se encontro ninguna factura");
            }
            return Ok(facturas);
        }


    }
}