using System;
using System.Collections.Generic;

namespace ProntoMobile.Common.Models
{
    public class EquipmentResponse
    {
        public int IdArticulo { get; set; }

        public string Descripcion { get; set; }

        public string Codigo { get; set; }

        public DateTime FechaUltimaLectura { get; set; }

        public decimal UltimaLectura { get; set; }

        public int IdUnidadLecturaMantenimiento { get; set; }

        public string Unidad { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<DetalleParteDiarioResponse> DetallePartesDiarios { get; set; }

        public ICollection<FallaResponse> Fallas { get; set; }
    }
}
