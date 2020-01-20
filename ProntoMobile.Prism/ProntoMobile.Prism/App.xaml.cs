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

            //var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            //if (Settings.IsRemembered && token?.Expiration > DateTime.Now)
            //{
            //    await NavigationService.NavigateAsync("/ProntoMobileMasterDetailPage/NavigationPage/PrincipalPage");
            //}
            //else
            //{
            //    await NavigationService.NavigateAsync("NavigationPage/LoginPage");
            //}
            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<IGeolocatorService, GeolocatorService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
        }
    }
}
