using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class Articulo
    {

        [Key]
        public int IdArticulo { get; set; }

        [MaxLength(255, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Descripcion { get; set; }

        [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Codigo { get; set; }

        public decimal? UltimaLectura { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaUltimaLectura { get; set; }

        public int? IdUnidadLecturaMantenimiento { get; set; }

        public string ParaMantenimiento { get; set; }

        public string EsConsumible { get; set; }

        public string Activo { get; set; }

        public int? IdObraActual { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        //TODO: replace the correct URL for the image
        public string ImageFullPath => string.IsNullOrEmpty(ImageUrl)
            ? null
            : $"http://prontomobile.bdlconsultores.com.ar{ImageUrl.Substring(1)}";

        public ICollection<DetalleParteDiario> DetallePartesDiarios { get; set; }

        public ICollection<Falla> Fallas { get; set; }

        public ICollection<DetalleConsumo> DetalleConsumos { get; set; }
    }
}
