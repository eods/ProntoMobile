using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
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

        public UsuariosController(DataContext dataContext)
        {
            _dataContext = dataContext;
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
    }
}
