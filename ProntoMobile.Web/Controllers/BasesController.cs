using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ProntoMobile.Web.Data;
using System.Collections.Generic;
using ProntoMobile.Web.Data.Entities;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Web.Controllers
{
    public class BasesController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public BasesController(
            DataContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: ServiceTypes
        public async Task<IActionResult> Index()
        {
            //var user = await _context.Usuarios
            //    .Include(o => o.User)
            //    .Include(o => o.DetalleUserBDs)
            //    .ThenInclude(p => p.Base)
            //    .FirstOrDefaultAsync(o => o.User.UserName.ToLower().Equals("pronto@yopmail.com"));

            //var response = new UserResponse
            //{
            //    Id = user.Id,
            //    FirstName = user.User.FirstName,
            //    LastName = user.User.LastName,
            //    Address = user.User.Address,
            //    Document = user.User.Document,
            //    Email = user.User.Email,
            //    PhoneNumber = user.User.PhoneNumber,
            //    DetalleUserBDs = user.DetalleUserBDs.Select(p => new DetalleUserBDResponse
            //    {
            //        IdDetalleUserBD = p.IdDetalleUserBD,
            //        UserId = p.UserId,
            //        IdBD = p.IdBD,
            //        Base = p.Base.Descripcion
            //    }).ToList()
            //};


            return View(await _context.Bases.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Base base1)
        {
            if (ModelState.IsValid)
            {
                _context.Bases.Add(base1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(base1);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.Bases.FindAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Base base1)
        {
            if (id != base1.IdBD)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(base1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaseExists(base1.IdBD))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(base1);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var base1 = await _context.Bases
                .FirstOrDefaultAsync(st => st.IdBD == id);

            if (base1 == null)
            {
                return NotFound();
            }

            var det = await _context.DetalleUserBDs.Where(o => o.IdBD == id.Value).ToListAsync();

            if (det.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "La base no puede ser eliminada porque tiene registros asociados");
                return RedirectToAction($"{nameof(Index)}");
            }

            _context.Bases.Remove(base1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaseExists(int id)
        {
            return _context.Bases.Any(e => e.IdBD == id);
        }
    }
}
