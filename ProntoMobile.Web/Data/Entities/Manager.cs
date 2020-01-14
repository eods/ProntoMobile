using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Manager
    {
        [Key]
        public int Id { get; set; }

        public User User { get; set; }
    }
}
