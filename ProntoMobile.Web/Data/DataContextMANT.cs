using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Web.Data.Entities;

namespace ProntoMobile.Web.Data
{
    public class DataContextMANT : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataContextMANT(DbContextOptions<DataContextMANT> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var _connectionString = _httpContextAccessor.HttpContext.Session.GetString("String_Mantenimiento");
            if (_connectionString != null)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public DbSet<Articulo> Articulos { get; set; }

        public DbSet<Unidad> Unidades { get; set; }

        public DbSet<DetalleParteDiario> DetallePartesDiarios { get; set; }

        public DbSet<TipoHoraNoProductiva> TiposHorasNoProductivas { get; set; }

        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<Falla> Fallas { get; set; }

        public DbSet<Obra> Obras { get; set; }

        public DbSet<Consumo> Consumos { get; set; }

        public DbSet<DetalleConsumo> DetalleConsumos { get; set; }
    }
}
