using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Moneda
    {
        [Key]
        public int IdMoneda { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Nombre { get; set; }

        [MaxLength(15, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Abreviatura { get; set; }
    }
}
