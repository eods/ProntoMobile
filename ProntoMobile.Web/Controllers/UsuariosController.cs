using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Web.Helpers;
using ProntoMobile.Web.Models;

namespace ProntoMobile.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        //private readonly IConverterHelper _converterHelper;
        //private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public UsuariosController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            //IConverterHelper converterHelper,
            //IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            //_converterHelper = converterHelper;
            //_imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            var usuarios = _context.Usuarios
                .Include(o => o.User)
                .Include(o => o.DetalleUserBDs);
                //.ThenInclude(p => p.Base);

            return View(usuarios);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Usuarios
                .Include(o => o.User)
                .Include(o => o.DetalleUserBDs)
                .ThenInclude(p => p.Base)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var user = await AddUser(view);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    return View(view);
                }

                var usuario = new Usuario
                {
                    DetalleUserBDs = new List<DetalleUserBD>(),
                    User = user,
                };

                _context.Usuarios.Add(usuario);

                try
                {
                    await _context.SaveChangesAsync();

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(view.Username, "Pronto Mobile Confirmacion de Email", $"<h1>Pronto Mobile Confirmacion de Email</h1>" +
                        $"Para ser habilitado como usuario, " +
                        $"Por favor haga click en el siguiente link:</br></br><a href = \"{tokenLink}\">Confirmar Email</a>");

                    //_mailHelper.SendMail(view.Username, "Email confirmation",
                    //    $"<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                    //    $"  <tr>" +
                    //    $"    <td style = 'background-color: #34495e; text-align: center; padding: 0'>" +
                    //    $"       <a href = 'https://www.facebook.com/NuskeCIV/' >" +
                    //    $"         <img width = '20%' style = 'display:block; margin: 1.5% 3%' src= 'https://veterinarianuske.com/wp-content/uploads/2016/10/line_separator.png'>" +
                    //    $"       </a>" +
                    //    $"  </td>" +
                    //    $"  </tr>" +
                    //    $"  <tr>" +
                    //    $"  <td style = 'padding: 0'>" +
                    //    $"     <img style = 'padding: 0; display: block' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/logo-nnske-blanck.jpg' width = '100%'>" +
                    //    $"  </td>" +
                    //    $"</tr>" +
                    //    $"<tr>" +
                    //    $" <td style = 'background-color: #ecf0f1'>" +
                    //    $"      <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                    //    $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' > Hola </h1>" +
                    //    $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                    //    $"                      El mejor Hospital Veterinario Especializado de la Ciudad de Morelia enfocado a brindar servicios médicos y quirúrgicos<br>" +
                    //    $"                      aplicando las técnicas más actuales y equipo de vanguardia para diagnósticos precisos y tratamientos oportunos..<br>" +
                    //    $"                      Entre los servicios tenemos:</p>" +
                    //    $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                    //    $"        <li> Urgencias.</li>" +
                    //    $"        <li> Medicina Interna.</li>" +
                    //    $"        <li> Imagenologia.</li>" +
                    //    $"        <li> Pruebas de laboratorio y gabinete.</li>" +
                    //    $"        <li> Estetica canina.</li>" +
                    //    $"      </ul>" +
                    //    $"  <div style = 'width: 100%;margin:20px 0; display: inline-block;text-align: center'>" +
                    //    $"    <img style = 'padding: 0; width: 200px; margin: 5px' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/tarjetas.png'>" +
                    //    $"  </div>" +
                    //    $"  <div style = 'width: 100%; text-align: center'>" +
                    //    $"    <h2 style = 'color: #e67e22; margin: 0 0 7px' >Email Confirmation </h2>" +
                    //    $"    To allow the user,plase click in this link:</ br ></ br > " +
                    //    $"    <a style ='text-decoration: none; border-radius: 5px; padding: 11px 23px; color: white; background-color: #3498db' href = \"{tokenLink}\">Confirm Email</a>" +
                    //    $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0' > Nuskë Clinica Integral Veterinaria 2019 </p>" +
                    //    $"  </div>" +
                    //    $" </td >" +
                    //    $"</tr>" +
                    //    $"</table>");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.ToString());
                    return View(view);
                }
            }

            return View(view);
        }

        private async Task<User> AddUser(AddUserViewModel view)
        {
            var user = new User
            {
                Address = view.Address,
                Document = view.Document,
                Email = view.Username,
                FirstName = view.FirstName,
                LastName = view.LastName,
                PhoneNumber = view.PhoneNumber,
                UserName = view.Username,
                Latitude = view.Latitude,
                Longitude = view.Longitude
            };

            var result = await _userHelper.AddUserAsync(user, view.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(view.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Customer");
            return newUser;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = usuario.User.Address,
                Document = usuario.User.Document,
                FirstName = usuario.User.FirstName,
                Id = usuario.Id,
                LastName = usuario.User.LastName,
                PhoneNumber = usuario.User.PhoneNumber,
                Latitude = usuario.User.Latitude,
                Longitude = usuario.User.Longitude
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .Include(o => o.User)
                    .Include(o => o.DetalleUserBDs)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                usuario.User.Document = model.Document;
                usuario.User.FirstName = model.FirstName;
                usuario.User.LastName = model.LastName;
                usuario.User.Address = model.Address;
                usuario.User.PhoneNumber = model.PhoneNumber;
                usuario.User.Latitude = model.Latitude;
                usuario.User.Longitude = model.Longitude;

                await _userHelper.UpdateUserAsync(usuario.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(o => o.User)
                .Include(o => o.DetalleUserBDs)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.DetalleUserBDs.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "El usuario no puede ser eliminado porque tiene registros asociados");
                return RedirectToAction($"{nameof(Index)}");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(usuario.User.Email);
            return RedirectToAction($"{nameof(Index)}");
        }

        private bool OwnerExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddBD(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id.Value);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new DetalleUserBDViewModel
            {
                UserId = usuario.Id,
                Bases = _combosHelper.GetComboBases()

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBD(DetalleUserBDViewModel model)
        {
            if (ModelState.IsValid)
            {
                var det = new DetalleUserBD
                {
                    UserId = model.UserId,
                    IdBD = model.IdBD
                };

                _context.DetalleUserBDs.Add(det);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{model.UserId}");
            }

            model.Bases = _combosHelper.GetComboBases();

            return View(model);
        }

        //public async Task<IActionResult> EditPet(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pet = await _context.Pets
        //        .Include(p => p.Owner)
        //        .Include(p => p.PetType)
        //        .FirstOrDefaultAsync(p => p.Id == id);
        //    if (pet == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(_converterHelper.ToPetViewModel(pet));
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditPet(PetViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var path = model.ImageUrl;

        //        if (model.ImageFile != null)
        //        {
        //            path = await _imageHelper.UploadImageAsync(model.ImageFile);
        //        }

        //        var pet = await _converterHelper.ToPetAsync(model, path, false);
        //        _context.Pets.Update(pet);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction($"Details/{model.OwnerId}");
        //    }

        //    model.PetTypes = _combosHelper.GetComboPetTypes();

        //    return View(model);
        //}

        //public async Task<IActionResult> DetailsPet(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pet = await _context.Pets
        //        .Include(p => p.Owner)
        //        .ThenInclude(o => o.User)
        //        .Include(p => p.Histories)
        //        .ThenInclude(h => h.ServiceType)
        //        .FirstOrDefaultAsync(o => o.Id == id.Value);
        //    if (pet == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(pet);
        //}

        public async Task<IActionResult> DeleteBD(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var det = await _context.DetalleUserBDs
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.IdDetalleUserBD == id.Value);

            if (det == null)
            {
                return NotFound();
            }

            _context.DetalleUserBDs.Remove(det);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{det.Usuario.Id}");
        }

    }
}
