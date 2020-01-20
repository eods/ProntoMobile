using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class TipoHoraNoProductiva
    {
        [Key]
        public int IdTipoHoraNoProductiva { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Descripcion { get; set; }

        public string Abreviatura { get; set; }
    }
}
