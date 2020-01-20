using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Cuenta
    {
        [Key]
        public int IdCuenta { get; set; }

        public int Codigo { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Descripcion { get; set; }
    }
}
