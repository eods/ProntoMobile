using System;

namespace ProntoMobile.Common.Models
{
    public class DetalleParteDiarioResponse
    {
        public int IdDetalleParteDiario { get; set; }

        public int IdParteDiario { get; set; }

        public int IdEquipo { get; set; }

        public DateTime FechaLectura { get; set; }

        public int Lectura { get; set; }

        public int IdUnidad { get; set; }

        public int? IdTipoHoraNoProductiva { get; set; }

        public decimal? HorasProductivas { get; set; }

        public decimal? HorasNoProductivas { get; set; }

        public string Unidad { get; set; }

        public string UnidadAb { get; set; }

        public string TipoHoraNoProductiva { get; set; }

        public string TipoHoraNoProductivaAb { get; set; }
    }
}
