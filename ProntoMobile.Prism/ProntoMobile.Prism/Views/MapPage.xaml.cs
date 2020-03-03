using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ProntoMobile.Prism.Views
{
    public partial class MapPage : ContentPage
    {
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;

        public MapPage(
            IGeolocatorService geolocatorService,
            IApiService apiService)
        {
            InitializeComponent();
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            //MoveMapToCurrentPositionAsync();
            //ShowOwnersAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MoveMapToCurrentPositionAsync();
        }

        private async void ShowOwnersAsync()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            //var response = await _apiService.GetListAsync<OwnerResponse>(url, "api", "/Owners", "bearer", token.Token);

            //if (!response.IsSuccess)
            //{
            //    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
            //    return;
            //}

            //var owners = (List<OwnerResponse>)response.Result;

            //foreach (var owner in owners)
            //{
            //    MyMap.Pins.Add(new Pin
            //    {
            //        Address = owner.Address,
            //        Label = owner.FullName,
            //        Position = new Position(owner.Latitude, owner.Longitude),
            //        Type = PinType.Place
            //    });
            //}
        }

        //private async void MoveMapToCurrentPositionAsync()
        //{
        //    await _geolocatorService.GetLocationAsync();
        //    if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
        //    {
        //        var position = new Position(
        //            _geolocatorService.Latitude,
        //            _geolocatorService.Longitude);
        //        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
        //            position,
        //            Distance.FromKilometers(.5)));
        //    }
        //}

        private async void MoveMapToCurrentPositionAsync()
        {
            bool isLocationPermision = await CheckLocationPermisionsAsync();

            if (isLocationPermision)
            {
                MyMap.IsShowingUser = true;

                await _geolocatorService.GetLocationAsync();
                if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
                {
                    Position position = new Position(
                        _geolocatorService.Latitude,
                        _geolocatorService.Longitude);
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        position,
                        Distance.FromKilometers(.5)));
                }
            }
        }

        private async Task<bool> CheckLocationPermisionsAsync()
        {
            PermissionStatus permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            PermissionStatus permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            PermissionStatus permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            bool isLocationEnabled = permissionLocation == PermissionStatus.Granted ||
                                     permissionLocationAlways == PermissionStatus.Granted ||
                                     permissionLocationWhenInUse == PermissionStatus.Granted;
            if (isLocationEnabled)
            {
                return true;
            }

            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

            permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            return permissionLocation == PermissionStatus.Granted ||
                   permissionLocationAlways == PermissionStatus.Granted ||
                   permissionLocationWhenInUse == PermissionStatus.Granted;
        }

    }
}
