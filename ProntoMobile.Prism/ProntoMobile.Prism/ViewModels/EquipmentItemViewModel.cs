using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Prism.ViewModels
{
    public class EquipmentItemViewModel : EquipmentResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectEquipmentCommand;

        public EquipmentItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectEquipmentCommand => _selectEquipmentCommand ?? (_selectEquipmentCommand = new DelegateCommand(SelectEquipment));

        private async void SelectEquipment()
        {
            Settings.Equipment = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("EquipmentTabbedPage");
        }
    }
}
