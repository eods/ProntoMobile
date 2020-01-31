using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Prism.ViewModels
{
    public class FirmasPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private ObservableCollection<FirmaItemViewModel> _firmas;
        private ObservableCollection<FirmaItemViewModel> FirmasFull;
        private bool _isRefreshing;
        private DelegateCommand _refreshFirmasCommand;
        private static FirmasPageViewModel _instance;
        private string _searchText = "";

        public FirmasPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Documentos a firmar";
            LoadFirmas();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public DelegateCommand RefreshFirmasCommand => _refreshFirmasCommand ?? (_refreshFirmasCommand = new DelegateCommand(RefreshFirmas));

        public ObservableCollection<FirmaItemViewModel> Firmas
        {
            get => _firmas;
            set => SetProperty(ref _firmas, value);
        }

        public static FirmasPageViewModel GetInstance()
        {
            return _instance;
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string SelectedFilter
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    LoadFirmasFiltered();
            }
        }

        public async Task UpdateFirmarAsync()
        {
            LoadFirmas();
            LoadFirmasFiltered();
        }

        private async void RefreshFirmas()
        {
            IsRefreshing = true;
            await UpdateFirmarAsync();
            IsRefreshing = false;
        }

        private async void LoadFirmas()
        {
            IsRefreshing = true;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BasePronto);

            if (user == null)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el login nuevamente", "Accept");
                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
                return;
            }

            if (dbname == null)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "No tiene definida la base de datos", "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            Title = $"Documentos a firmar - {dbname.Abreviatura}";

            var response = await _apiService.GetByEmailAsync<FirmaItemViewModel>(
                url, 
                "/api", 
                "/Firmas/GetFirmasByUser", 
                "bearer", 
                token.Token, 
                dbname.Descripcion, 
                user.Email);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "Error de conexion con el host", "Accept");
                return;
            }

            var myFirmas = (List<FirmaItemViewModel>)response.Result;
            Firmas = new ObservableCollection<FirmaItemViewModel>(myFirmas.Select(a => new FirmaItemViewModel(_navigationService)
            {
                IdTempAutorizacion = a.IdTempAutorizacion,
                IdComprobante = a.IdComprobante,
                TipoComprobante = a.TipoComprobante,
                TipoComprobanteAb = a.TipoComprobanteAb,
                Numero = a.Numero,
                IdAutorizacion = a.IdAutorizacion,
                IdFormulario = a.IdFormulario,
                IdDetalleAutorizacion = a.IdDetalleAutorizacion,
                SectorEmisor = a.SectorEmisor,
                OrdenAutorizacion = a.OrdenAutorizacion,
                IdAutoriza = a.IdAutoriza,
                IdSector = a.IdSector,
                IdLibero = a.IdLibero,
                Fecha = a.Fecha,
                Proveedor = a.Proveedor,
                Moneda = a.Moneda,
                ImporteTotal = a.ImporteTotal
            }).ToList());

            FirmasFull = Firmas;

            IsRefreshing = false;
        }

        private void LoadFirmasFiltered()
        {
            IsRefreshing = true;

            if (string.IsNullOrWhiteSpace(this._searchText))
                Firmas = FirmasFull;
            else
            {
                Firmas = new ObservableCollection<FirmaItemViewModel>(FirmasFull.Where(i => i.Proveedor.ToLower().Contains(_searchText.ToLower()) || i.Numero.ToLower().Contains(_searchText.ToLower())));
            }

            IsRefreshing = false;
        }
    }
}
