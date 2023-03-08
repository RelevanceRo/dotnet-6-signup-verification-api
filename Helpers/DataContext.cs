namespace TCIG.MHUSIGTASAPI.Helpers;

using Microsoft.EntityFrameworkCore;

using TCIG.MHUSIGTASAPI.Entities;



public class DataContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    
    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
    }
}