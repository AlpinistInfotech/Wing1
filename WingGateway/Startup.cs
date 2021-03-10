using Database;
using Database.Classes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WingGateway.Classes;

namespace WingGateway
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
            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(option=> {
                    option.Password.RequireDigit = true;
                    option.Password.RequireLowercase = true;
                    option.Password.RequireUppercase = true;
                    option.Password.RequiredLength = 7;
                    option.Lockout.MaxFailedAccessAttempts = 4;
                    option.Lockout.DefaultLockoutTimeSpan = new TimeSpan(24, 0, 0);
                }
            ).AddEntityFrameworkStores<DBContext>();            

            #region ************* Services Registration ************************
            services.AddScoped<ICaptchaGenratorBase>(ctx => new CaptchaGenratorBase(ctx.GetRequiredService<DBContext>()));
            services.AddScoped<ISequenceMaster>(ctx => new SequenceMaster(ctx.GetRequiredService<DBContext>()));
            services.AddScoped<IConsolidatorProfile>(ctx => new ConsolidatorProfile(ctx.GetRequiredService<DBContext>()));
            services.AddScoped<ITcMaster>(ctx => new TcMaster(ctx.GetRequiredService<DBContext>()));
            services.AddScoped<IConsProfile>(ctx => new ConsProfile(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));

            #endregion


            services.AddScoped<ICurrentUsers>(ctx => new CurrentUsers(ctx.GetRequiredService<IHttpContextAccessor>(), ctx.GetRequiredService<DBContext>()) );
            services.AddAuthorization(options =>
            {
                foreach (var name in Enum.GetNames(typeof(enmDocumentMaster)))
                {
                    options.AddPolicy(name, p => p.RequireClaim(name));
                }                
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
