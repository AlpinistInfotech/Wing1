using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BApis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.AddScoped<IAccount>(ctx => new Account(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ITripJack>(ctx => new TripJack(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ITBO>(ctx => new TBO(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<IBooking>(ctx => new Booking(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>(), ctx.GetRequiredService<ITripJack>(), ctx.GetRequiredService<ITBO>()));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "B2BApis", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "B2BApis v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
