using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProntoMobile.Prism.ViewModels
{
    public class FirmaItemViewModel : FirmaResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectFirmaCommand;

        public FirmaItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectFirmaCommand => _selectFirmaCommand ?? (_selectFirmaCommand = new DelegateCommand(SelectFirma));

        private async void SelectFirma()
        {
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);

            var parameters = new NavigationParameters
            {
                { "firma", this }
            };

            await _navigationService.NavigateAsync("FirmaPage", parameters);
        }

    }
}
