using Newtonsoft.Json;
using Prism;
using Prism.Ioc;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using ProntoMobile.Prism.ViewModels;
using ProntoMobile.Prism.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ProntoMobile.Prism
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTk4MDI1QDMxMzcyZTM0MmUzMGN0U0JvNGVJdktnTUtPYTVzY2VvU25ac3NVU08zUE9Rc0cvQnVWWHYzdms9");

            InitializeComponent();

            //var token = Settings.Token2;

            var token0 = Settings.Token;
            var token2 = JsonConvert.DeserializeObject<TokenResponse>(token0);
            var user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            if (user != null && Settings.IsRemembered && token2?.Expiration > DateTime.Now)
            {
                await NavigationService.NavigateAsync("/ProntoMobileMasterDetailPage/NavigationPage/PrincipalPage");
            }
            else
            {
                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<IGeolocatorService, GeolocatorService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<ProntoMobileMasterDetailPage, ProntoMobileMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<PrincipalPage, PrincipalPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<RememberPasswordPage, RememberPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<PartesDiariosPage, PartesDiariosPageViewModel>();
            containerRegistry.RegisterForNavigation<ParteDiarioPage, ParteDiarioPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<FirmasPage, FirmasPageViewModel>();
            containerRegistry.RegisterForNavigation<FirmaPage, FirmaPageViewModel>();
            containerRegistry.RegisterForNavigation<EquipmentTabbedPage, EquipmentTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<EquipmentsPage, EquipmentsPageViewModel>();
            containerRegistry.RegisterForNavigation<EquipmentPage, EquipmentPageViewModel>();
            containerRegistry.RegisterForNavigation<EditEquipmentPage, EditEquipmentPageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<BasesDatosPage, BasesDatosPageViewModel>();
            containerRegistry.RegisterForNavigation<FallasPage, FallasPageViewModel>();
            containerRegistry.RegisterForNavigation<FallaPage, FallaPageViewModel>();
        }
    }
}
