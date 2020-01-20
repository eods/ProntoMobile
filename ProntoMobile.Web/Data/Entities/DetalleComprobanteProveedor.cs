using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class DetalleComprobanteProveedor
    {
        [Key]
        public int IdDetalleComprobanteProveedor { get; set; }

        [Required]
        [ForeignKey("ComprobanteProveedor")]
        public int IdComprobanteProveedor { get; set; }

        [Required]
        [ForeignKey("Cuenta")]
        public int IdCuenta { get; set; }

        public int? Item { get; set; }

        public decimal Importe { get; set; }

        public ComprobanteProveedor ComprobanteProveedor { get; set; }

        public Cuenta Cuenta { get; set; }
    }
}
