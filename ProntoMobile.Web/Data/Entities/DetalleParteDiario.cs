using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class DetalleParteDiario
    {
        [Key]
        public int IdDetalleParteDiario { get; set; }

        public int IdParteDiario { get; set; }

        [Required]
        [ForeignKey("Articulo")]
        public int IdEquipo { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaLectura { get; set; }

        [Required]
        public int Lectura { get; set; }

        [Required]
        [ForeignKey("Unidad")]
        public int IdUnidad { get; set; }

        [ForeignKey("TipoHoraNoProductiva")]
        public int? IdTipoHoraNoProductiva { get; set; }

        public decimal? HorasProductivas { get; set; }

        public decimal? HorasNoProductivas { get; set; }

        public Articulo Articulo { get; set; }

        public Unidad Unidad { get; set; }

        public TipoHoraNoProductiva TipoHoraNoProductiva { get; set; }
    }
}
