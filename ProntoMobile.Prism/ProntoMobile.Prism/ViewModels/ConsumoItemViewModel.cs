using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Prism.ViewModels
{
    public class ConsumoItemViewModel : DetalleConsumoResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectConsumoCommand;

        public ConsumoItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectConsumoCommand => _selectConsumoCommand ?? (_selectConsumoCommand = new DelegateCommand(SelectConsumo));

        private async void SelectConsumo()
        {
            var parameters = new NavigationParameters
            {
                { "consumo", this }
            };

            await _navigationService.NavigateAsync("ConsumoPage", parameters);
        }
    }
}
