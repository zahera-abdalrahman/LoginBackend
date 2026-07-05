using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationVSAuthorization.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        IConfiguration configure;

        public AppDbContext(IConfiguration _configure)
        {
            configure = _configure;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configure.GetConnectionString("SqlCon"));

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
        }
    }
}
