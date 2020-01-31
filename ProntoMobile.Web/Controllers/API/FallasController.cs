using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Web.Helpers;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FallasController : ControllerBase
    {
        private readonly DataContext _datacontextbase;
        private readonly DataContextMANT _dataContext;
        private readonly IConverterHelper _converterHelper;

        public FallasController(
            DataContext datacontextbase,
            DataContextMANT dataContext,
            IConverterHelper converterHelper)
        {
            _datacontextbase = datacontextbase;
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostParte([FromBody] FallaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.IdFalla > 0)
            {
                var oldFalla = await _dataContext.Fallas.FindAsync(request.IdFalla);
                if (oldFalla == null)
                {
                    return BadRequest("Falla inexistente");
                }

                oldFalla.IdArticulo = request.IdArticulo;
                oldFalla.Descripcion = request.Descripcion;
                oldFalla.Anulada = request.Anulada;
                oldFalla.FechaFalla = request.FechaFalla ?? DateTime.Today;
                oldFalla.Observaciones = request.Observaciones;
                oldFalla.NumeroFalla = request.NumeroFalla;
                oldFalla.FechaAlta = request.FechaAlta;
                oldFalla.IdObra = request.IdObra;
                oldFalla.IdReporto = request.IdReporto;
                oldFalla.Maquinista = request.Maquinista;

                _dataContext.Fallas.Update(oldFalla);
                await _dataContext.SaveChangesAsync();
                return Ok(_converterHelper.ToFallaResponse(oldFalla));
            }
            else
            {
                var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
                HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

                int NumeroFalla = 0;

                SqlConnection cnn = new SqlConnection(database.StringConection);
                cnn.Open();

                SqlCommand command = new SqlCommand("Select * from Parametros where IdParametro=1", cnn);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    NumeroFalla = Int32.Parse(reader["ProximoNumeroFalla"].ToString());
                }
                reader.Close();

                var falla = new Falla
                {
                    IdArticulo = request.IdArticulo,
                    Descripcion = request.Descripcion,
                    Anulada = request.Anulada,
                    FechaFalla = request.FechaFalla ?? DateTime.Today,
                    Observaciones = request.Observaciones,
                    NumeroFalla = NumeroFalla,
                    FechaAlta = request.FechaAlta,
                    IdObra = request.IdObra,
                    IdReporto = request.IdReporto,
                    Maquinista = request.Maquinista
                };

                _dataContext.Fallas.Add(falla);
                await _dataContext.SaveChangesAsync();

                SqlCommand cmd = new SqlCommand("Update Parametros Set ProximoNumeroFalla=IsNull(ProximoNumeroFalla,0) + 1 Where IdParametro=1", cnn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cnn.Close();

                return Ok(_converterHelper.ToFallaResponse(falla));
            }
        }
    }
}