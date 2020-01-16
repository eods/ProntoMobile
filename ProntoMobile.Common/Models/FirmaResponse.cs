using System;
using System.Collections.Generic;

namespace ProntoMobile.Common.Models
{
    public class FirmaResponse
    {
        public int IdTempAutorizacion { get; set; }

        public int IdComprobante { get; set; }

        public string TipoComprobante { get; set; }

        public string TipoComprobanteAb { get; set; }

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

        public int IdMoneda { get; set; }

        public string Moneda { get; set; }

        public decimal? ImporteTotal { get; set; }

        public string Proveedor { get; set; }

        public string Empleado { get; set; }

        public string Email { get; set; }

        public ICollection<DetalleComprobanteResponse> DetallesComprobante { get; set; }
    }
}
