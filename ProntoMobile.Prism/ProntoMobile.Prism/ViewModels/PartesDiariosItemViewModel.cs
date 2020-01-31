using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Prism.ViewModels
{
    public class PartesDiariosItemViewModel : DetalleParteDiarioResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectParteDiarioCommand;

        public PartesDiariosItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectParteDiarioCommand => _selectParteDiarioCommand ?? (_selectParteDiarioCommand = new DelegateCommand(SelectParteDiario));

        private async void SelectParteDiario()
        {
            var parameters = new NavigationParameters
            {
                { "partediario", this }
            };

            await _navigationService.NavigateAsync("ParteDiarioPage", parameters);
        }
    }
}
