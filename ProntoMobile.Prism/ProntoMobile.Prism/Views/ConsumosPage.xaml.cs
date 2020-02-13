using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace ProntoMobile.Prism.Views
{
    public partial class ConsumosPage : ContentPage
    {
        public ConsumosPage()
        {
            InitializeComponent();
            this.dataGrid.CurrentCellBeginEdit += DataGrid_CurrentCellBeginEdit;
            this.dataGrid.CurrentCellEndEdit += DataGrid_CurrentCellEndEdit;
        }

        private void DataGrid_CurrentCellBeginEdit(object sender, GridCurrentCellBeginEditEventArgs args)
        {
            // Editing prevented for the cell at RowColumnIndex(2,2).
            if (args.RowColumnIndex == new Syncfusion.GridCommon.ScrollAxis.RowColumnIndex(2, 2))
                args.Cancel = true;
        }

        private void DataGrid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs args)
        {
            // Editing prevented for the cell at RowColumnIndex(1,3).
            if (args.RowColumnIndex == new Syncfusion.GridCommon.ScrollAxis.RowColumnIndex(1, 3))
                args.Cancel = true;
        }
    }
}
