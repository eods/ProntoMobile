using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadesController : ControllerBase
    {
        private readonly DataContext _datacontextbase;
        private readonly DataContextMANT _datacontext;

        public UnidadesController(
            DataContextMANT context,
            DataContext datacontextbase)
        {
            _datacontextbase = datacontextbase;
            _datacontext = context;
        }

        [HttpPost]
        [Route("GetUnidades")]
        public IEnumerable<Unidad> GetUnidades([FromBody] DbNameRequest request)
        {
            var database = _datacontextbase.Bases.Where(a => a.Descripcion.ToLower().Equals(request.DbName.ToLower())).FirstOrDefault();
            HttpContext.Session.SetString("String_Mantenimiento", database.StringConection);

            return _datacontext.Unidades.OrderBy(u => u.Descripcion);
        }

    }
}
