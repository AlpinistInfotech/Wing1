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
using B2BClasses.Services.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdentityServer4.AccessTokenValidation;

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
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                };
            });
            services.AddAuthorization(options =>
            {
                foreach (enmDocumentMaster _enm in Enum.GetValues(typeof(enmDocumentMaster)))
                {
                    options.AddPolicy(_enm.ToString(), policy => policy.Requirements.Add(new AccessRightRequirement(_enm)));
                }

            });
            services.AddScoped<IAuthorizationHandler, AccessRightHandler>();
            
            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.AddScoped<IAccount>(ctx => new Account(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ITripJack>(ctx => new TripJack(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<ITBO>(ctx => new TBO(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>()));
            services.AddScoped<IBooking>(ctx => new Booking(ctx.GetRequiredService<DBContext>(), ctx.GetRequiredService<IConfiguration>(), ctx.GetRequiredService<ITripJack>(), ctx.GetRequiredService<ITBO>()));
           services.AddScoped<ICurrentUsers>(ctx => new CurrentUsers( ctx.GetRequiredService<DBContext>()));
            //services.AddScoped<ICurrentUsers>(ctx => new CurrentUsers(ctx.GetRequiredService<IHttpContextAccessor>(), ctx.GetRequiredService<DBContext>()));

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

            app.UseCors("CorsPolicy");            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
