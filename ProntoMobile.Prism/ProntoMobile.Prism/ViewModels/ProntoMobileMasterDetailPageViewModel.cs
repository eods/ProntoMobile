using Prism.Navigation;
using ProntoMobile.Common.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProntoMobile.Prism.ViewModels
{
    public class ProntoMobileMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ProntoMobileMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            LoadMenus();
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        private void LoadMenus()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_truck_menu",
                    PageName = "EquipmentsPage",
                    Title = "Equipos"
                },

                new Menu
                {
                    Icon = "ic_map",
                    PageName = "MapPage",
                    Title = "Mapa"
                },

                new Menu
                {
                    Icon = "ic_list_alt",
                    PageName = "FirmasPage",
                    Title = "Firmas"
                },

                new Menu
                {
                    Icon = "ic_person",
                    PageName = "ProfilePage",
                    Title = "Modificar perfil"
                },

                new Menu
                {
                    Icon = "ic_exit_to_app",
                    PageName = "LoginPage",
                    Title = "Cerrar sesion"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());
        }
    }
}
