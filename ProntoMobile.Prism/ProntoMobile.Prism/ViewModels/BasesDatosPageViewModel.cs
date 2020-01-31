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

namespace ProntoMobile.Prism.ViewModels
{
    public class BasesDatosPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private ObservableCollection<BaseResponse> _basesPronto;
        private ObservableCollection<BaseResponse> _basesMantenimiento;
        private BaseResponse _basePronto;
        private BaseResponse _baseMantenimiento;
        private DelegateCommand _saveCommand;

        public BasesDatosPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public ObservableCollection<BaseResponse> BasesPronto
        {
            get => _basesPronto;
            set => SetProperty(ref _basesPronto, value);
        }

        public BaseResponse BasePronto
        {
            get => _basePronto;
            set => SetProperty(ref _basePronto, value);
        }

        public ObservableCollection<BaseResponse> BasesMantenimiento
        {
            get => _basesMantenimiento;
            set => SetProperty(ref _basesMantenimiento, value);
        }

        public BaseResponse BaseMantenimiento
        {
            get => _baseMantenimiento;
            set => SetProperty(ref _baseMantenimiento, value);
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

            LoadCombos();
        }

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
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);

            var response = await _apiService.GetByEmailAsync<BaseResponse>(
                url,
                "/api",
                "/Bases/GetBasesPorEmail",
                "bearer",
                token.Token,
                string.Empty,
                user.Email);

            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Error al leer las bases de datos, intente mas tarde",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var basesResponse = (List<BaseResponse>)response.Result;

            BasesPronto = new ObservableCollection<BaseResponse>(basesResponse);
            BasesMantenimiento = new ObservableCollection<BaseResponse>(basesResponse);

            var ult_base1 = JsonConvert.DeserializeObject<BaseResponse>(Settings.BasePronto);
            if (ult_base1 != null)
            {
                BasePronto = BasesPronto.FirstOrDefault(b => b.Descripcion == ult_base1.Descripcion);
            }

            var ult_base2 = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);
            if (ult_base2 != null)
            {
                BaseMantenimiento = BasesPronto.FirstOrDefault(b => b.Descripcion == ult_base2.Descripcion);
            }
        }

        private async void SaveAsync()
        {
            Settings.BasePronto = JsonConvert.SerializeObject(BasePronto);
            Settings.BaseMantenimiento = JsonConvert.SerializeObject(BaseMantenimiento);
            await _navigationService.GoBackAsync();
        }


    }
}
