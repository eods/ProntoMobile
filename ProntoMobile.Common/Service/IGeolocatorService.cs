using System.Threading.Tasks;

namespace ProntoMobile.Common.Service
{
    public interface IGeolocatorService
    {
        double Latitude { get; set; }

        double Longitude { get; set; }

        Task GetLocationAsync();
    }
}
