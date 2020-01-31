using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Web.Helpers;
using ProntoMobile.Web.Models;

namespace ProntoMobile.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly DataContext _dataContext2;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ManagersController(
            DataContext dataContext,
            DataContext dataContext2,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _dataContext2 = dataContext2;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            //_httpContextAccessor.HttpContext.Session.SetString("String_Mantenimiento", "Server=SQLMVC;Database=ProntoMantenimiento_Marcalba;Persist Security Info=False;User ID=sa; Password=.SistemaPronto.;MultipleActiveResultSets=true");
            //var empleado1 = _dataContext2.Empleados.ToList();

            return View(_dataContext.Managers.Include(m => m.User));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _dataContext.Managers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
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

                var manager = new Manager { User = user };

                _dataContext.Managers.Add(manager);
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(view.Username, "Pronto Mobile Confirmacion de Email", $"<h1>Pronto Mobile Confirmacion de Email</h1>" +
                    $"Para ser habilitado como administrador, " +
                    $"Por favor haga click en el siguiente link:</br></br><a href = \"{tokenLink}\">Confirmar Email</a>");

                return RedirectToAction(nameof(Index));
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
                UserName = view.Username
            };

            var result = await _userHelper.AddUserAsync(user, view.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(view.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Admin");
            return newUser;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _dataContext.Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            var view = new EditUserViewModel
            {
                Address = manager.User.Address,
                Document = manager.User.Document,
                FirstName = manager.User.FirstName,
                Id = manager.Id,
                LastName = manager.User.LastName,
                PhoneNumber = manager.User.PhoneNumber
            };

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var owner = await _dataContext.Usuarios
                    .Include(o => o.User)
                    .Include(o => o.DetalleUserBDs)
                    .FirstOrDefaultAsync(o => o.Id == view.Id);

                owner.User.Document = view.Document;
                owner.User.FirstName = view.FirstName;
                owner.User.LastName = view.LastName;
                owner.User.Address = view.Address;
                owner.User.PhoneNumber = view.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.User);
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _dataContext.Managers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            _dataContext.Managers.Remove(manager);
            await _dataContext.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(manager.User.Email);
            return RedirectToAction(nameof(Index));
        }

        private bool ManagerExists(int id)
        {
            return _dataContext.Managers.Any(e => e.Id == id);
        }
    }
}
