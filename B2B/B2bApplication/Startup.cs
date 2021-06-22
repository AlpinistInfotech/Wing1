using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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

            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.AddDbContext<B2BClasses.Database.LogDatabase.LogDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LogConnection")), ServiceLifetime.Transient);

            services.AddScoped<IAccount>(ctx => new Account(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            
            services.AddScoped<ICurrentUsers>(ctx => new CurrentUsers(ctx.GetRequiredService<IHttpContextAccessor>(), ctx.GetRequiredService<DBContext>()));
            
            services.AddScoped<ICustomerMaster>(ctx => new CustomerMaster(ctx.GetRequiredService<DBContext>(),
                ctx.GetRequiredService<B2BClasses.Database.LogDatabase.LogDBContext>(), ctx.GetRequiredService<ISettings>(), ctx.GetRequiredService<IConfiguration>(), ctx.GetRequiredService<ICurrentUsers>().UserId));
            services.AddScoped<ICustomerWallet>(ctx => new CustomerWallet(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ISettings>(ctx => new B2BClasses.Settings(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<IMasters>(ctx => new B2BClasses.Masters(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<IMarkup>(ctx => new B2BClasses.Markup(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            
            #region **************** Flight *********************************
            services.AddScoped<ITripJack> (ctx => new TripJack(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ITBO>(ctx => new TBO(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<IBooking>(ctx => new Booking(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>(), ctx.GetRequiredService<ITripJack>(), ctx.GetRequiredService<ITBO>()));



            #endregion



            services.AddRazorPages(options => { options.Conventions.AuthorizeFolder("/Admin"); })
            .AddCookieTempDataProvider(options => { options.Cookie.IsEssential = true; });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                 .AddCookie(config =>
                 {
                     //config.Cookie.Name = "UserLoginCookie";
                     config.LoginPath = "/Account/Login";
                     config.ExpireTimeSpan = new TimeSpan(24, 0, 0);
                 });

            services.AddAuthorization(options =>
            {
                foreach (var name in Enum.GetValues(typeof(B2BClasses.Services.Enums.enmDocumentMaster)))
                {
                    options.AddPolicy(name.ToString(), p => p.Requirements.Add(new AccessRightRequirement((B2BClasses.Services.Enums.enmDocumentMaster)name)));
                }
            });

            services.AddScoped<IAuthorizationHandler, AccessRightHandler>();


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
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                CheckConsentNeeded= context => true
            };
            app.UseCookiePolicy(cookiePolicyOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
