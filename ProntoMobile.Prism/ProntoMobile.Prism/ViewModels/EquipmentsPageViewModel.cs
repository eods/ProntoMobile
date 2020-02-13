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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Prism.ViewModels
{
    public class EquipmentsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRefreshing;
        private ObservableCollection<EquipmentItemViewModel> _equipments;
        private ObservableCollection<EquipmentItemViewModel> EquipmentsFull;
        private DelegateCommand _refreshEquipmentsCommand;
        private static EquipmentsPageViewModel _instance;
        private string _searchText = "";

        public EquipmentsPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Equipos";
            LoadEquipments();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public DelegateCommand RefreshEquipmentsCommand => _refreshEquipmentsCommand ?? (_refreshEquipmentsCommand = new DelegateCommand(RefreshEquipments));

        private async void LoadEquipments()
        {
            IsRefreshing = true;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

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

            Title = $"Equipos - {dbname.Abreviatura}";

            var isconected = await _apiService.CheckConnection(url);
            if (!isconected)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "No hay conexion con el host", "Accept");
                return;
            }

            var response = await _apiService.GetByEmailAsync<EquipmentResponse>(url, "/api", "/Equipments/GetEquipments3", "bearer", token.Token, dbname.Descripcion, user.Email);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "Error de conexion con el host", "Accept");
                return;
            }

            var myEquipment = (List<EquipmentResponse>)response.Result;
            Equipments = new ObservableCollection<EquipmentItemViewModel>
                (myEquipment.Select(a => new EquipmentItemViewModel(_navigationService)
                {
                    IdArticulo = a.IdArticulo,
                    Descripcion = a.Descripcion,
                    Codigo = a.Codigo,
                    FechaUltimaLectura = a.FechaUltimaLectura,
                    UltimaLectura = a.UltimaLectura,
                    IdUnidadLecturaMantenimiento = a.IdUnidadLecturaMantenimiento,
                    //IdObraActual = a.IdObraActual,
                    ImageUrl = a.ImageUrl,
                    DetallePartesDiarios = a.DetallePartesDiarios,
                    Fallas = a.Fallas,
                    DetalleConsumos = a.DetalleConsumos
                }).ToList());  //.Where(e => e.Descripcion.ToLower().Contains("a")).ToList());

            EquipmentsFull = Equipments;

            IsRefreshing = false;
        }

        private void LoadEquipmentsFiltered()
        {
            IsRefreshing = true;

            if (string.IsNullOrWhiteSpace(this._searchText))
                Equipments = EquipmentsFull;
            else
            {
                Equipments = new ObservableCollection<EquipmentItemViewModel>(EquipmentsFull.Where(i => i.Descripcion.ToLower().Contains(_searchText.ToLower())));
            }

            IsRefreshing = false;
        }

        public ObservableCollection<EquipmentItemViewModel> Equipments
        {
            get => _equipments;
            set => SetProperty(ref _equipments, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public static EquipmentsPageViewModel GetInstance()
        {
            return _instance;
        }

        public string SelectedFilter
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    LoadEquipmentsFiltered();
            }
        }

        public async Task UpdateEquipmentAsync(int Id)
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            //var response = await _apiService.GetListAsync<EquipmentResponse>(
            //    url,
            //    "/api",
            //    "/Equipments/"+ Id,
            //    "bearer",
            //    token.Token);

            //if (response.IsSuccess)
            //{
            //    //var owner = (OwnerResponse)response.Result;
            //    //Settings.Owner = JsonConvert.SerializeObject(owner);
            //    LoadEquipments();
            //}
            LoadEquipments();
        }

        private async void RefreshEquipments()
        {
            IsRefreshing = true;
            //await UpdateEquipmentAsync();
            IsRefreshing = false;
        }

    }
}
