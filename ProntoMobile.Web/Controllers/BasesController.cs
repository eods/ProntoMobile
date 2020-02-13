using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers
{
    public class BasesController : Controller
    {
        private readonly DataContext _context;
        private readonly DataContextMANT _context2;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasesController(
            DataContext context,
            DataContextMANT context2,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _context2 = context2;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: ServiceTypes
        public async Task<IActionResult> Index()
        {
            //_httpContextAccessor.HttpContext.Session.SetString("String_Mantenimiento", "Server=SQLMVC;Database=ProntoMantenimiento_VialAgro;Persist Security Info=False;User ID=sa; Password=.SistemaPronto.;MultipleActiveResultSets=true");
            //var empleado1 = _context2.Empleados.ToList();

            //var a2 = _httpContextAccessor.HttpContext.Session.GetString("Test");


            //var database = _context.Bases.Where(a => a.Descripcion.ToLower().Equals("marcalba - mantenimiento")).FirstOrDefault();
            //HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            //var empleado = _context2.Empleados.Where(a => a.Email.ToLower().Equals("pronto@yopmail.com")).FirstOrDefault();
            //var idObraAsignada = empleado?.IdObraAsignada ?? 0;

            //var equipments = await _context2.Articulos
            //                    .Include(a => a.DetallePartesDiarios)
            //                    .ThenInclude(u => u.Unidad)
            //                    .Include(a => a.DetallePartesDiarios)
            //                    .ThenInclude(t => t.TipoHoraNoProductiva)
            //                    .Include(a => a.Fallas)
            //                    .Include(a => a.DetalleConsumos)
            //                    .ThenInclude(t => t.Consumible)
            //                    .Include(a => a.DetalleConsumos)
            //                    .ThenInclude(t => t.Unidad)
            //                    .Include(a => a.DetalleConsumos)
            //                    .ThenInclude(t => t.Consumo)
            //                    .Where(a => (a.ParaMantenimiento ?? "") == "SI" && (a.Activo ?? "") == "SI" && (idObraAsignada <= 0 || (a.IdObraActual ?? 0) == idObraAsignada))
            //                    .OrderBy(a => a.Descripcion).ToListAsync();

            //var response = new List<EquipmentResponse>();
            //foreach (var equipment in equipments)
            //{
            //    var idunidad = equipment.IdUnidadLecturaMantenimiento ?? 0;
            //    string unidad = "";
            //    if (idunidad != 0)
            //    {
            //        unidad = _context2.Unidades.Where(a => a.IdUnidad == idunidad).FirstOrDefault().Descripcion;
            //    }

            //    var equipmentRespose = new EquipmentResponse
            //    {
            //        IdArticulo = equipment.IdArticulo,
            //        Descripcion = equipment.Descripcion,
            //        Codigo = equipment.Codigo,
            //        FechaUltimaLectura = equipment.FechaUltimaLectura ?? DateTime.Today,
            //        UltimaLectura = equipment.UltimaLectura ?? 0,
            //        IdUnidadLecturaMantenimiento = idunidad,
            //        Unidad = unidad,
            //        //IdObraActual = equipment.IdObraActual ?? 0,
            //        ImageUrl = equipment.ImageFullPath,
            //        DetallePartesDiarios = equipment.DetallePartesDiarios?.Select(h => new DetalleParteDiarioResponse
            //        {
            //            IdDetalleParteDiario = h.IdDetalleParteDiario,
            //            IdEquipo = h.IdEquipo,
            //            FechaLectura = h.FechaLectura, // ?? DateTime.Today,
            //            Lectura = h.Lectura,
            //            IdUnidad = h.IdUnidad,
            //            IdTipoHoraNoProductiva = h.IdTipoHoraNoProductiva ?? 0,
            //            HorasProductivas = h.HorasProductivas ?? 0,
            //            HorasNoProductivas = h.HorasNoProductivas ?? 0,
            //            Unidad = h.Unidad.Descripcion,
            //            UnidadAb = h.Unidad.Abreviatura,
            //            TipoHoraNoProductiva = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Descripcion : ""),
            //            TipoHoraNoProductivaAb = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Abreviatura : "")
            //        }).Where(dp => dp.FechaLectura > DateTime.Today.AddYears(-1)).OrderByDescending(dp => dp.FechaLectura).ToList(),
            //        Fallas = equipment.Fallas?.Select(h => new FallaResponse
            //        {
            //            IdFalla = h.IdFalla,
            //            IdArticulo = h.IdArticulo,
            //            Descripcion = h.Descripcion,
            //            Anulada = h.Anulada,
            //            FechaFalla = (h.FechaFalla != null ? h.FechaFalla : DateTime.Today),
            //            Observaciones = h.Observaciones,
            //            IdOrdenTrabajo = h.IdOrdenTrabajo ?? 0,
            //            NumeroFalla = h.NumeroFalla ?? 0,
            //            FechaAlta = (h.FechaAlta != null ? h.FechaAlta : (h.FechaFalla != null ? h.FechaFalla : DateTime.Today)),
            //            IdObra = h.IdObra ?? 0,
            //            IdReporto = h.IdReporto ?? 0,
            //            Maquinista = h.Maquinista,
            //            Articulo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
            //            Reporto = (h.Empleado != null ? h.Empleado.Nombre : "")
            //        }).OrderByDescending(dp => dp.FechaFalla).ToList(),
            //        DetalleConsumos = equipment.DetalleConsumos?.Select(h => new DetalleConsumoResponse
            //        {
            //            IdDetalleConsumo = h.IdDetalleConsumo,
            //            IdConsumo = h.IdConsumo,
            //            IdArticulo = h.IdArticulo,
            //            IdConsumible = h.IdConsumible,
            //            Cantidad = h.Cantidad,
            //            IdUnidadConsumible = h.IdUnidadConsumible,
            //            Costo = h.Costo,
            //            Observaciones = h.Observaciones,
            //            Equipo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
            //            Consumible = (h.Consumible != null ? h.Consumible.Descripcion : ""),
            //            Unidad = (h.Unidad != null ? h.Unidad.Descripcion : ""),
            //            UnidadAb = (h.Unidad != null ? h.Unidad.Abreviatura : ""),
            //            FechaConsumo = (h.Consumo != null ? h.Consumo.FechaConsumo : DateTime.Today),
            //            Consumo = (h.Consumo != null ? h.Consumo.NumeroConsumo.ToString() : "")
            //        }).Where(dp => dp.FechaConsumo > DateTime.Today.AddYears(-1)).OrderByDescending(dp => dp.FechaConsumo).ToList(),
            //    };
            //    response.Add(equipmentRespose);
            //};


            //var a = _context2.TiposHorasNoProductivas.OrderBy(u => u.Descripcion);


            //var database = _context.Bases.Where(a => a.Descripcion.ToLower().Equals("marcalba - mantenimiento")).FirstOrDefault();
            //HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            //int NumeroFalla = 0;

            //SqlConnection cnn = new SqlConnection(database.StringConection);
            //cnn.Open();

            //SqlCommand command = new SqlCommand("Select * from Parametros where IdParametro=1", cnn);
            //SqlDataReader reader = command.ExecuteReader();
            //if (reader.Read())
            //{
            //    NumeroFalla = Int32.Parse(reader["ProximoNumeroFalla"].ToString());
            //}
            //reader.Close();

            //SqlCommand cmd = new SqlCommand("Update Parametros Set ProximoNumeroFalla=IsNull(ProximoNumeroFalla,0) + 1 Where IdParametro=1", cnn);
            //cmd.ExecuteNonQuery();
            //cmd.Dispose();
            //cnn.Close();

            return View(await _context.Bases.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Base base1)
        {
            if (ModelState.IsValid)
            {
                _context.Bases.Add(base1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(base1);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.Bases.FindAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Base base1)
        {
            if (id != base1.IdBD)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(base1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaseExists(base1.IdBD))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(base1);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var base1 = await _context.Bases
                .FirstOrDefaultAsync(st => st.IdBD == id);

            if (base1 == null)
            {
                return NotFound();
            }

            var det = await _context.DetalleUserBDs.Where(o => o.IdBD == id.Value).ToListAsync();

            if (det.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "La base no puede ser eliminada porque tiene registros asociados");
                return RedirectToAction($"{nameof(Index)}");
            }

            _context.Bases.Remove(base1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaseExists(int id)
        {
            return _context.Bases.Any(e => e.IdBD == id);
        }
    }
}
