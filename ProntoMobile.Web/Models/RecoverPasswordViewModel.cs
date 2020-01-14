using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
