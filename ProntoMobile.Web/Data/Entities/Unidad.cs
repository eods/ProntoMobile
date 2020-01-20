using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Unidad
    {
        [Key]
        public int IdUnidad { get; set; }

        [MaxLength(30, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Descripcion { get; set; }

        [MaxLength(15, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Abreviatura { get; set; }
    }
}
