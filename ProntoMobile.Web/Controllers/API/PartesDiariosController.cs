using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PartesDiariosController : Controller
    {
        private readonly DataContextMANT _dataContext;
        private readonly IConverterHelper _converterHelper;

        public PartesDiariosController(
            DataContextMANT dataContext,
            IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        [Route("GetUltimoParteDiario")]
        public async Task<IActionResult> GetUltimoParteDiario([FromBody] DetalleParteDiarioRequest request)
        {
            var ParteDiario = await _dataContext.DetallePartesDiarios
                .Where(pd => pd.IdEquipo == request.IdEquipo)
                .OrderByDescending(pd => pd.FechaLectura)
                .FirstOrDefaultAsync();

            var response = new DetalleParteDiarioResponse
            {
                IdDetalleParteDiario = ParteDiario.IdDetalleParteDiario,
                IdParteDiario = ParteDiario.IdParteDiario,
                IdEquipo = ParteDiario.IdEquipo,
                FechaLectura = ParteDiario.FechaLectura,
                Lectura = ParteDiario.Lectura,
                IdUnidad = ParteDiario.IdUnidad,
                IdTipoHoraNoProductiva = ParteDiario.IdTipoHoraNoProductiva,
                HorasProductivas = ParteDiario.HorasProductivas,
                HorasNoProductivas = ParteDiario.HorasNoProductivas
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PostParte([FromBody] DetalleParteDiarioRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var unidad = await _dataContext.Unidades.FindAsync(request.IdUnidad);
            if (unidad == null)
            {
                return BadRequest("Not valid unit.");
            }

            var detalleParteDiario = new DetalleParteDiario
            {
                IdParteDiario = request.IdParteDiario,
                IdEquipo = request.IdEquipo,
                FechaLectura = request.FechaLectura,
                Lectura = request.Lectura,
                IdUnidad = request.IdUnidad,
                IdTipoHoraNoProductiva = request.IdTipoHoraNoProductiva,
                HorasProductivas = request.HorasProductivas,
                HorasNoProductivas = request.HorasNoProductivas
            };

            _dataContext.DetallePartesDiarios.Add(detalleParteDiario);
            await _dataContext.SaveChangesAsync();
            return Ok(_converterHelper.ToDetalleParteDiarioResponse(detalleParteDiario));
        }

        //[HttpPost]
        //[Route("PutParte2")]
        //public async Task<IActionResult> PutParte2([FromBody] PetRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var oldPet = await _dataContext.Pets.FindAsync(request.Id);
        //    if (oldPet == null)
        //    {
        //        return BadRequest("Pet doesn't exists.");
        //    }

        //    var petType = await _dataContext.PetTypes.FindAsync(request.PetTypeId);
        //    if (petType == null)
        //    {
        //        return BadRequest("Not valid pet type.");
        //    }

        //    oldPet.Born = request.Born.ToUniversalTime();
        //    oldPet.Name = request.Name;
        //    oldPet.PetType = petType;
        //    oldPet.Race = request.Race;
        //    oldPet.Remarks = request.Remarks;

        //    _dataContext.Pets.Update(oldPet);
        //    await _dataContext.SaveChangesAsync();
        //    return Ok(_converterHelper.ToPetResponse(oldPet));
        //}
    }
}
