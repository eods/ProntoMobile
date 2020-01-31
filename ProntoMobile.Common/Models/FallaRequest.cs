using System;
using System.Collections.Generic;
using System.Text;

namespace ProntoMobile.Common.Models
{
    public class FallaRequest
    {
        public int IdFalla { get; set; }

        public int IdArticulo { get; set; }

        public string Descripcion { get; set; }

        public string Anulada { get; set; }

        public DateTime? FechaFalla { get; set; }

        public string Observaciones { get; set; }

        public int? IdOrdenTrabajo { get; set; }

        public int? NumeroFalla { get; set; }

        public DateTime? FechaAlta { get; set; }

        public int? IdObra { get; set; }

        public int? IdReporto { get; set; }

        public string Maquinista { get; set; }

        public string DbName { get; set; }
    }
}
