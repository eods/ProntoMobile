using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly DataContextMANT _dataContextMANT;
        private readonly DataContextERP _dataContextERP;

        public UsuariosController(
            DataContext dataContext,
            DataContextMANT dataContextMANT,
            DataContextERP dataContextERP)
        {
            _dataContext = dataContext;
            _dataContextMANT = dataContextMANT;
            _dataContextERP = dataContextERP;
        }

        [HttpPost]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(EmailRequest emailRequest)
        {
            var user = await _dataContext.Usuarios
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

            var response = new UserResponse
            {
                Id = user.Id,
                FirstName = user.User.FirstName,
                LastName = user.User.LastName,
                Address = user.User.Address,
                Document = user.User.Document,
                Email = user.User.Email,
                PhoneNumber = user.User.PhoneNumber,
                DetalleUserBDs = user.DetalleUserBDs.Select(p => new DetalleUserBDResponse
                {
                    IdDetalleUserBD = p.IdDetalleUserBD,
                    UserId = p.UserId,
                    IdBD = p.IdBD,
                    Base = p.Base.Descripcion
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("GetEmpleadoByUserEmail")]
        public async Task<IActionResult> GetEmpleadoByUserEmail(EmailRequest emailRequest)
        {
            var database = _dataContext.Bases.Where(a => a.Descripcion.ToLower().Equals(emailRequest.DbName.ToLower())).FirstOrDefault();

            Empleado empleado = null;
            if (database.Sistema.ToUpper() == "MANTENIMIENTO")
            {
                empleado = _dataContextMANT.Empleados.Where(a => a.Email.ToLower().Equals(emailRequest.Email.ToLower())).FirstOrDefault();
            }
            if (database.Sistema.ToUpper() == "ERP")
            {
                empleado = _dataContextERP.Empleados.Where(a => a.Email.ToLower().Equals(emailRequest.Email.ToLower())).FirstOrDefault();
            }

            if (empleado == null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Usuario inexistente"
                });
            }

            var response = new EmpleadoResponse
            {
                IdEmpleado = empleado.IdEmpleado,
                Nombre = empleado.Nombre,
                UsuarioNT = empleado.UsuarioNT
            };

            return Ok(response);
        }

    }
}
