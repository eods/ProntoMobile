using System;

namespace ProntoMobile.Common.Models
{
    public class DetalleConsumoResponse
    {
        public int IdDetalleConsumo { get; set; }

        public int IdConsumo { get; set; }

        public int IdArticulo { get; set; }

        public int IdConsumible { get; set; }

        public decimal Cantidad { get; set; }

        public int IdUnidadConsumible { get; set; }

        public decimal Costo { get; set; }

        public string Observaciones { get; set; }

        public string Consumo { get; set; }

        public string Equipo { get; set; }

        public string Consumible { get; set; }

        public string Unidad { get; set; }

        public string UnidadAb { get; set; }

        public DateTime FechaConsumo { get; set; }
    }
}
