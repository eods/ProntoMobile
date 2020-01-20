using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        public int NumeroPedido { get; set; }

        [Required]
        [ForeignKey("Proveedor")]
        public int IdProveedor { get; set; }

        public DateTime FechaPedido { get; set; }

        [Required]
        [ForeignKey("Moneda")]
        public int IdMoneda { get; set; }

        public decimal TotalPedido { get; set; }

        public ICollection<DetallePedido> DetallePedidos { get; set; }
    }
}
