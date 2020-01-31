using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Prism.ViewModels
{
    public class FallaPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EquipmentResponse _equipment;
        private FallaResponse _falla;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;
        private DelegateCommand _saveCommand;

        public FallaPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public FallaResponse Falla
        {
            get => _falla;
            set => SetProperty(ref _falla, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            var User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);

            if (parameters.ContainsKey("falla"))
            {
                Falla = parameters.GetValue<FallaResponse>("falla");
                IsEdit = true;
                Title = "Edicion Falla";
            }
            else
            {
                Falla = new FallaResponse
                {
                    FechaFalla = DateTime.Today,
                    FechaAlta = DateTime.Today,
                    NumeroFalla = 0,
                    IdArticulo = Equipment.IdArticulo,
                    //IdReporto = User.Id,
                    Maquinista = User.FullName
                };
                IsEdit = false;
                Title = "Nueva Falla";
            }
        }

        private async void SaveAsync()
        {
            var isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            var response2 = await _apiService.GetByEmail2Async<EmpleadoResponse>(url, "/api", "/Usuarios/GetEmpleadoByUserEmail", "bearer", token.Token, dbname.Descripcion, user.Email);
            if (!response2.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response2.Message, "Accept");
                return;
            }
            var empleado = (EmpleadoResponse)response2.Result;

            var fallaRequest = new FallaRequest
            {
                IdFalla = Falla.IdFalla,
                IdArticulo = Falla.IdArticulo,
                Descripcion = Falla.Descripcion,
                Anulada = Falla.Anulada,
                FechaFalla = Falla.FechaFalla,
                Observaciones = Falla.Observaciones,
                IdOrdenTrabajo = Falla.IdOrdenTrabajo,
                NumeroFalla = Falla.NumeroFalla,
                FechaAlta = Falla.FechaAlta,
                IdObra = Falla.IdObra,
                IdReporto = empleado.IdEmpleado,
                Maquinista = Falla.Maquinista,
            };
            fallaRequest.DbName = dbname.Descripcion;

            Response<object> response;
            //if (IsEdit)
            //{
            //    response = await _apiService.PutAsync(url, "/api", "/Equipments/PutFalla", fallaRequest.IdFalla, fallaRequest, "bearer", token.Token);
            //}
            //else
            //{
                response = await _apiService.PostAsync<FallaRequest>(url, "/api", "/Fallas", "bearer", token.Token, dbname.Descripcion, fallaRequest);
            //}

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            //await App.Current.MainPage.DisplayAlert(
            //    "Ok",
            //    "Ok",
            //    "Accept");

            await FallasPageViewModel.GetInstance().UpdateFallasAsync();
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            return true;
        }

    }
}
