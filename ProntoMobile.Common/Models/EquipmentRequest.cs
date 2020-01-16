using System;
using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Common.Models
{
    public class EquipmentRequest
    {
        public int IdArticulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Codigo { get; set; }

        public DateTime FechaUltimaLectura { get; set; }

        public decimal UltimaLectura { get; set; }

        public int IdUnidadLecturaMantenimiento { get; set; }

        public byte[] ImageArray { get; set; }
    }
}
