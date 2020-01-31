using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Prism.ViewModels
{
    public class FallasPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private ObservableCollection<FallaItemViewModel> _fallas;
        private bool _isRefreshing;
        private EquipmentResponse _equipment;
        private DelegateCommand _addFallaCommand;
        private DelegateCommand _refreshFallasCommand;
        private static FallasPageViewModel _instance;

        public FallasPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Fallas";
            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            LoadFallas();
        }

        public DelegateCommand AddFallaCommand => _addFallaCommand ?? (_addFallaCommand = new DelegateCommand(AddFalla));

        public DelegateCommand RefreshFallasCommand => _refreshFallasCommand ?? (_refreshFallasCommand = new DelegateCommand(RefreshFallas));

        private async void AddFalla()
        {
            await _navigationService.NavigateAsync("FallaPage");
        }

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public ObservableCollection<FallaItemViewModel> Fallas
        {
            get => _fallas;
            set => SetProperty(ref _fallas, value);
        }

        public static FallasPageViewModel GetInstance()
        {
            return _instance;
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public async Task UpdateFallasAsync()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            if (dbname == null)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "No tiene definida la base de datos", "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var response = await _apiService.GetByIdAsync<EquipmentResponse>(
                url,
                "/api",
                "/Equipments/GetEquipmentById",
                "bearer",
                token.Token,
                dbname.Descripcion,
                _equipment.IdArticulo);

            if (response.IsSuccess)
            {
                var equipment = (EquipmentResponse)response.Result;
                Settings.Equipment = JsonConvert.SerializeObject(equipment);
                _equipment = equipment;
                LoadFallas();
            }
        }

        private async void RefreshFallas()
        {
            IsRefreshing = true;
            await UpdateFallasAsync();
            IsRefreshing = false;
        }

        private void LoadFallas()
        {
            IsRefreshing = true;

            if (Equipment.Fallas != null)
            {
                Fallas = new ObservableCollection<FallaItemViewModel>(Equipment.Fallas.Select(h => new FallaItemViewModel(_navigationService)
                {
                    IdFalla = h.IdFalla,
                    IdArticulo = h.IdArticulo,
                    Descripcion = h.Descripcion,
                    Anulada = h.Anulada,
                    FechaFalla = h.FechaFalla,
                    Observaciones = h.Observaciones,
                    IdOrdenTrabajo = h.IdOrdenTrabajo,
                    NumeroFalla = h.NumeroFalla,
                    FechaAlta = h.FechaAlta,
                    IdObra = h.IdObra,
                    IdReporto = h.IdReporto,
                    Maquinista = h.Maquinista,
                    Articulo = h.Articulo,
                    Reporto = h.Reporto
                }).OrderByDescending(pd => pd.FechaAlta).ToList());
            }

            IsRefreshing = false;
        }

    }
}
