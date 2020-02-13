using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ProntoMobile.Prism.ViewModels
{
    public class ConsumoPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EquipmentResponse _equipment;
        private DetalleConsumoResponse _consumo;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;
        private int _idUnidad;
        private string _unidad2;
        private ObservableCollection<ConsumiblesResponse> _consumibles;
        private ConsumiblesResponse _consumible;
        private ObservableCollection<UnidadResponse> _unidades;
        private UnidadResponse _unidad;
        private DelegateCommand _saveCommand;
        private DelegateCommand _deleteCommand;

        public ConsumoPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;

            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            //Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            //var a = Thread.CurrentThread.CurrentCulture;
            //var b = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public DelegateCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand(DeleteAsync));

        public ObservableCollection<ConsumiblesResponse> Consumibles
        {
            get => _consumibles;
            set => SetProperty(ref _consumibles, value);
        }

        public ConsumiblesResponse Consumible
        {
            get => _consumible;
            set => SetProperty(ref _consumible, value);
        }

        public ObservableCollection<UnidadResponse> Unidades
        {
            get => _unidades;
            set => SetProperty(ref _unidades, value);
        }

        public UnidadResponse Unidad
        {
            get => _unidad;
            set => SetProperty(ref _unidad, value);
        }

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

        public DetalleConsumoResponse Consumo
        {
            get => _consumo;
            set => SetProperty(ref _consumo, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);

            if (parameters.ContainsKey("consumo"))
            {
                Consumo = parameters.GetValue<DetalleConsumoResponse>("consumo");
                IsEdit = true;
                Title = "Consumos";
            }
            else
            {
                Consumo = new DetalleConsumoResponse
                {
                    FechaConsumo = DateTime.Today,
                    IdArticulo = Equipment.IdArticulo
                };
                IsEdit = false;
                Title = "Nuevo Consumo";
            }

            LoadCombos();
        }

        private async void LoadCombos()
        {
            IsEnabled = false;
            IsRunning = true;

            var url = App.Current.Resources["UrlAPI"].ToString();

            var connection = await _apiService.CheckConnection(url);
            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Check the internet connection.",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            var response = await _apiService.GetListAsync<UnidadResponse>(
                url,
                "/api",
                "/Unidades/GetUnidades",
                "bearer",
                token.Token,
                dbname.Descripcion);

            if (!response.IsSuccess)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the units list, please try later,",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var unidadesResponse = (List<UnidadResponse>)response.Result;
            Unidades = new ObservableCollection<UnidadResponse>(unidadesResponse);

            if (!string.IsNullOrEmpty(Consumo.Unidad))
            {
                Unidad = Unidades.FirstOrDefault(pt => pt.Descripcion == Consumo.Unidad);
            }

            response = await _apiService.GetListAsync<ConsumiblesResponse>(
                url,
                "/api",
                "/Consumos/GetConsumibles",
                "bearer",
                token.Token,
                dbname.Descripcion);

            if (!response.IsSuccess)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se puede acceder a la lista de consumibles, intente mas tarde",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var consumiblesResponse = (List<ConsumiblesResponse>)response.Result;
            Consumibles = new ObservableCollection<ConsumiblesResponse>(consumiblesResponse);

            if (!string.IsNullOrEmpty(Consumo.Consumible))
            {
                Consumible = Consumibles.FirstOrDefault(pt => pt.Descripcion == Consumo.Consumible);
            }

            IsEnabled = true;
            IsRunning = false;
        }

        private async void SaveAsync()
        {
            if (Unidad != null)
            {
                Consumo.IdUnidadConsumible = Unidad.IdUnidad;
            }

            if (Consumible != null)
            {
                Consumo.IdConsumible = Consumible.IdArticulo;
                Consumo.Consumible = Consumible.Descripcion;
            }

            var isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            var response2 = await _apiService.GetByEmail2Async<EmpleadoResponse>(url, "/api", "/Usuarios/GetEmpleadoByUserEmail", "bearer", token.Token, dbname.Descripcion, user.Email);
            if (!response2.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response2.Message, "Accept");
                return;
            }
            var empleado = (EmpleadoResponse)response2.Result;

            string cant = Consumo.Cantidad.ToString().Replace(",", ".");

            var consumoRequest = new DetalleConsumoRequest
            {
                IdDetalleConsumo = Consumo.IdDetalleConsumo,
                IdConsumo = Consumo.IdConsumo,
                IdArticulo = Consumo.IdArticulo,
                IdConsumible = Consumo.IdConsumible,
                Cantidad = decimal.Parse(cant),
                IdUnidadConsumible = Consumo.IdUnidadConsumible,
                Costo = Consumo.Costo,
                Observaciones = Consumo.Observaciones,
                FechaConsumo = Consumo.FechaConsumo,
                IdResponsable = empleado.IdEmpleado
            };
            consumoRequest.DbName = dbname.Descripcion;

            Response<object> response;
            if (IsEdit)
            {
                //response = await _apiService.PutAsync(url, "/api", "/Consumos/PutConsumo", consumoRequest.IdDetalleConsumo, consumoRequest, "bearer", token.Token);
                response = await _apiService.PostAsync<DetalleConsumoRequest>(url, "/api", "/Consumos", "bearer", token.Token, dbname.Descripcion, consumoRequest);
            }
            else
            {
                response = await _apiService.PostAsync(url, "/api", "/Consumos", "bearer", token.Token, dbname.Descripcion, consumoRequest);
            }

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

            await ConsumosPageViewModel.GetInstance().UpdateConsumosAsync();
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (Consumo.FechaConsumo == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Fecha de consumo incorrecta", "Accept");
                return false;
            }

            if (Consumo.Cantidad == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe informar la cantidad", "Accept");
                return false;
            }

            if (Consumo.IdUnidadConsumible == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe indicar la unidad", "Accept");
                return false;
            }

            return true;
        }

        private async void DeleteAsync()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
                "Confirm",
                "Esta seguro de eliminar?",
                "Yes",
                "No");

            if (!answer)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            var request = new IdRequest
            {
                Id = Consumo.IdDetalleConsumo,
                DbName = dbname.Descripcion
            };

            var response = await _apiService.PostAsync(url, "/api", "/Consumos/DeleteConsumo", "bearer", token.Token, dbname.Descripcion, request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            await ConsumosPageViewModel.GetInstance().UpdateConsumosAsync();
            IsRunning = false;
            IsEnabled = true;
            await _navigationService.GoBackAsync();
        }
    }
}
