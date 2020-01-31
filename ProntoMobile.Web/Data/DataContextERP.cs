using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProntoMobile.Web.Data.Entities;

namespace ProntoMobile.Web.Data
{
    public class DataContextERP : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataContextERP(DbContextOptions<DataContextERP> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var _connectionString = _httpContextAccessor.HttpContext.Session.GetString("String_Pronto");
            if (_connectionString != null)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<_TempAutorizacion> _TempAutorizaciones { get; set; }

        public DbSet<AutorizacionPorComprobante> AutorizacionesPorComprobante { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<DetallePedido> DetallePedidos { get; set; }

        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<Articulo> Articulos { get; set; }

        public DbSet<Cuenta> Cuentas { get; set; }

        public DbSet<ComprobanteProveedor> ComprobantesProveedores { get; set; }

        public DbSet<DetalleComprobanteProveedor> DetalleComprobantesProveedores { get; set; }

        public DbSet<Moneda> Monedas { get; set; }
    }
}
