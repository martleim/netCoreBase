using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using GenericAirways.DependencyResolver;
using GenericAirways.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using GenericAirways.WebApi.Auth;

namespace GenericAirways.WebApi
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

            ConfigureSecurityServices(services);


            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Generic Airways API", Version = "v1" });
            });

            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://example.com"));
                    options.AddPolicy("AllowAllOrigins",
                    builder =>{
                        builder.AllowAnyOrigin();
                    });
            });

            ComponentLoader.LoadContainer(services, "", "*.dll");

            /*services.AddTransient<IUserStore<User>, UserStore<User>>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Generic Airways API");
            });
            
            app.UseAuthentication();
            app.UseIdentity();
            app.UseMvc();
        }

        private void ConfigureSecurityServices(IServiceCollection services) {

            services.AddIdentity<User, ApplicationRole>(options=>{
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                // Lockout settings
                /*options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;*/

                // User settings
                //options.User.RequireUniqueEmail = true;
                
            })
            .AddDefaultTokenProviders();
            services.AddTransient<IUserStore<User>, GenericAirways.WebApi.UserStore<User>>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            
            services.AddSingleton<IJwtFactory<User>, JwtFactory>();

            /*services.AddAuthorization(o => {
                o.AddPolicy("apipolicy", b =>
                {
                    b.RequireAuthenticatedUser();
                    b.RequireClaim(System.IdentityModel.Claims.ClaimTypes.Role, "Access.Api");
                    b.AuthenticationSchemes = new List<string>{JwtBearerDefaults.AuthenticationScheme};
                });
            });*/
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.SecurityTokenValidators.Clear();
                jwtBearerOptions.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    /*ValidateActor = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,*/
                    ValidateAudience = true,
                    ValidIssuer = Configuration.GetSection("AppConfiguration")["Issuer"],
                    ValidAudience = Configuration.GetSection("AppConfiguration")["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppConfiguration")["SigningKey"]))
                };
                /*jwtBearerOptions.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        Console.WriteLine("OnAuthenticationFailed");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("OnTokenValidated: " + 
                            context.SecurityToken);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine("OnChallenge: "+context.Error );
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine("OnMessageReceived: "+context);
                        return Task.CompletedTask;
                    }

                };*/
            });

            services.AddAuthentication().AddFacebook(facebookOptions => {
                facebookOptions.AppId = "";//Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = "";// Configuration["Authentication:Facebook:AppSecret"];
            });


            /*services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });*/

        }
    }

    /*public class AddAuthorizeFiltersControllerConvention : Microsoft.AspNetCore.Mvc.ApplicationModels.IControllerModelConvention {
        public void Apply(Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel controller)
        {
            controller.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter("apipolicy"));
        }
    }*/

}
