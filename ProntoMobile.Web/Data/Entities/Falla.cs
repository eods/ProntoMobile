using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class Falla
    {
        [Key]
        public int IdFalla { get; set; }

        [Required]
        [ForeignKey("Articulo")]
        public int IdArticulo { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Descripcion { get; set; }

        [MaxLength(2, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Anulada { get; set; }
        
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaFalla { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Observaciones { get; set; }

        public int? IdOrdenTrabajo { get; set; }

        public int? NumeroFalla { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaAlta { get; set; }

        public int? IdObra { get; set; }

        [Required]
        [ForeignKey("Empleado")]
        public int? IdReporto { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Maquinista { get; set; }

        public Articulo Articulo { get; set; }

        public Empleado Empleado { get; set; }

    }
}
