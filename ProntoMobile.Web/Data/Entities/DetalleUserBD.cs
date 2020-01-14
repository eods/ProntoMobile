using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class DetalleUserBD
    {
        [Key]
        public int IdDetalleUserBD { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int UserId { get; set; }

        public Usuario Usuario { get; set; }

        [Required]
        [ForeignKey("Base")]
        public int IdBD { get; set; }

        public Base Base { get; set; }

    }
}
