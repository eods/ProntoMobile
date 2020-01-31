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

namespace ProntoMobile.Prism.ViewModels
{
    public class ParteDiarioPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EquipmentResponse _equipment;
        private DetalleParteDiarioResponse _parteDiario;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;
        private DateTime _fechaUltimoParte;
        private int _lectura;
        private int _idUnidad;
        private string _unidad2;
        private ObservableCollection<TipoHoraNoProductivaResponse> _tiposHorasNoProductivas;
        private TipoHoraNoProductivaResponse _tipoHoraNoProductiva;
        private ObservableCollection<UnidadResponse> _unidades;
        private UnidadResponse _unidad;
        private DelegateCommand _saveCommand;
        private DelegateCommand _deleteCommand;

        public ParteDiarioPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public DelegateCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand(DeleteAsync));

        public ObservableCollection<TipoHoraNoProductivaResponse> TiposHorasNoProductivas
        {
            get => _tiposHorasNoProductivas;
            set => SetProperty(ref _tiposHorasNoProductivas, value);
        }

        public TipoHoraNoProductivaResponse TipoHoraNoProductiva
        {
            get => _tipoHoraNoProductiva;
            set => SetProperty(ref _tipoHoraNoProductiva, value);
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

        public DateTime FechaUltimoParte
        {
            get => _fechaUltimoParte;
            set => SetProperty(ref _fechaUltimoParte, value);
        }

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public DetalleParteDiarioResponse ParteDiario
        {
            get => _parteDiario;
            set => SetProperty(ref _parteDiario, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);

            if (parameters.ContainsKey("partediario"))
            {
                ParteDiario = parameters.GetValue<DetalleParteDiarioResponse>("partediario");
                IsEdit = true;
                Title = "Edicion Parte Diario";
            }
            else
            {
                var UltimoParte2 = Equipment.DetallePartesDiarios.OrderByDescending(pd => pd.FechaLectura).FirstOrDefault();
                if (UltimoParte2 != null)
                {
                    FechaUltimoParte = UltimoParte2.FechaLectura.AddDays(1); //DateTime.Today
                    _lectura = UltimoParte2.Lectura;
                    _idUnidad = UltimoParte2.IdUnidad;
                    _unidad2 = UltimoParte2.Unidad;
                }
                else
                {
                    FechaUltimoParte = DateTime.Today;
                    _lectura = 0;
                    _idUnidad = Equipment.IdUnidadLecturaMantenimiento;
                    _unidad2 = Equipment.Unidad;
                }
                ParteDiario = new DetalleParteDiarioResponse
                {
                    FechaLectura = FechaUltimoParte,
                    Lectura = _lectura,
                    IdEquipo = Equipment.IdArticulo,
                    IdUnidad = _idUnidad,
                    Unidad = _unidad2
                };
                IsEdit = false;
                Title = "Nuevo Parte Diario";
            }

            LoadCombos();
        }

        //private async void UltimoParte()
        //{
        //    IsEnabled = false;

        //    var url = App.Current.Resources["UrlAPI"].ToString();

        //    var connection = await _apiService.CheckConnection(url);
        //    if (!connection)
        //    {
        //        IsEnabled = true;
        //        IsRunning = false;
        //        await App.Current.MainPage.DisplayAlert(
        //            "Error",
        //            "Check the internet connection.",
        //            "Accept");
        //        await _navigationService.GoBackAsync();
        //        return;
        //    }

        //    var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

        //    var request = new DetalleParteDiarioResponse
        //    {
        //        IdEquipo = Equipment.IdArticulo
        //    };

        //    var response = await _apiService.PostAsync(
        //        url,
        //        "/api",
        //        "/PartesDiarios/GetUltimoParteDiario",
        //        request,
        //        "bearer",
        //        token.Token);

        //    IsEnabled = true;

        //    if (!response.IsSuccess)
        //    {
        //        await App.Current.MainPage.DisplayAlert(
        //            "Error",
        //            "Getting the las daily part, please try later,",
        //            "Accept");
        //        await _navigationService.GoBackAsync();
        //        return;
        //    }

        //    var partediario = (DetalleParteDiarioResponse)response.Result;
        //    Lectura = 5000;
        //    //ParteDiario.IdEquipo = partediario.IdEquipo;
        //    //ParteDiario.Lectura = partediario.Lectura;
        //    //ParteDiario.IdUnidad = partediario.IdUnidad;
        //}

        private async void LoadCombos()
        {
            IsEnabled = false;

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

            var response = await _apiService.GetListAsync<TipoHoraNoProductivaResponse>(
                url,
                "/api",
                "/TiposHoraNoProductiva/GetTiposHoraNoProductiva",
                "bearer",
                token.Token,
                dbname.Descripcion);

            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the hours non productive list, please try later,",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var tiposHorasNoProductivasResponse = (List<TipoHoraNoProductivaResponse>)response.Result;
            TiposHorasNoProductivas = new ObservableCollection<TipoHoraNoProductivaResponse>(tiposHorasNoProductivasResponse);

            if (!string.IsNullOrEmpty(ParteDiario.TipoHoraNoProductiva))
            {
                TipoHoraNoProductiva = TiposHorasNoProductivas.FirstOrDefault(pt => pt.Descripcion == ParteDiario.TipoHoraNoProductiva);
            }

            response = await _apiService.GetListAsync<UnidadResponse>(
                url,
                "/api",
                "/Unidades/GetUnidades",
                "bearer",
                token.Token,
                dbname.Descripcion);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the units list, please try later,",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var unidadesResponse = (List<UnidadResponse>)response.Result;
            Unidades = new ObservableCollection<UnidadResponse>(unidadesResponse);

            if (!string.IsNullOrEmpty(ParteDiario.Unidad))
            {
                Unidad = Unidades.FirstOrDefault(pt => pt.Descripcion == ParteDiario.Unidad);
            }
        }

        private async void SaveAsync()
        {
            if (TipoHoraNoProductiva != null)
            {
                ParteDiario.IdTipoHoraNoProductiva = TipoHoraNoProductiva.IdTipoHoraNoProductiva;
            }
            if (Unidad != null)
            {
                ParteDiario.IdUnidad = Unidad.IdUnidad;
            }

            var isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BasePronto);

            var parteDiarioRequest = new DetalleParteDiarioRequest
            {
                IdDetalleParteDiario = ParteDiario.IdDetalleParteDiario,
                IdParteDiario = ParteDiario.IdParteDiario,
                IdEquipo = ParteDiario.IdEquipo,
                FechaLectura = ParteDiario.FechaLectura,
                Lectura = ParteDiario.Lectura,
                IdUnidad = ParteDiario.IdUnidad,
                IdTipoHoraNoProductiva = ParteDiario.IdTipoHoraNoProductiva,
                HorasProductivas = ParteDiario.HorasProductivas,
                HorasNoProductivas = ParteDiario.HorasNoProductivas
            };
            parteDiarioRequest.DbName = dbname.Descripcion;

            Response<object> response;
            if (IsEdit)
            {
                response = await _apiService.PutAsync(url, "/api", "/PartesDiarios/PutParte2", parteDiarioRequest.IdDetalleParteDiario, parteDiarioRequest, "bearer", token.Token);
            }
            else
            {
                response = await _apiService.PostAsync<DetalleParteDiarioRequest>(url, "/api", "/PartesDiarios", "bearer", token.Token, dbname.Descripcion, parteDiarioRequest);
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

            await PartesDiariosPageViewModel.GetInstance().UpdatePartesDiariosAsync();
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (ParteDiario.FechaLectura != FechaUltimoParte)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Fecha de parte diario incorrecta", "Accept");
                return false;
            }

            if ((ParteDiario.HorasProductivas ?? 0) == 0 && (ParteDiario.HorasNoProductivas ?? 0) == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe informar horas", "Accept");
                return false;
            }

            if ((ParteDiario.HorasProductivas ?? 0) < 0 || (ParteDiario.HorasNoProductivas ?? 0) < 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Las horas no pueden ser menores a 0", "Accept");
                return false;
            }

            if (ParteDiario.IdUnidad == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe indicar la unidad", "Accept");
                return false;
            }

            if ((ParteDiario.HorasProductivas ?? 0) + (ParteDiario.HorasNoProductivas ?? 0) > 24 && ParteDiario.Unidad == "Horas")
            {
                await App.Current.MainPage.DisplayAlert("Error", "Las horas no pueden ser mayores a 24", "Accept");
                return false;
            }

            if ((ParteDiario.HorasNoProductivas ?? 0) > 0 && ParteDiario.IdTipoHoraNoProductiva == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Si hay horas no productivas debe indicar el tipo", "Accept");
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

            //var petRequest = new PetRequest
            //{
            //    Id = ParteDiario.IdDetalleParteDiario
            //};

            //var response = await _apiService.PostAsync(url, "/api", "/Pets/DeletePet", petRequest, "bearer", token.Token);

            //if (!response.IsSuccess)
            //{
            //    IsRunning = false;
            //    IsEnabled = true;
            //    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
            //    return;
            //}

            //await PetsPageViewModel.GetInstance().UpdateOwnerAsync();

            IsRunning = false;
            IsEnabled = true;
            await _navigationService.GoBackToRootAsync();
        }

    }
}
