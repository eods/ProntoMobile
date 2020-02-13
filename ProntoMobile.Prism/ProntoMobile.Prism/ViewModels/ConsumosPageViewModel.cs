using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ProntoMobile.Prism.ViewModels
{
    public class ConsumosPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private ObservableCollection<ConsumoItemViewModel> _consumos;
        private bool _isRefreshing;
        private EquipmentResponse _equipment;
        private DelegateCommand _addConsumoCommand;
        private DelegateCommand _refreshConsumosCommand;
        private static ConsumosPageViewModel _instance;
        private Command selectionCommand;
        private Command cellChangedCommand;

        public ConsumosPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Consumos";
            Equipment = JsonConvert.DeserializeObject<EquipmentResponse>(Settings.Equipment);
            LoadConsumos();

            TappedCommandAction = new DerivedTappedCommand(this);
            CellChangedCommand = new DerivedCellChangedCommand(this);
            selectionCommand = new Command(onSelectionChanged);
        }
        public DelegateCommand AddConsumoCommand => _addConsumoCommand ?? (_addConsumoCommand = new DelegateCommand(AddConsumo));

        public DelegateCommand RefreshConsumosCommand => _refreshConsumosCommand ?? (_refreshConsumosCommand = new DelegateCommand(RefreshConsumos));


        // DataGrid
        public Command SelectionCommand
        {
            get { return selectionCommand; }
            set { selectionCommand = value; }
        }

        private void onSelectionChanged()
        {
            var a = 1;
        }

        public DerivedCellChangedCommand CellChangedCommand
        {
            get;
            set;
        }

        public class DerivedCellChangedCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private ConsumosPageViewModel viewModel
            {
                get;
                set;
            }
            public DerivedCellChangedCommand(ConsumosPageViewModel view)
            {
                viewModel = view;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                var a = 1;
            }
        }

        public DerivedTappedCommand TappedCommandAction
        {
            get;
            set;
        }

        public class DerivedTappedCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private ConsumosPageViewModel viewModel
            {
                get;
                set;
            }
            public DerivedTappedCommand(ConsumosPageViewModel view)
            {
                viewModel = view;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                var a = 1;
            }
        }

        // ------------------------------------------------------


        private async void AddConsumo()
        {
            await _navigationService.NavigateAsync("ConsumoPage");
        }

        public EquipmentResponse Equipment
        {
            get => _equipment;
            set => SetProperty(ref _equipment, value);
        }

        public ObservableCollection<ConsumoItemViewModel> Consumos
        {
            get => _consumos;
            set => SetProperty(ref _consumos, value);
        }

        public static ConsumosPageViewModel GetInstance()
        {
            return _instance;
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public async Task UpdateConsumosAsync()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

            if (dbname == null)
            {
                IsRefreshing = false;
                await App.Current.MainPage.DisplayAlert("Error", "No tiene definida la base de datos", "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var response = await _apiService.GetByIdAsync<EquipmentResponse>(
                url,
                "/api",
                "/Equipments/GetEquipmentById",
                "bearer",
                token.Token,
                dbname.Descripcion,
                _equipment.IdArticulo);

            if (response.IsSuccess)
            {
                var equipment = (EquipmentResponse)response.Result;
                Settings.Equipment = JsonConvert.SerializeObject(equipment);
                _equipment = equipment;
                LoadConsumos();
            }
        }

        private async void RefreshConsumos()
        {
            IsRefreshing = true;
            await UpdateConsumosAsync();
            IsRefreshing = false;
        }

        private void LoadConsumos()
        {
            IsRefreshing = true;

            if (Equipment.DetalleConsumos != null)
            {
                Consumos = new ObservableCollection<ConsumoItemViewModel>(Equipment.DetalleConsumos.Select(h => new ConsumoItemViewModel(_navigationService)
                {
                    IdDetalleConsumo = h.IdDetalleConsumo,
                    IdConsumo = h.IdConsumo,
                    IdArticulo = h.IdArticulo,
                    IdConsumible = h.IdConsumible,
                    Cantidad = h.Cantidad,
                    IdUnidadConsumible = h.IdUnidadConsumible,
                    Costo = h.Costo,
                    Observaciones = h.Observaciones,
                    Consumo = h.Consumo,
                    Equipo = h.Equipo,
                    Consumible = h.Consumible,
                    Unidad = h.Unidad,
                    UnidadAb = h.UnidadAb,
                    FechaConsumo = h.FechaConsumo
                }).OrderByDescending(pd => pd.FechaConsumo).ToList());
            }

            IsRefreshing = false;
        }
    }
}
