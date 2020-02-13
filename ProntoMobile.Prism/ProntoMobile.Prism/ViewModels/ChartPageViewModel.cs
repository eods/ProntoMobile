using Newtonsoft.Json;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProntoMobile.Prism.ViewModels
{
    public class ChartPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EquipmentResponse _equipment;
        private ObservableCollection<PartesDiariosItemViewModel> _partesDiarios;
        private bool _isRefreshing;

        public ChartPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Grafico";
            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            LoadPartesDiarios();
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

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
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
            }).Where(pd => pd.HorasProductivas != 0)
              .OrderByDescending(pd => pd.FechaLectura).ToList());

            IsRefreshing = false;
        }

    }
}
