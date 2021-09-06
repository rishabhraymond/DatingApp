using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions {
    public static class ApplicationServiceExtensions {
        public static IServiceCollection GetApplicationExtensions(this IServiceCollection services, IConfiguration configuration) {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DBContext>(context => {
                context.UseSqlite(configuration.GetConnectionString("DatingApp"));
            });

            return services;
        }
    }
}
