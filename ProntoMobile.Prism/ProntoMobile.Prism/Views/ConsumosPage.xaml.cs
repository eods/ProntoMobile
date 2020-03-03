using Newtonsoft.Json;
using ProntoMobile.Common.Helpers;
using ProntoMobile.Common.Models;
using ProntoMobile.Common.Service;
using ProntoMobile.Prism.ViewModels;
using Syncfusion.SfDataGrid.XForms;
using System;
using Xamarin.Forms;

namespace ProntoMobile.Prism.Views
{
    public partial class ConsumosPage : ContentPage
    {
        public ConsumosPage()
        {
            InitializeComponent();
            //dataGrid.CurrentCellBeginEdit += DataGrid_CurrentCellBeginEdit;
            dataGrid.CurrentCellEndEdit += DataGrid_CurrentCellEndEdit;
        }

        private void DataGrid_CurrentCellBeginEdit(object sender, GridCurrentCellBeginEditEventArgs args)
        {
            // Editing prevented for the cell at RowColumnIndex(2,2).
            //if (args.RowColumnIndex == new Syncfusion.GridCommon.ScrollAxis.RowColumnIndex(2, 2))
            //{
            //    args.Cancel = true;
            //}
        }

        private async void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs args)
        {
            //var data = dataGrid.GetRowGenerator().Items.FirstOrDefault(x => x.RowIndex == args.RowColumnIndex.RowIndex).RowData;
            //var data3 = dataGrid.CurrentItem;

            if (args.NewValue != args.OldValue)
            {
                var cant = args.NewValue.ToString();

                var rowselect = dataGrid.SelectedItem;
                int IdDetalleConsumo = (int)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "IdDetalleConsumo");
                int IdConsumo = (int)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "IdConsumo");
                int IdArticulo = (int)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "IdArticulo");
                int IdConsumible = (int)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "IdConsumible");
                int IdUnidadConsumible = (int)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "IdUnidadConsumible");
                decimal Costo = (decimal)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "Costo");
                string Observaciones = dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "Observaciones").ToString();
                DateTime FechaConsumo = (DateTime)dataGrid.View.GetPropertyAccessProvider().GetValue(rowselect, "FechaConsumo");

                string url = App.Current.Resources["UrlAPI"].ToString();
                UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
                BaseResponse dbname = JsonConvert.DeserializeObject<BaseResponse>(Settings.BaseMantenimiento);

                ApiService _apiService = new ApiService();

                Response<object> response2 = await _apiService.GetByEmail2Async<EmpleadoResponse>(url, "/api", "/Usuarios/GetEmpleadoByUserEmail", "bearer", token.Token, dbname.Descripcion, user.Email);
                if (!response2.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Error", response2.Message, "Accept");
                    return;
                }
                EmpleadoResponse empleado = (EmpleadoResponse)response2.Result;

                DetalleConsumoRequest consumoRequest = new DetalleConsumoRequest
                {
                    IdDetalleConsumo = IdDetalleConsumo,
                    IdConsumo = IdConsumo,
                    IdArticulo = IdArticulo,
                    IdConsumible = IdConsumible,
                    Cantidad = decimal.Parse(cant),
                    IdUnidadConsumible = IdUnidadConsumible,
                    Costo = Costo,
                    Observaciones = Observaciones,
                    FechaConsumo = FechaConsumo,
                    IdResponsable = empleado.IdEmpleado
                };
                consumoRequest.DbName = dbname.Descripcion;

                Response<object> response;
                response = await _apiService.PostAsync<DetalleConsumoRequest>(url, "/api", "/Consumos", "bearer", token.Token, dbname.Descripcion, consumoRequest);

                if (!response.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                    return;
                }

                await ConsumosPageViewModel.GetInstance().UpdateConsumosAsync();

            }

            // Editing prevented for the cell at RowColumnIndex(1,3).
            //if (args.RowColumnIndex == new Syncfusion.GridCommon.ScrollAxis.RowColumnIndex(1, 3))
            //{
            //    args.Cancel = true;
            //}
        }
    }
}
