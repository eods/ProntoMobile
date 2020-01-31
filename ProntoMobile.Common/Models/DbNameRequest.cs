using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Common.Models
{
    public class DbNameRequest
    {
        [Required]
        public string DbName { get; set; }
    }
}
