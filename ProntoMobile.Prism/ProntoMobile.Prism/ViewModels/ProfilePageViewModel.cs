using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System.Threading.Tasks;

namespace ProntoMobile.Prism.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private UserResponse _user;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changePasswordCommand;

        public ProfilePageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Modificar usuario";
            IsEnabled = true;
            User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
        }

        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand(ChangePassword));

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
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

        public IApiService ApiService => _apiService;

        private async void Save()
        {
            var isValid = await ValidateData();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var userRequest = new UserRequest
            {
                Address = User.Address,
                Document = User.Document,
                Email = User.Email,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Password = "123456", // It doesn't matter what is sent here. It is only for the model to be valid
                Phone = User.PhoneNumber
            };

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await ApiService.PutAsync(
                url,
                "/api",
                "/Account/PutUser2",
                userRequest,
                "bearer",
                token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            Settings.User = JsonConvert.SerializeObject(User);

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "User updated succesfully.",
                "Accept");
        }

        private async Task<bool> ValidateData()
        {
            if (string.IsNullOrEmpty(User.Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a document.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(User.FirstName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a FirstName.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(User.LastName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a LastName.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(User.Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an Address.", "Accept");
                return false;
            }

            return true;
        }

        private async void ChangePassword()
        {
            await _navigationService.NavigateAsync("ChangePasswordPage");
        }
    }
}
