using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public User User { get; set; }

        public ICollection<DetalleUserBD> DetalleUserBDs { get; set; }
    }
}
