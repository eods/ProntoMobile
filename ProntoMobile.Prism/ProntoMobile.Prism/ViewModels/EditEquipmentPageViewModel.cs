using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProntoMobile.Prism.ViewModels
{
    public class EditEquipmentPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EquipmentResponse _equipment;
        private ImageSource _imageSource;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;
        //private ObservableCollection<PetTypeResponse> _petTypes;
        //private PetTypeResponse _petType;
        private MediaFile _file;
        private DelegateCommand _changeImageCommand;
        private DelegateCommand _saveCommand;
        private DelegateCommand _deleteCommand;

        public EditEquipmentPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;
        }

        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public DelegateCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand(DeleteAsync));

        //public ObservableCollection<PetTypeResponse> PetTypes
        //{
        //    get => _petTypes;
        //    set => SetProperty(ref _petTypes, value);
        //}

        //public PetTypeResponse PetType
        //{
        //    get => _petType;
        //    set => SetProperty(ref _petType, value);
        //}


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

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("equipment"))
            {
                Equipment = parameters.GetValue<EquipmentResponse>("equipment");
                ImageSource = Equipment.ImageUrl ?? "noimage";
                IsEdit = true;
                Title = "Modificacion de equipo";
            }
            else
            {
                Equipment = new EquipmentResponse { FechaUltimaLectura = DateTime.Today };
                ImageSource = "noimage";
                IsEdit = false;
                Title = "Nuevo equipo";
            }

            LoadPetTypesAsync();
        }

        private async void LoadPetTypesAsync()
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
                    "Verifique su conexion a internet.",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            //var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            //var response = await _apiService.GetListAsync<PetTypeResponse>(
            //    url,
            //    "/api",
            //    "/PetTypes",
            //    "bearer",
            //    token.Token);

            IsEnabled = true;

            //if (!response.IsSuccess)
            //{
            //    await App.Current.MainPage.DisplayAlert(
            //        "Error",
            //        "Getting the pet types list, please try later,",
            //        "Accept");
            //    await _navigationService.GoBackAsync();
            //    return;
            //}

            //var petTypes = (List<PetTypeResponse>)response.Result;
            //PetTypes = new ObservableCollection<PetTypeResponse>(petTypes);

            //if (!string.IsNullOrEmpty(Pet.PetType))
            //{
            //    PetType = PetTypes.FirstOrDefault(pt => pt.Name == Pet.PetType);
            //}
        }

        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                "Desde donde quiere incorporar la imagen?",
                "Cancel",
                null,
                "Desde la galeria",
                "Desde la camara");

            if (source == "Cancel")
            {
                _file = null;
                return;
            }

            if (source == "Desde la camara")
            {
                _file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                _file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (_file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = _file.GetStream();
                    return stream;
                });
            }
        }

        private async void SaveAsync()
        {
            var isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = FilesHelper.ReadFully(_file.GetStream());
            }

            var equipmentRequest = new EquipmentRequest
            {
                IdArticulo = Equipment.IdArticulo,
                Descripcion = Equipment.Descripcion,
                Codigo = Equipment.Codigo,
                FechaUltimaLectura = Equipment.FechaUltimaLectura,
                UltimaLectura = Equipment.UltimaLectura,
                IdUnidadLecturaMantenimiento = Equipment.IdUnidadLecturaMantenimiento,
                //IdObraActual = Equipment.IdObraActual,
                ImageArray = imageArray,
                DbName = dbname.Descripcion
            };

            Response<object> response;
            //if (IsEdit)
            //{
                response = await _apiService.PutAsync(url, "/api", "/Equipments/PutEquipment2", equipmentRequest.IdArticulo, equipmentRequest, "bearer", token.Token);
            //}
            //else
            //{
                //response = await _apiService.PostAsync(url, "/api", "/Equipments", equipmentRequest, "bearer", token.Token);
            //}

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "Equipo registrado",
                //string.Format(Languages.CreateEditPetConfirm, IsEdit ? Languages.Edited : Languages.Created),
                "Accept");

            await EquipmentsPageViewModel.GetInstance().UpdateEquipmentAsync(Equipment.IdArticulo);
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(Equipment.Descripcion))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Sin descripcion", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Equipment.Codigo))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Sin codigo", "Accept");
                return false;
            }

            //if (PetType == null)
            //{
            //    await App.Current.MainPage.DisplayAlert("Error", Languages.PetTypeError, "Accept");
            //    return false;
            //}

            return true;
        }

        private async void DeleteAsync()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
                "Confirm",
                "Esta seguro de eliminar el equipo ?",
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

            var petRequest = new EquipmentRequest
            {
                IdArticulo = Equipment.IdArticulo
            };

            //var response = await _apiService.PostAsync(url, "/api", "/Equipments/DeleteEquipment", petRequest, "bearer", token.Token);

            //if (!response.IsSuccess)
            //{
            //    IsRunning = false;
            //    IsEnabled = true;
            //    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
            //    return;
            //}

            await EquipmentsPageViewModel.GetInstance().UpdateEquipmentAsync(Equipment.IdArticulo);

            IsRunning = false;
            IsEnabled = true;
            await _navigationService.GoBackToRootAsync();
        }

    }
}
