using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class _TempAutorizacion
    {
        [Key]
        public int IdTempAutorizacion { get; set; }

        public int IdComprobante { get; set; }

        public string TipoComprobante { get; set; }

        public string Numero { get; set; }

        public int IdAutorizacion { get; set; }

        public int IdFormulario { get; set; }

        public int IdDetalleAutorizacion { get; set; }

        public string SectorEmisor { get; set; }

        public int OrdenAutorizacion { get; set; }

        [Required]
        [ForeignKey("Empleado")]
        public int IdAutoriza { get; set; }

        public int? IdSector { get; set; }

        public int? IdLibero { get; set; }

        public DateTime Fecha { get; set; }

        public Empleado Empleado { get; set; }

    }
}
