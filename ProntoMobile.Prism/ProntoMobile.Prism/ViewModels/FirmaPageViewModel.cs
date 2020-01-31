using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProntoMobile.Prism.ViewModels
{
    public class FirmaPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private FirmaResponse _firma;
        private ObservableCollection<DetalleComprobanteResponse> _detallesComprobante;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _signCommand;

        public FirmaPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;
        }

        public DelegateCommand SignCommand => _signCommand ?? (_signCommand = new DelegateCommand(SignAsync));

        public FirmaResponse Firma
        {
            get => _firma;
            set => SetProperty(ref _firma, value);
        }

        public ObservableCollection<DetalleComprobanteResponse> DetallesComprobante
        {
            get => _detallesComprobante;
            set => SetProperty(ref _detallesComprobante, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("firma"))
            {
                Firma = parameters.GetValue<FirmaResponse>("firma");
                Title = "Documento a firmar";
                LoadComprobante();
            }
        }

        private async void LoadComprobante()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            //Esto es para cambiar el UrlAPI en el movil
            //Application.Current.Resources["UrlAPI"] = "http://myvet.bdlconsultores1.com.ar";
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BasePronto);

            //var IdRequest = new IdRequest
            //{
            //    Id = Firma.IdTempAutorizacion
            //};

            var response = await _apiService.GetByIdAsync<FirmaResponse>(url, "/api", "/Firmas/GetFirmasById", "bearer", token.Token, dbname.Descripcion, Firma.IdTempAutorizacion);
            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            var miComprobante = (FirmaResponse)response.Result;
            var misDetalles = (List<DetalleComprobanteResponse>)miComprobante.DetallesComprobante;
            DetallesComprobante = new ObservableCollection<DetalleComprobanteResponse>(misDetalles.Select(a => new DetalleComprobanteResponse
            {
                IdDetalleComprobante = a.IdDetalleComprobante,
                IdComprobante = a.IdComprobante,
                Detalle = a.Detalle,
                Cantidad = a.Cantidad,
                Importe = a.Importe
            }).ToList());
        }

        private async void SignAsync()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
                "Confirm",
                "Esta seguro de firmar este documento?",
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
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BasePronto);

            var IdRequest = new IdRequest
            {
                Id = Firma.IdTempAutorizacion
            };

            var response = await _apiService.PostAsync<FirmaResponse>(url, "/api", "/Firmas/FirmarDocumento", "bearer", token.Token, dbname.Descripcion, Firma.IdTempAutorizacion);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            await FirmasPageViewModel.GetInstance().UpdateFirmarAsync();

            IsRunning = false;
            IsEnabled = true;
            await _navigationService.GoBackAsync();
        }

    }
}
