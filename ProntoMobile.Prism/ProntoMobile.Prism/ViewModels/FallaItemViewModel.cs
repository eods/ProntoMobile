using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Prism.ViewModels
{
    public class FallaItemViewModel : FallaResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectFallaCommand;

        public FallaItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectFallaCommand => _selectFallaCommand ?? (_selectFallaCommand = new DelegateCommand(SelectFalla));

        private async void SelectFalla()
        {
            var parameters = new NavigationParameters
            {
                { "falla", this }
            };

            await _navigationService.NavigateAsync("FallaPage", parameters);
        }
    }
}
