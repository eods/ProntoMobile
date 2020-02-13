using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [ApiController]
    public class ConsumosController : ControllerBase
    {
        private readonly DataContext _datacontextbase;
        private readonly DataContextMANT _datacontext;
        private readonly IConverterHelper _converterHelper;

        public ConsumosController(
            DataContextMANT context,
            DataContext datacontextbase,
            IConverterHelper converterHelper)
        {
            _datacontextbase = datacontextbase;
            _datacontext = context;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        [Route("GetConsumibles")]
        public async Task<IActionResult> GetConsumibles(DbNameRequest dbnameRequest)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(dbnameRequest.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            var equipments = await _datacontext.Articulos
                                .Where(a => (a.EsConsumible ?? "") == "SI" && (a.Activo ?? "SI") == "SI")
                                .OrderBy(a => a.Descripcion).ToListAsync();

            var response = new List<ConsumiblesResponse>();
            foreach (var equipment in equipments)
            {
                var consumibleRespose = new ConsumiblesResponse
                {
                    IdArticulo = equipment.IdArticulo,
                    Descripcion = equipment.Descripcion,
                    Codigo = equipment.Codigo,
                };

                response.Add(consumibleRespose);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PostParte([FromBody] DetalleConsumoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int IdConsumo = request.IdConsumo;

            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            if (request.IdDetalleConsumo > 0)
            {
                var oldConsumo = await _datacontext.DetalleConsumos.FindAsync(request.IdDetalleConsumo);
                if (oldConsumo == null)
                {
                    return BadRequest("Consumo inexistente");
                }

                oldConsumo.IdConsumo = request.IdConsumo;
                oldConsumo.IdArticulo = request.IdArticulo;
                oldConsumo.IdConsumible = request.IdConsumible;
                oldConsumo.Cantidad = request.Cantidad;
                oldConsumo.Observaciones = request.Observaciones;
                oldConsumo.IdUnidadConsumible = request.IdUnidadConsumible;
                oldConsumo.Costo = request.Costo ?? 0;

                _datacontext.DetalleConsumos.Update(oldConsumo);
                await _datacontext.SaveChangesAsync();
                return Ok(_converterHelper.ToConsumoResponse(oldConsumo));
            }
            else
            {
                int NumeroConsumo = 0;

                SqlConnection cnn = new SqlConnection(database.StringConection);
                cnn.Open();

                SqlCommand command = new SqlCommand("Select * from Parametros where IdParametro=1", cnn);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    NumeroConsumo = Int32.Parse(reader["ProximoConsumo"].ToString());
                }
                reader.Close();

                var consumo_c_new = new Consumo
                {
                    NumeroConsumo = NumeroConsumo,
                    FechaConsumo = request.FechaConsumo,
                    IdObra = 0,
                    IdResponsable = request.IdResponsable
                };

                _datacontext.Consumos.Add(consumo_c_new);
                IdConsumo = consumo_c_new.IdConsumo;

                var consumo = new DetalleConsumo
                {
                    IdConsumo = IdConsumo,
                    IdArticulo = request.IdArticulo,
                    IdConsumible = request.IdConsumible,
                    Cantidad = request.Cantidad,
                    Observaciones = request.Observaciones,
                    IdUnidadConsumible = request.IdUnidadConsumible,
                    Costo = request.Costo ?? 0
                };

                _datacontext.DetalleConsumos.Add(consumo);
                await _datacontext.SaveChangesAsync();

                return Ok(_converterHelper.ToConsumoResponse(consumo));
            }
        }

        [HttpPost]
        [Route("DeleteConsumo")]
        public async Task<IActionResult> DeleteConsumo([FromBody] IdRequest request)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            var consumo = await _datacontext.DetalleConsumos.FirstOrDefaultAsync(p => p.IdDetalleConsumo == request.Id);
            if (consumo == null)
            {
                return this.NotFound();
            }

            _datacontext.DetalleConsumos.Remove(consumo);
            await _datacontext.SaveChangesAsync();
            return Ok(_converterHelper.ToConsumoResponse(consumo));
        }
    }
}
