using System;
using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class AutorizacionPorComprobante
    {
        [Key]
        public int IdAutorizacionPorComprobante { get; set; }

        public int IdFormulario { get; set; }

        public int IdComprobante { get; set; }

        public int OrdenAutorizacion { get; set; }

        public int IdAutorizo { get; set; }

        public DateTime FechaAutorizacion { get; set; }
    }
}
