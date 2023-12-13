using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace app_shortlink.DAL;

public class SampleContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=urlsdb;Username=mike;Password=mike");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}