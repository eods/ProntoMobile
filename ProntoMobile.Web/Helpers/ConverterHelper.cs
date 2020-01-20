using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data;
using ProntoMobile.Web.Data.Entities;

namespace ProntoMobile.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(
            DataContext dataContext,
            ICombosHelper combosHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }

        public EquipmentResponse ToEquipmentResponse(Articulo articulo)
        {
            if (articulo == null)
            {
                return null;
            }

            return new EquipmentResponse
            {
                IdArticulo = articulo.IdArticulo,
                Descripcion = articulo.Descripcion,
                Codigo = articulo.Codigo,
                //FechaUltimaLectura = articulo.FechaUltimaLectura ?? Date,
                UltimaLectura = articulo.UltimaLectura ?? 0,
                IdUnidadLecturaMantenimiento = articulo.IdUnidadLecturaMantenimiento ?? 0,
                //IdObraActual = articulo.IdObraActual ?? 0,
                ImageUrl = articulo.ImageFullPath
            };
        }

        public DetalleParteDiarioResponse ToDetalleParteDiarioResponse(DetalleParteDiario detalleParteDiario)
        {
            if (detalleParteDiario == null)
            {
                return null;
            }

            return new DetalleParteDiarioResponse
            {
                IdDetalleParteDiario = detalleParteDiario.IdDetalleParteDiario,
                IdParteDiario = detalleParteDiario.IdParteDiario,
                IdEquipo = detalleParteDiario.IdEquipo,
                FechaLectura = detalleParteDiario.FechaLectura,
                Lectura = detalleParteDiario.Lectura,
                IdUnidad = detalleParteDiario.IdUnidad,
                HorasProductivas = detalleParteDiario.HorasProductivas,
                HorasNoProductivas = detalleParteDiario.HorasNoProductivas,
                IdTipoHoraNoProductiva = detalleParteDiario.IdTipoHoraNoProductiva
            };
        }
    }
}
