using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Seeding;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;
using WorkforceManagement.Services.Services;
using WorkforceManagement.WebApi.Configuration;
using WorkforceManagement.WebApi.Policies;

namespace WorkforceManagement.WebApi
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            // EF Identity
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkforceManagement.WebApi", Version = "v1" });

                // Adds the authorize button in swagger UI 
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Uses the token from the authorize input and sends it as a header
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

            });

            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddTransient<ITimeOffRequestService, TimeOffRequestService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IDateService, DateService>();
            services.AddTransient<IAuthorizationHandler, TeamLeaderHandler>();

            services.AddHostedService<SendDailyReminderEmailsService>();        
            services.AddHostedService<ScheduleAddDaysOffService>();
            services.AddHostedService<ScheduleRemoveTimeOffRequestsService>();

            // IdentityServer
            var builder = services.AddIdentityServer((options) =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                                   .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                                   .AddInMemoryClients(IdentityConfig.Clients);

            builder.AddDeveloperSigningCredential();
            builder.AddResourceOwnerValidator<PasswordValidator>();

            // Authentication
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("IsTeamLeadOrAdmin", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.Requirements.Add(new TeamLeaderRequirement(1, GlobalConstants.AdministratorRoleName));
                    });
                })
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })

                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetValue<string>("JwtBearer:Authority");
                    options.Audience = Configuration.GetValue<string>("JwtBearer:Audience");
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkforceManagement.WebApi v1"));
            }

            // Creates new database
            using (IServiceScope serviceScope = app.ApplicationServices.CreateScope())
            {
                ApplicationDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();

                new DataSeeder(new ISeeder[] { new RolesSeeder(), new UsersSeeder() }).SeedData(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            app.UseIdentityServer();

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
