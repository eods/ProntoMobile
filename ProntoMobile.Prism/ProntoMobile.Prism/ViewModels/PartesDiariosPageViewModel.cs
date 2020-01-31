using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProntoMobile.Prism.ViewModels
{
    public class PartesDiariosPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private ObservableCollection<PartesDiariosItemViewModel> _partesDiarios;
        private bool _isRefreshing;
        private EquipmentResponse _equipment;
        private DelegateCommand _addParteDiarioCommand;
        private DelegateCommand _refreshPartesDiariosCommand;
        private static PartesDiariosPageViewModel _instance;

        public PartesDiariosPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Partes";
            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            LoadPartesDiarios();
        }

        public DelegateCommand AddParteDiarioCommand => _addParteDiarioCommand ?? (_addParteDiarioCommand = new DelegateCommand(AddParteDiario));

        public DelegateCommand RefreshPartesDiariosCommand => _refreshPartesDiariosCommand ?? (_refreshPartesDiariosCommand = new DelegateCommand(RefreshPartesDiarios));

        private async void AddParteDiario()
        {
            await _navigationService.NavigateAsync("ParteDiarioPage");
        }

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public ObservableCollection<PartesDiariosItemViewModel> PartesDiarios
        {
            get => _partesDiarios;
            set => SetProperty(ref _partesDiarios, value);
        }

        public static PartesDiariosPageViewModel GetInstance()
        {
            return _instance;
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public async Task UpdatePartesDiariosAsync()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetByIdAsync<EquipmentResponse>(
                url,
                "/api",
                "/Equipments",
                "bearer",
                token.Token,
                _equipment.IdArticulo);

            if (response.IsSuccess)
            {
                var equipment = (EquipmentResponse)response.Result;
                Settings.Equipment = JsonConvert.SerializeObject(equipment);
                _equipment = equipment;
                LoadPartesDiarios();
            }
        }

        private async void RefreshPartesDiarios()
        {
            IsRefreshing = true;
            await UpdatePartesDiariosAsync();
            IsRefreshing = false;
        }

        private void LoadPartesDiarios()
        {
            IsRefreshing = true;

            PartesDiarios = new ObservableCollection<PartesDiariosItemViewModel>(Equipment.DetallePartesDiarios.Select(h => new PartesDiariosItemViewModel(_navigationService)
            {
                IdDetalleParteDiario = h.IdDetalleParteDiario,
                IdParteDiario = h.IdParteDiario,
                IdEquipo = h.IdEquipo,
                FechaLectura = h.FechaLectura,
                Lectura = h.Lectura,
                IdUnidad = h.IdUnidad,
                IdTipoHoraNoProductiva = h.IdTipoHoraNoProductiva,
                HorasProductivas = h.HorasProductivas,
                HorasNoProductivas = h.HorasNoProductivas,
                Unidad = h.Unidad,
                UnidadAb = h.UnidadAb,
                TipoHoraNoProductiva = h.TipoHoraNoProductiva,
                TipoHoraNoProductivaAb = h.TipoHoraNoProductivaAb
            }).OrderByDescending(pd => pd.FechaLectura).ToList());

            IsRefreshing = false;
        }
    }
}
