using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProntoMobile.Web.Data.Entities
{
    public class DetalleConsumo
    {
        [Key]
        public int IdDetalleConsumo { get; set; }

        [Required]
        [ForeignKey("Consumo")]
        public int IdConsumo { get; set; }

        [Required]
        public int IdArticulo { get; set; }

        public int IdConsumible { get; set; }

        public decimal Cantidad { get; set; }

        [Required]
        [ForeignKey("Unidad")]
        public int IdUnidadConsumible { get; set; }
        
        public decimal Costo { get; set; }

        public string Observaciones { get; set; }

        public Consumo Consumo { get; set; }

        public Unidad Unidad { get; set; }

        [ForeignKey("IdArticulo")]
        [InverseProperty("DetalleConsumos")]
        public virtual Articulo Articulo { get; set; }

        [ForeignKey("IdConsumible")]
        public virtual Articulo Consumible { get; set; }
    }
}
