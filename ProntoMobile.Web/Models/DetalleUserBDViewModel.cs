using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Models
{
    public class DetalleUserBDViewModel
    {
        public int IdDetalleUserBD { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Base de datos")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a base de datos.")]
        public int IdBD { get; set; }

        public IEnumerable<SelectListItem> Bases { get; set; }
    }
}
