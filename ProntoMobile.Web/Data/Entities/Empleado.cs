using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        public string Nombre { get; set; }

        public string UsuarioNT { get; set; }

        public string Email { get; set; }

        public int? IdObraAsignada { get; set; }
    }
}
