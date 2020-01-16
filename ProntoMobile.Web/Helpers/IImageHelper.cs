using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ProntoMobile.Web.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
    }
}