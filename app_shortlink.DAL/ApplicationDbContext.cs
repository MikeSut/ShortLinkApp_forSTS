using Microsoft.EntityFrameworkCore;

namespace app_shortlink.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }
    }
}

