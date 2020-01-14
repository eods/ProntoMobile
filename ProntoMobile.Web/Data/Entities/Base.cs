using System.ComponentModel.DataAnnotations;

namespace ProntoMobile.Web.Data.Entities
{
    public class Base
    {
        [Key]
        public int IdBD { get; set; }

        public string Descripcion { get; set; }

        public string StringConection { get; set; }

        public string ServidorSQL { get; set; }

        public string Usr { get; set; }

        public string Pass { get; set; }

        public string Sistema { get; set; }

    }
}
