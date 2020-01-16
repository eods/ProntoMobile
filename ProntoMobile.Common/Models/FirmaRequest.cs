using System;

namespace ProntoMobile.Common.Models
{
    public class FirmaRequest
    {
        public int IdTempAutorizacion { get; set; }

        public int IdComprobante { get; set; }

        public string TipoComprobante { get; set; }

        public string Numero { get; set; }

        public int IdAutorizacion { get; set; }

        public int IdFormulario { get; set; }

        public int IdDetalleAutorizacion { get; set; }

        public string SectorEmisor { get; set; }

        public int OrdenAutorizacion { get; set; }

        public int IdAutoriza { get; set; }

        public int IdSector { get; set; }

        public int IdLibero { get; set; }

        public DateTime Fecha { get; set; }
    }
}
