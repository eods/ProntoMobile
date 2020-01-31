using Prism.Navigation;

namespace ProntoMobile.Prism.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public MapPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Map";
        }
    }
}
