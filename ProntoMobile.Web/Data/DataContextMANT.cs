using Microsoft.EntityFrameworkCore;

namespace ProntoMobile.Web.Data
{
    public class DataContextMANT : DbContext
    {
        public DataContextMANT(DbContextOptions<DataContext> options) : base(options)
        {
        }

    }
}
