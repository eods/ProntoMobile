using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class Consumo
    {
        [Key]
        public int IdConsumo { get; set; }

        public int NumeroConsumo { get; set; }

        public DateTime FechaConsumo { get; set; }

        public int IdObra { get; set; }

        [ForeignKey("Empleado")]
        public int? IdResponsable { get; set; }

        public ICollection<DetalleConsumo> DetalleConsumos { get; set; }

        public Empleado Empleado { get; set; }
    }
}
