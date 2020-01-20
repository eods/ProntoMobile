using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Data.Entities
{
    public class ComprobanteProveedor
    {
        [Key]
        public int IdComprobanteProveedor { get; set; }

        public int IdTipoComprobante { get; set; }

        [ForeignKey("Proveedor")]
        public int? IdProveedor { get; set; }

        public int? IdProveedorEventual { get; set; }

        public int? IdCuenta { get; set; }

        public DateTime FechaComprobante { get; set; }

        public DateTime FechaRecepcion { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public string Letra { get; set; }

        public int NumeroComprobante1 { get; set; }

        public int NumeroComprobante2 { get; set; }

        public int NumeroReferencia { get; set; }

        [Required]
        [ForeignKey("Moneda")]
        public int IdMoneda { get; set; }

        public decimal CotizacionMoneda { get; set; }

        public decimal TotalComprobante { get; set; }

        public ICollection<DetalleComprobanteProveedor> DetalleComprobantesProveedores { get; set; }
    }
}
