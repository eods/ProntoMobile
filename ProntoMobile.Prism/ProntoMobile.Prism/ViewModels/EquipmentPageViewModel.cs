using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;

namespace ProntoMobile.Prism.ViewModels
{
    public class EquipmentPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private EquipmentResponse _equipment;
        private DelegateCommand _editEquipmentCommand;
        //private ObservableCollection<HistoryItemViewModel> _histories;

        public EquipmentPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Detalles";
        }

        public DelegateCommand EditEquipmentCommand => _editEquipmentCommand ?? (_editEquipmentCommand = new DelegateCommand(EditEquipmentAsync));

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        //public ObservableCollection<HistoryItemViewModel> Histories
        //{
        //    get => _histories;
        //    set => SetProperty(ref _histories, value);
        //}

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            //Histories = new ObservableCollection<HistoryItemViewModel>(Pet.Histories.Select(h => new HistoryItemViewModel(_navigationService)
            //{
            //    Date = h.Date,
            //    Description = h.Description,
            //    Id = h.Id,
            //    Remarks = h.Remarks,
            //    ServiceType = h.ServiceType
            //}).ToList());
        }

        private async void EditEquipmentAsync()
        {
            var parameters = new NavigationParameters
            {
                { "equipment", Equipment }
            };

            await _navigationService.NavigateAsync("EditEquipmentPage", parameters);
        }
    }
}
