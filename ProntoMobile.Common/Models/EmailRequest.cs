using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Common.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string DbName { get; set; }
    }
}
