using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System;

namespace ProntoMobile.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;
        private DelegateCommand _registerCommand;
        private DelegateCommand _forgotPasswordCommand;

        public LoginPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;

            Title = "Login";
            IsEnabled = true;
            IsRemember = true;

            //TODO:
            Email = "pronto@yopmail.com";
            Password = "123456";
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(Register));

        public DelegateCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new DelegateCommand(ForgotPassword));

        public bool IsRemember { get; set; }

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
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

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "EmailError", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "PasswordError", "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var connection = await _apiService.CheckConnection(url);
            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "Check the internet connection.", "Accept");
                return;
            }

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "LoginError", "Accept");
                Password = string.Empty;
                return;
            }

            var token = response.Result;

            var response2 = await _apiService.GetUserByEmailAsync(
                url,
                "api",
                "/Usuarios/GetUserByEmail",
                "bearer",
                token.Token,
                Email);

            if (!response2.IsSuccess)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "This user have a big problem.", "Accept");
                Password = string.Empty;
                return;
            }

            var user = response2.Result;
            //var parameters = new NavigationParameters
            //{
            //    { "owner", owner }
            //};

            Settings.User = JsonConvert.SerializeObject(user);
            Settings.Token = JsonConvert.SerializeObject(token);
            Settings.IsRemembered = IsRemember;

            IsEnabled = true;
            IsRunning = false;

            await _navigationService.NavigateAsync("/ProntoMobileMasterDetailPage/NavigationPage/PrincipalPage");
            Password = string.Empty;
        }

        private async void Register()
        {
            await _navigationService.NavigateAsync("RegisterPage");
        }

        private async void ForgotPassword()
        {
            await _navigationService.NavigateAsync("RememberPasswordPage");
        }
    }
}
