using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BasesController : ControllerBase
    {
        private readonly DataContext _context;
        public BasesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Base> GetBases()
        {
            return _context.Bases.OrderBy(u => u.Descripcion);
        }

        public class BaseEmail
        {
            public int IdBD { get; set; }
            public string Descripcion { get; set; }
            public string Abreviatura { get; set; }
            public string StringConection { get; set; }
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("GetBasesPorEmail")]
        public async Task<IActionResult> GetBasesPorEmail(EmailRequest emailRequest)
        {
            var user = await _context.Usuarios
                .Include(o => o.User)
                .Include(o => o.DetalleUserBDs)
                .ThenInclude(p => p.Base)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower().Equals(emailRequest.Email.ToLower()));

            if (user == null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Usuario inexistente"
                });
            }


            var response = new List<BaseResponse>();
            foreach (var base1 in user.DetalleUserBDs)
            {
                var baseRespose = new BaseResponse
                {
                    IdBD = base1.IdBD,
                    Descripcion = base1.Base.Descripcion,
                    Abreviatura = base1.Base.Abreviatura,
                    StringConection = base1.Base.StringConection
                };

                response.Add(baseRespose);
            }

            return Ok(response);
        }
    }
}