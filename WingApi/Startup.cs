using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingApi.Classes.Database;
using Microsoft.EntityFrameworkCore;
using WingApi.Classes.TekTravel;
using WingApi.Classes;
using WingApi.Classes.TripJack;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WingApi
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

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:SigningKey"]));
            var validationParams = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateAudience = true,
                ValidAudience = Configuration["Token:Audience"],
                ValidateIssuer = true,
                ValidIssuer = Configuration["Token:Issuer"],
                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            services.AddScoped(serviceProvider => new JwtTokenGenerator(validationParams.ToTokenOptions()));


            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Transient);
            #region ************* Services Registration ************************
            //services.AddScoped<ITekTravel>(ctx => new TekTravel(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ITripJack>(ctx => new TripJack(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            #endregion


            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

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
