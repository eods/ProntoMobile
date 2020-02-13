using ProntoMobile.Common.Models;
using ProntoMobile.Web.Data.Entities;

namespace ProntoMobile.Web.Helpers
{
    public interface IConverterHelper
    {
        EquipmentResponse ToEquipmentResponse(Articulo articulo);

        DetalleParteDiarioResponse ToDetalleParteDiarioResponse(DetalleParteDiario detalleParteDiario);

        FallaResponse ToFallaResponse(Falla falla);

        DetalleConsumoResponse ToConsumoResponse(DetalleConsumo consumo);
    }

}
