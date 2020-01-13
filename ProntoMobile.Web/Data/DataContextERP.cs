using Microsoft.EntityFrameworkCore;

namespace ProntoMobile.Web.Data
{
    public class DataContextERP : DbContext
    {
        public DataContextERP(DbContextOptions<DataContext> options) : base(options)
        {
        }

    }
}
