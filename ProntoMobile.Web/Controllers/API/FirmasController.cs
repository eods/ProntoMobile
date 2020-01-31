using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FirmasController : Controller
    {
        private readonly DataContext _datacontextbase;
        private readonly DataContextERP _dataContext;
        private readonly IConverterHelper _converterHelper;
        private readonly IConfiguration _configuration;

        public FirmasController(
            DataContext datacontextbase,
            DataContextERP dataContext,
            IConverterHelper converterHelper,
            IConfiguration configuration)
        {
            _datacontextbase = datacontextbase;
            _dataContext = dataContext;
            _converterHelper = converterHelper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("GetFirmasByUser")]
        public async Task<IActionResult> GetFirmasByUser([FromBody] EmailRequest request)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Pronto", database.StringConection);

            //var firmas = await _dataContext._TempAutorizaciones
            //    .Include(a => a.Empleado)
            //    .Where(a => a.Empleado.Email.ToLower().Equals(request.Email.ToLower()))
            //    .OrderByDescending(a => a.Fecha)
            //    .ToListAsync();

            var firmas = await (from a in _dataContext._TempAutorizaciones
                                from b in _dataContext.Empleados.Where(o => o.IdEmpleado == a.IdAutoriza).DefaultIfEmpty()
                                from c in _dataContext.Pedidos.Where(o => o.IdPedido == a.IdComprobante && a.IdFormulario == 4).DefaultIfEmpty()
                                from d in _dataContext.ComprobantesProveedores.Where(o => o.IdComprobanteProveedor == a.IdComprobante && a.IdFormulario == 31).DefaultIfEmpty()
                                from e in _dataContext.Proveedores.Where(o => o.IdProveedor == c.IdProveedor).DefaultIfEmpty()
                                from f in _dataContext.Proveedores.Where(o => o.IdProveedor == d.IdProveedor).DefaultIfEmpty()
                                from g in _dataContext.Monedas.Where(o => o.IdMoneda == (c != null ? c.IdMoneda : d.IdMoneda)).DefaultIfEmpty()
                                select new FirmaResponse
                                {
                                    IdTempAutorizacion = a.IdTempAutorizacion,
                                    IdComprobante = a.IdComprobante,
                                    TipoComprobante = a.TipoComprobante,
                                    TipoComprobanteAb = a.TipoComprobante == "Comp.Proveedores" ? "CP" : (a.TipoComprobante == "Pedido" ? "PE" : (a.TipoComprobante == "R.M." ? "RM" : a.TipoComprobante)),
                                    Numero = a.Numero,
                                    IdAutorizacion = a.IdAutorizacion,
                                    IdFormulario = a.IdFormulario,
                                    IdDetalleAutorizacion = a.IdDetalleAutorizacion,
                                    SectorEmisor = a.SectorEmisor,
                                    OrdenAutorizacion = a.OrdenAutorizacion,
                                    IdAutoriza = a.IdAutoriza,
                                    IdSector = a.IdSector ?? 0,
                                    IdLibero = a.IdLibero ?? 0,
                                    Fecha = a.Fecha,
                                    Empleado = b != null ? b.Nombre : "",
                                    Email = b != null ? b.Email : "",
                                    Proveedor = e != null ? e.RazonSocial : (f != null ? f.RazonSocial : ""),
                                    IdMoneda = g != null ? g.IdMoneda : 0,
                                    Moneda = g != null ? g.Abreviatura : "",
                                    ImporteTotal = c != null ? c.TotalPedido : (d != null ? d.TotalComprobante : 0)
                                })
                                .Where(a => a.Email.ToLower().Equals(request.Email.ToLower()))
                                //.OrderByDescending(a => a.Fecha)
                                .OrderBy(a => a.TipoComprobante).OrderBy(a => a.Numero)
                                .ToListAsync();

            var response = new List<FirmaResponse>();
            foreach (var firma in firmas)
            {
                var firmaRespose = new FirmaResponse
                {
                    IdTempAutorizacion = firma.IdTempAutorizacion,
                    IdComprobante = firma.IdComprobante,
                    TipoComprobante = firma.TipoComprobante,
                    TipoComprobanteAb = firma.TipoComprobanteAb,
                    Numero = firma.Numero,
                    IdAutorizacion = firma.IdAutorizacion,
                    IdFormulario = firma.IdFormulario,
                    IdDetalleAutorizacion = firma.IdDetalleAutorizacion,
                    SectorEmisor = firma.SectorEmisor,
                    OrdenAutorizacion = firma.OrdenAutorizacion,
                    IdAutoriza = firma.IdAutoriza,
                    IdSector = firma.IdSector,
                    IdLibero = firma.IdLibero,
                    Fecha = firma.Fecha,
                    Empleado = firma.Empleado,
                    Proveedor = firma.Proveedor,
                    IdMoneda = firma.IdMoneda,
                    Moneda = firma.Moneda,
                    ImporteTotal = firma.ImporteTotal
                };

                response.Add(firmaRespose);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("GetFirmasById")]
        public async Task<IActionResult> GetFirmasById([FromBody] IdRequest request)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Pronto", database.StringConection);

            var firma = await _dataContext._TempAutorizaciones.FirstOrDefaultAsync(o => o.IdTempAutorizacion == request.Id);
            if (firma == null)
            {
                return NotFound();
            }

            var comprobante = new List<DetalleComprobanteResponse>();
            DateTime fecha = DateTime.Today;

            if (firma.IdFormulario == 4)
            {
                var pedido = await _dataContext.Pedidos
                                .Include(p => p.DetallePedidos)
                                .ThenInclude(p => p.Articulo)
                                .FirstOrDefaultAsync(p => p.IdPedido == firma.IdComprobante);

                fecha = pedido.FechaPedido;

                foreach (var p in pedido.DetallePedidos)
                {
                    var itemPedido = new DetalleComprobanteResponse
                    {
                        IdDetalleComprobante = p.IdDetallePedido,
                        IdComprobante = p.IdPedido,
                        FechaComprobante = p.Pedido.FechaPedido,
                        Detalle = p.Articulo.Descripcion,
                        Cantidad = p.Cantidad,
                        Importe = p.Precio
                    };

                    comprobante.Add(itemPedido);
                }
            }

            if (firma.IdFormulario == 31)
            {
                var comprobanteProveedor = await _dataContext.ComprobantesProveedores
                                            .Include(p => p.DetalleComprobantesProveedores)
                                            .ThenInclude(p => p.Cuenta)
                                            .FirstOrDefaultAsync(p => p.IdComprobanteProveedor == firma.IdComprobante);

                fecha = comprobanteProveedor.FechaComprobante;

                foreach (var c in comprobanteProveedor.DetalleComprobantesProveedores)
                {
                    var itemComprobante = new DetalleComprobanteResponse
                    {
                        IdDetalleComprobante = c.IdDetalleComprobanteProveedor,
                        IdComprobante = c.IdComprobanteProveedor,
                        FechaComprobante = c.ComprobanteProveedor.FechaComprobante,
                        Detalle = c.Cuenta.Descripcion,
                        Cantidad = 1,
                        Importe = c.Importe
                    };

                    comprobante.Add(itemComprobante);
                }
            }

            var response = new FirmaResponse
            {
                IdTempAutorizacion = request.Id,
                Numero = firma.Numero,
                Fecha = fecha,
                TipoComprobante = firma.TipoComprobante,
                DetallesComprobante = comprobante.Select(p => new DetalleComprobanteResponse
                {
                    IdDetalleComprobante = p.IdDetalleComprobante,
                    IdComprobante = p.IdComprobante,
                    Detalle = p.Detalle,
                    Cantidad = p.Cantidad,
                    Importe = p.Importe
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("FirmarDocumento")]
        public async Task<IActionResult> FirmarDocumento([FromBody] IdRequest request)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Pronto", database.StringConection);

            var firma = await _dataContext._TempAutorizaciones.FirstOrDefaultAsync(a => a.IdTempAutorizacion == request.Id);
            if (firma == null)
            {
                return BadRequest("Document doesn't exists");
            }

            var firma2 = await _dataContext.AutorizacionesPorComprobante.FirstOrDefaultAsync(a => a.IdFormulario == firma.IdFormulario && a.IdComprobante == firma.IdComprobante && a.OrdenAutorizacion == firma.OrdenAutorizacion);
            if (firma2 != null)
            {
                return BadRequest("existing signature");
            }

            var autorizacionPorComprobante = new AutorizacionPorComprobante
            {
                IdFormulario = firma.IdFormulario,
                IdComprobante = firma.IdComprobante,
                OrdenAutorizacion = firma.OrdenAutorizacion,
                IdAutorizo = firma.IdAutoriza,
                FechaAutorizacion = DateTime.Today
            };

            _dataContext.AutorizacionesPorComprobante.Add(autorizacionPorComprobante);
            await _dataContext.SaveChangesAsync();

            //var myParam = new System.Data.SqlClient.SqlParameter("@RespetarPrecedencia", "SI");
            //var result = _dataContext.AutorizacionesPorComprobante.FromSql("AutorizacionesPorComprobante_Generar @RespetarPrecedencia", myParam).FirstOrDefault();
            //_dataContext.Database.ExecuteSqlCommand("AutorizacionesPorComprobante_Generar @RespetarPrecedencia", parameters: new[] { "SI" });

            //Cuando devuelve una entidad o lista de entidad https://www.talkingdotnet.com/how-to-execute-stored-procedure-in-entity-framework-core/
            //List<Category> lst = dataContext.Categories.FromSql("usp_GetAllCategories").ToList();

            //Cuando devuelve un valor
            //SqlParameter param1 = new SqlParameter("@ProductID", 72);
            //var totalOrders = await context.Database.SqlQuery<int>("CountOfOrders @ProductID", param1).SingleAsync();

            //SqlConnection cnn = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection2"]);
            SqlConnection cnn = new SqlConnection(database.StringConection);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "AutorizacionesPorComprobante_Generar";
            //add any parameters the stored procedure might require
            cnn.Open();
            object o = cmd.ExecuteScalar();
            cnn.Close();

            var response = new FirmaResponse
            {
                IdTempAutorizacion = request.Id
            };

            return Ok(response);
        }

    }
}
