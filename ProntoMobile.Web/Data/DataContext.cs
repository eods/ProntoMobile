using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Web.Data.Entities;

namespace ProntoMobile.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Base> Bases { get; set; }

        public DbSet<DetalleUserBD> DetalleUserBDs { get; set; }

        public DbSet<Manager> Managers { get; set; }

    }
}
