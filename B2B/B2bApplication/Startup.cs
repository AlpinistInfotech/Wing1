using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2bApplication
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
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Transient);
            services.AddScoped<IAccount>(ctx => new Account(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            
            services.AddScoped<ICurrentUsers>(ctx => new CurrentUsers(ctx.GetRequiredService<IHttpContextAccessor>(), ctx.GetRequiredService<DBContext>()));
            services.AddScoped<ICustomerWallet>(ctx => new CustomerWallet(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ISettings>(ctx => new B2BClasses.Settings(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            #region **************** Flight *********************************
            services.AddScoped<ITripJack> (ctx => new TripJack(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<IBooking>(ctx => new Booking(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>(), ctx.GetRequiredService<ITripJack>()));
            


            #endregion


            services.AddAuthentication("CookieAuthentication")
                 .AddCookie("CookieAuthentication", config =>
                 {
                     config.Cookie.Name = "UserLoginCookie";
                     config.LoginPath = "/Account/Login";
                     config.ExpireTimeSpan = new TimeSpan(24, 0, 0);
                 });
            

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
