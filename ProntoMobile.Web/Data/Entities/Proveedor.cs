using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }

        public string RazonSocial { get; set; }
    }
}
