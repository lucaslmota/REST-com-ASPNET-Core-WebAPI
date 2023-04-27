using DevIo.API.Data;
using DevIo.API.Extensions;
using DevIo.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevIo.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration (this IServiceCollection services, IConfiguration configuration)
        {
            //var connectionString = builder.Configuration.GetConnectionString("ConnStr");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ConnStr")));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddErrorDescriber<IdentityMensagensPortugues>.AddDefaultTokenProviders();
            
            return services;
        }
    }
}
