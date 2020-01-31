using Prism.Commands;
using Prism.Navigation;

namespace ProntoMobile.Prism.ViewModels
{
    public class PrincipalPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private DelegateCommand _basesDatosCommand;
        private DelegateCommand _partesDiariosCommand;
        private DelegateCommand _firmasCommand;

        public PrincipalPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Modulos habilitados";

            //var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
        }

        public DelegateCommand BasesDatosCommand => _basesDatosCommand ?? (_basesDatosCommand = new DelegateCommand(BasesDatos));

        public DelegateCommand PartesDiariosCommand => _partesDiariosCommand ?? (_partesDiariosCommand = new DelegateCommand(PartesDiarios));

        public DelegateCommand FirmasCommand => _firmasCommand ?? (_firmasCommand = new DelegateCommand(Firmas));

        private async void BasesDatos()
        {
            await _navigationService.NavigateAsync("BasesDatosPage");
        }

        private async void PartesDiarios()
        {
            await _navigationService.NavigateAsync("EquipmentsPage");
        }

        private async void Firmas()
        {
            await _navigationService.NavigateAsync("FirmasPage");
        }
    }
}
