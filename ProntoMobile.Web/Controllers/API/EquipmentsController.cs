using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentsController : ControllerBase
    {
        private readonly DataContext _datacontextbase;
        private readonly DataContextMANT _dataContext;
        private readonly IConverterHelper _converterHelper;

        public EquipmentsController(
            DataContext datacontextbase,
            DataContextMANT datacontext,
            IConverterHelper converterHelper)
        {
            _datacontextbase = datacontextbase;
            _dataContext = datacontext;
            _converterHelper = converterHelper;
        }

        [HttpGet]
        public IEnumerable<Articulo> GetEquipments()
        {
            return _dataContext.Articulos.OrderBy(a => a.Descripcion);
        }

        [HttpGet]
        [Route("GetEquipments2")]
        public async Task<IActionResult> GetEquipments2()
        {
            var equipments = await _dataContext.Articulos
                                .Include(a => a.DetallePartesDiarios)
                                .ThenInclude(u => u.Unidad)
                                .Include(a => a.DetallePartesDiarios)
                                .ThenInclude(t => t.TipoHoraNoProductiva)
                                .Include(a => a.Fallas)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Consumible)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Unidad)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Consumo)
                                .Where(a => (a.ParaMantenimiento ?? "") == "SI" && (a.Activo ?? "") == "SI")
                                .OrderBy(a => a.Descripcion).ToListAsync();

            var response = new List<EquipmentResponse>();
            foreach (var equipment in equipments)
            {
                var idunidad = equipment.IdUnidadLecturaMantenimiento ?? 0;
                string unidad = "";
                if (idunidad != 0)
                {
                    unidad = _dataContext.Unidades.Where(a => a.IdUnidad == idunidad).FirstOrDefault().Descripcion;
                }

                var equipmentRespose = new EquipmentResponse
                {
                    IdArticulo = equipment.IdArticulo,
                    Descripcion = equipment.Descripcion,
                    Codigo = equipment.Codigo,
                    FechaUltimaLectura = equipment.FechaUltimaLectura ?? DateTime.Today,
                    UltimaLectura = equipment.UltimaLectura ?? 0,
                    IdUnidadLecturaMantenimiento = idunidad,
                    Unidad = unidad,
                    //IdObraActual = equipment.IdObraActual ?? 0,
                    ImageUrl = equipment.ImageFullPath,
                    DetallePartesDiarios = equipment.DetallePartesDiarios?.Select(h => new DetalleParteDiarioResponse
                    {
                        IdDetalleParteDiario = h.IdDetalleParteDiario,
                        IdEquipo = h.IdEquipo,
                        FechaLectura = h.FechaLectura, // ?? DateTime.Today,
                        Lectura = h.Lectura,
                        IdUnidad = h.IdUnidad,
                        IdTipoHoraNoProductiva = h.IdTipoHoraNoProductiva ?? 0,
                        HorasProductivas = h.HorasProductivas ?? 0,
                        HorasNoProductivas = h.HorasNoProductivas ?? 0,
                        Unidad = h.Unidad.Descripcion,
                        UnidadAb = h.Unidad.Abreviatura,
                        TipoHoraNoProductiva = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Descripcion : ""),
                        TipoHoraNoProductivaAb = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Abreviatura : "")
                    }).ToList(),
                    Fallas = equipment.Fallas?.Select(h => new FallaResponse
                    {
                        IdFalla = h.IdFalla,
                        IdArticulo = h.IdArticulo,
                        Descripcion = h.Descripcion,
                        Anulada = h.Anulada,
                        FechaFalla = h.FechaFalla,
                        Observaciones = h.Observaciones,
                        IdOrdenTrabajo = h.IdOrdenTrabajo,
                        NumeroFalla = h.NumeroFalla,
                        FechaAlta = h.FechaAlta,
                        IdObra = h.IdObra,
                        IdReporto = h.IdReporto,
                        Maquinista = h.Maquinista,
                        Articulo = h.Articulo.Descripcion,
                        Reporto = h.Empleado.Nombre
                        //TipoHoraNoProductivaAb = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Abreviatura : "")
                    }).ToList(),
                    DetalleConsumos = equipment.DetalleConsumos?.Select(h => new DetalleConsumoResponse
                    {
                        IdDetalleConsumo = h.IdDetalleConsumo,
                        IdConsumo = h.IdConsumo,
                        IdArticulo = h.IdArticulo,
                        IdConsumible = h.IdConsumible,
                        Cantidad = h.Cantidad,
                        IdUnidadConsumible = h.IdUnidadConsumible,
                        Costo = h.Costo,
                        Observaciones = h.Observaciones,
                        Equipo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
                        Consumible = (h.Consumible != null ? h.Consumible.Descripcion : ""),
                        Unidad = (h.Unidad != null ? h.Unidad.Descripcion : ""),
                        UnidadAb = (h.Unidad != null ? h.Unidad.Abreviatura : ""),
                        FechaConsumo = (h.Consumo != null ? h.Consumo.FechaConsumo : DateTime.Today),
                        Consumo = (h.Consumo != null ? h.Consumo.NumeroConsumo.ToString() : "")
                    }).Where(dp => dp.FechaConsumo > DateTime.Today.AddYears(-1)).OrderByDescending(dp => dp.FechaConsumo).ToList(),
                };

                response.Add(equipmentRespose);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("GetEquipments3")]
        public async Task<IActionResult> GetEquipments3(EmailRequest emailRequest)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(emailRequest.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            var empleado = _dataContext.Empleados.Where(a => a.Email.ToLower().Equals(emailRequest.Email.ToLower())).FirstOrDefault();
            var idObraAsignada = empleado?.IdObraAsignada ?? 0;

            var equipments = await _dataContext.Articulos
                                .Include(a => a.DetallePartesDiarios)
                                .ThenInclude(u => u.Unidad)
                                .Include(a => a.DetallePartesDiarios)
                                .ThenInclude(t => t.TipoHoraNoProductiva)
                                .Include(a => a.Fallas)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Consumible)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Unidad)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Consumo)
                                .Where(a => (a.ParaMantenimiento ?? "") == "SI" && (a.Activo ?? "") == "SI" && (idObraAsignada <= 0 || (a.IdObraActual ?? 0) == idObraAsignada))
                                .OrderBy(a => a.Descripcion).ToListAsync();

            var response = new List<EquipmentResponse>();
            foreach (var equipment in equipments)
            {
                var idunidad = equipment.IdUnidadLecturaMantenimiento ?? 0;
                string unidad = "";
                if (idunidad != 0)
                {
                    unidad = _dataContext.Unidades.Where(a => a.IdUnidad == idunidad).FirstOrDefault().Descripcion;
                }

                var equipmentRespose = new EquipmentResponse
                {
                    IdArticulo = equipment.IdArticulo,
                    Descripcion = equipment.Descripcion,
                    Codigo = equipment.Codigo,
                    FechaUltimaLectura = equipment.FechaUltimaLectura ?? DateTime.Today,
                    UltimaLectura = equipment.UltimaLectura ?? 0,
                    IdUnidadLecturaMantenimiento = idunidad,
                    Unidad = unidad,
                    //IdObraActual = equipment.IdObraActual ?? 0,
                    ImageUrl = equipment.ImageFullPath,
                    DetallePartesDiarios = equipment.DetallePartesDiarios?.Select(h => new DetalleParteDiarioResponse
                    {
                        IdDetalleParteDiario = h.IdDetalleParteDiario,
                        IdEquipo = h.IdEquipo,
                        FechaLectura = h.FechaLectura, // ?? DateTime.Today,
                        Lectura = h.Lectura,
                        IdUnidad = h.IdUnidad,
                        IdTipoHoraNoProductiva = h.IdTipoHoraNoProductiva ?? 0,
                        HorasProductivas = h.HorasProductivas ?? 0,
                        HorasNoProductivas = h.HorasNoProductivas ?? 0,
                        Unidad = h.Unidad.Descripcion,
                        UnidadAb = h.Unidad.Abreviatura,
                        TipoHoraNoProductiva = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Descripcion : ""),
                        TipoHoraNoProductivaAb = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Abreviatura : "")
                    }).Where(dp => dp.FechaLectura > DateTime.Today.AddMonths(-6)).OrderByDescending(dp => dp.FechaLectura).ToList(),
                    Fallas = equipment.Fallas?.Select(h => new FallaResponse
                    {
                        IdFalla = h.IdFalla,
                        IdArticulo = h.IdArticulo,
                        Descripcion = h.Descripcion,
                        Anulada = h.Anulada,
                        FechaFalla = (h.FechaFalla != null ? h.FechaFalla : DateTime.Today),
                        Observaciones = h.Observaciones,
                        IdOrdenTrabajo = h.IdOrdenTrabajo ?? 0,
                        NumeroFalla = h.NumeroFalla ?? 0,
                        FechaAlta = (h.FechaAlta != null ? h.FechaAlta : (h.FechaFalla != null ? h.FechaFalla : DateTime.Today)),
                        IdObra = h.IdObra ?? 0,
                        IdReporto = h.IdReporto ?? 0,
                        Maquinista = h.Maquinista,
                        Articulo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
                        Reporto = (h.Empleado != null ? h.Empleado.Nombre : "")
                    }).OrderByDescending(dp => dp.FechaFalla).ToList(),
                    DetalleConsumos = equipment.DetalleConsumos?.Select(h => new DetalleConsumoResponse
                    {
                        IdDetalleConsumo = h.IdDetalleConsumo,
                        IdConsumo = h.IdConsumo,
                        IdArticulo = h.IdArticulo,
                        IdConsumible = h.IdConsumible,
                        Cantidad = h.Cantidad,
                        IdUnidadConsumible = h.IdUnidadConsumible,
                        Costo = h.Costo,
                        Observaciones = h.Observaciones,
                        Equipo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
                        Consumible = (h.Consumible != null ? h.Consumible.Descripcion : ""),
                        Unidad = (h.Unidad != null ? h.Unidad.Descripcion : ""),
                        UnidadAb = (h.Unidad != null ? h.Unidad.Abreviatura : ""),
                        FechaConsumo = (h.Consumo != null ? h.Consumo.FechaConsumo : DateTime.Today),
                        Consumo = (h.Consumo != null ? h.Consumo.NumeroConsumo.ToString() : "")
                    }).Where(dp => dp.FechaConsumo > DateTime.Today.AddMonths(-6)).OrderByDescending(dp => dp.FechaConsumo).ToList(),
                };

                response.Add(equipmentRespose);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("GetEquipmentById")]
        public async Task<IActionResult> GetEquipmentById(IdRequest idRequest)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(idRequest.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            var equipment = await _dataContext.Articulos
                                .Include(a => a.DetallePartesDiarios)
                                .ThenInclude(u => u.Unidad)
                                .Include(a => a.DetallePartesDiarios)
                                .ThenInclude(t => t.TipoHoraNoProductiva)
                                .Include(a => a.Fallas)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Consumible)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Unidad)
                                .Include(a => a.DetalleConsumos)
                                .ThenInclude(t => t.Consumo)
                                .Where(a => a.IdArticulo == idRequest.Id).FirstOrDefaultAsync();

            var idunidad = equipment.IdUnidadLecturaMantenimiento ?? 0;
            string unidad = "";
            if (idunidad != 0)
            {
                unidad = _dataContext.Unidades.Where(a => a.IdUnidad == idunidad).FirstOrDefault().Descripcion;
            }

            var response = new EquipmentResponse
            {
                IdArticulo = equipment.IdArticulo,
                Descripcion = equipment.Descripcion,
                Codigo = equipment.Codigo,
                FechaUltimaLectura = equipment.FechaUltimaLectura ?? DateTime.Today,
                UltimaLectura = equipment.UltimaLectura ?? 0,
                IdUnidadLecturaMantenimiento = idunidad,
                Unidad = unidad,
                //IdObraActual = equipment.IdObraActual ?? 0,
                ImageUrl = equipment.ImageFullPath,
                DetallePartesDiarios = equipment.DetallePartesDiarios?.Select(h => new DetalleParteDiarioResponse
                {
                    IdDetalleParteDiario = h.IdDetalleParteDiario,
                    IdEquipo = h.IdEquipo,
                    FechaLectura = h.FechaLectura, // ?? DateTime.Today,
                    Lectura = h.Lectura,
                    IdUnidad = h.IdUnidad,
                    IdTipoHoraNoProductiva = h.IdTipoHoraNoProductiva ?? 0,
                    HorasProductivas = h.HorasProductivas ?? 0,
                    HorasNoProductivas = h.HorasNoProductivas ?? 0,
                    Unidad = h.Unidad.Descripcion,
                    UnidadAb = h.Unidad.Abreviatura,
                    TipoHoraNoProductiva = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Descripcion : ""),
                    TipoHoraNoProductivaAb = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Abreviatura : "")
                }).Where(dp => dp.FechaLectura > DateTime.Today.AddMonths(-6)).OrderByDescending(dp => dp.FechaLectura).ToList(),
                Fallas = equipment.Fallas?.Select(h => new FallaResponse
                {
                    IdFalla = h.IdFalla,
                    IdArticulo = h.IdArticulo,
                    Descripcion = h.Descripcion,
                    Anulada = h.Anulada,
                    FechaFalla = (h.FechaFalla != null ? h.FechaFalla : DateTime.Today),
                    Observaciones = h.Observaciones,
                    IdOrdenTrabajo = h.IdOrdenTrabajo ?? 0,
                    NumeroFalla = h.NumeroFalla ?? 0,
                    FechaAlta = (h.FechaAlta != null ? h.FechaAlta : (h.FechaFalla != null ? h.FechaFalla : DateTime.Today)),
                    IdObra = h.IdObra ?? 0,
                    IdReporto = h.IdReporto ?? 0,
                    Maquinista = h.Maquinista,
                    Articulo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
                    Reporto = (h.Empleado != null ? h.Empleado.Nombre : "")
                }).OrderByDescending(dp => dp.FechaFalla).ToList(),
                DetalleConsumos = equipment.DetalleConsumos?.Select(h => new DetalleConsumoResponse
                {
                    IdDetalleConsumo = h.IdDetalleConsumo,
                    IdConsumo = h.IdConsumo,
                    IdArticulo = h.IdArticulo,
                    IdConsumible = h.IdConsumible,
                    Cantidad = h.Cantidad,
                    IdUnidadConsumible = h.IdUnidadConsumible,
                    Costo = h.Costo,
                    Observaciones = h.Observaciones,
                    Equipo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
                    Consumible = (h.Consumible != null ? h.Consumible.Descripcion : ""),
                    Unidad = (h.Unidad != null ? h.Unidad.Descripcion : ""),
                    UnidadAb = (h.Unidad != null ? h.Unidad.Abreviatura : ""),
                    FechaConsumo = (h.Consumo != null ? h.Consumo.FechaConsumo : DateTime.Today),
                    Consumo = (h.Consumo != null ? h.Consumo.NumeroConsumo.ToString() : "")
                }).Where(dp => dp.FechaConsumo > DateTime.Today.AddMonths(-6)).OrderByDescending(dp => dp.FechaConsumo).ToList(),
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipments([FromRoute] int id)
        {
            var equipment = await _dataContext.Articulos
                .Include(a => a.DetallePartesDiarios)
                .ThenInclude(u => u.Unidad)
                .Include(a => a.DetallePartesDiarios)
                .ThenInclude(t => t.TipoHoraNoProductiva)
                .Include(a => a.Fallas)
                .Include(a => a.DetalleConsumos)
                .ThenInclude(t => t.Consumible)
                .Include(a => a.DetalleConsumos)
                .ThenInclude(t => t.Unidad)
                .Include(a => a.DetalleConsumos)
                .ThenInclude(t => t.Consumo)
                .FirstOrDefaultAsync(o => o.IdArticulo == id);
            if (equipment == null)
            {
                return NotFound();
            }

            var response = new EquipmentResponse
            {
                IdArticulo = equipment.IdArticulo,
                Descripcion = equipment.Descripcion,
                Codigo = equipment.Codigo,
                FechaUltimaLectura = equipment.FechaUltimaLectura ?? DateTime.Today,
                UltimaLectura = equipment.UltimaLectura ?? 0,
                IdUnidadLecturaMantenimiento = equipment.IdUnidadLecturaMantenimiento ?? 0,
                //IdObraActual = equipment.IdObraActual ?? 0,
                ImageUrl = equipment.ImageFullPath,
                DetallePartesDiarios = equipment.DetallePartesDiarios.Select(h => new DetalleParteDiarioResponse
                {
                    IdDetalleParteDiario = h.IdDetalleParteDiario,
                    IdEquipo = h.IdEquipo,
                    FechaLectura = h.FechaLectura, // ?? DateTime.Today,
                    Lectura = h.Lectura,
                    IdUnidad = h.IdUnidad,
                    IdTipoHoraNoProductiva = h.IdTipoHoraNoProductiva ?? 0,
                    HorasProductivas = h.HorasProductivas ?? 0,
                    HorasNoProductivas = h.HorasNoProductivas ?? 0,
                    Unidad = h.Unidad.Descripcion,
                    UnidadAb = h.Unidad.Abreviatura,
                    TipoHoraNoProductiva = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Descripcion : ""),
                    TipoHoraNoProductivaAb = (h.TipoHoraNoProductiva != null ? h.TipoHoraNoProductiva.Abreviatura : "")
                }).ToList(),
                DetalleConsumos = equipment.DetalleConsumos?.Select(h => new DetalleConsumoResponse
                {
                    IdDetalleConsumo = h.IdDetalleConsumo,
                    IdConsumo = h.IdConsumo,
                    IdArticulo = h.IdArticulo,
                    IdConsumible = h.IdConsumible,
                    Cantidad = h.Cantidad,
                    IdUnidadConsumible = h.IdUnidadConsumible,
                    Costo = h.Costo,
                    Observaciones = h.Observaciones,
                    Equipo = (h.Articulo != null ? h.Articulo.Descripcion : ""),
                    Consumible = (h.Consumible != null ? h.Consumible.Descripcion : ""),
                    Unidad = (h.Unidad != null ? h.Unidad.Descripcion : ""),
                    UnidadAb = (h.Unidad != null ? h.Unidad.Abreviatura : ""),
                    FechaConsumo = (h.Consumo != null ? h.Consumo.FechaConsumo : DateTime.Today),
                    Consumo = (h.Consumo != null ? h.Consumo.NumeroConsumo.ToString() : "")
                }).Where(dp => dp.FechaConsumo > DateTime.Today.AddMonths(-6)).OrderByDescending(dp => dp.FechaConsumo).ToList(),
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("PutEquipment2")]
        public async Task<IActionResult> PutEquipment2([FromBody] EquipmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            var oldEquipment = await _dataContext.Articulos.FindAsync(request.IdArticulo);
            if (oldEquipment == null)
            {
                return BadRequest("Equipment doesn't exists.");
            }

            //var petType = await _dataContext.PetTypes.FindAsync(request.PetTypeId);
            //if (petType == null)
            //{
            //    return BadRequest("Not valid pet type.");
            //}

            var imageUrl = oldEquipment.ImageUrl;
            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(request.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Equipos";
                var fullPath = $"~/images/Equipos/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            oldEquipment.Descripcion = request.Descripcion;
            oldEquipment.Codigo = request.Codigo;
            oldEquipment.FechaUltimaLectura = request.FechaUltimaLectura;
            oldEquipment.UltimaLectura = request.UltimaLectura;
            oldEquipment.IdUnidadLecturaMantenimiento = request.IdUnidadLecturaMantenimiento;
            //oldEquipment.IdObraActual = request.IdObraActual;
            oldEquipment.ImageUrl = imageUrl;

            _dataContext.Articulos.Update(oldEquipment);
            await _dataContext.SaveChangesAsync();
            return Ok(_converterHelper.ToEquipmentResponse(oldEquipment));
        }

    }
}