using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class DetallePedido
    {
        [Key]
        public int IdDetallePedido { get; set; }

        [Required]
        [ForeignKey("Pedido")]
        public int IdPedido { get; set; }

        public int NumeroItem { get; set; }

        public decimal Cantidad { get; set; }

        [Required]
        [ForeignKey("Unidad")]
        public int IdUnidad { get; set; }

        [Required]
        [ForeignKey("Articulo")]
        public int IdArticulo { get; set; }

        public DateTime FechaEntrega { get; set; }

        public decimal Precio { get; set; }

        public Pedido Pedido { get; set; }

        public Articulo Articulo { get; set; }
    }
}
