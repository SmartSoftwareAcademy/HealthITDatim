using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using HealthITDatim.Data;
using System.IO;
using HealthITDatim.Configeration;

namespace HealthITDatim
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
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(jwt =>
           {
               var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

               jwt.SaveToken = true;
               jwt.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   RequireExpirationTime = false
               };
           });
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddEntityFrameworkStores<DatimData>();
            
            string conStringDb = "server=127.0.0.1;database=datim;uid=root;password='root';port=3307;Allow User Variables=true;";
            Globals.conString=conStringDb;
            services.AddDbContext<DatimData>(opt => opt.UseMySql(connectionString:conStringDb, new MySqlServerVersion(new Version(8, 0, 21))));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthITDatim", Version = "v1" });
                c.AddSecurityDefinition(
            "token",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization
            }
        );
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "token"
                        },
                    },
                    Array.Empty<string>()
                }
                    }
                );
            });
            services.AddCors(options =>
            {
                options.AddPolicy(name: "ServerPolicy",
                    builder =>
                    {
                        builder.WithOrigins("")
                                         .AllowAnyHeader()
                                         .AllowAnyMethod()
                                         .SetIsOriginAllowed(origin => true) // allow any origin
                                         .AllowCredentials(); // allow credential
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthITDatim v1"));
            }

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
