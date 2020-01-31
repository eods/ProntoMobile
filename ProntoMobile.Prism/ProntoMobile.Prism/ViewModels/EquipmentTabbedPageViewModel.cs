using Newtonsoft.Json;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Prism.ViewModels
{
    public class EquipmentTabbedPageViewModel : ViewModelBase
    {
        public EquipmentTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            var equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            Title = $"{equipment.Descripcion}";
        }
    }
}
