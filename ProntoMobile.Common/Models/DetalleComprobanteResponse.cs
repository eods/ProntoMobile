using System;

namespace ProntoMobile.Common.Models
{
    public class DetalleComprobanteResponse
    {
        public int IdDetalleComprobante { get; set; }

        public int IdComprobante { get; set; }

        public DateTime FechaComprobante { get; set; }

        public string Detalle { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Importe { get; set; }

        public decimal ImporteTotal => Cantidad * Importe;
    }
}
