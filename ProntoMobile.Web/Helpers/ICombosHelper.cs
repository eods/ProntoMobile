using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ProntoMobile.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboUsuarios();

    }
}
