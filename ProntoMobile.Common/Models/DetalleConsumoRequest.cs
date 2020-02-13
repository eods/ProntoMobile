using System;

namespace ProntoMobile.Common.Models
{
    public class DetalleConsumoRequest
    {
        public int IdDetalleConsumo { get; set; }

        public int IdConsumo { get; set; }

        public int IdArticulo { get; set; }

        public int IdConsumible { get; set; }

        public decimal Cantidad { get; set; }

        public int IdUnidadConsumible { get; set; }

        public decimal? Costo { get; set; }

        public string Observaciones { get; set; }

        public DateTime FechaConsumo { get; set; }

        public string DbName { get; set; }

        public int IdResponsable { get; set; }
    }
}
