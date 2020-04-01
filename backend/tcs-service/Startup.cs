#pragma warning disable 1591

using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using tcs_service.EF;
using tcs_service.Helpers;
using tcs_service.Repos;
using tcs_service.Repos.Interfaces;
using tcs_service.Services;
using tcs_service.Services.Interfaces;
using tcs_service.Services.ScheduledTasks;
using tcs_service.UnitOfWorks;
using tcs_service.UnitOfWorks.Interfaces;

namespace tcs_service
{
    public class Startup
    {
        readonly string AllowAnywhere = "_AllowAnywhere";
        public IConfiguration Configuration { get; }
        private readonly IHostingEnvironment Environment;

        public Startup(IConfiguration configuration, IHostingEnvironment Environment)
        {
            this.Environment = Environment;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbConfig = Configuration.GetSection("Db").Get<DbConfig>();
            var bannerConfig = Configuration.GetSection("Banner").Get<BannerConfig>();

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAnywhere,
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .WithExposedHeaders("*", "Total-Pages", "Total-Records", "Current-Page", "Next", "Prev")
                            .AllowAnyMethod();
                    });
            });

            services.AddAutoMapper(typeof(Startup));
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            if (Environment.IsProduction())
            {
                var bannerAuth = bannerConfig.User + ":" + bannerConfig.Password;
                var encodedAuth = bannerAuth.ToBase64();

                services.AddHttpClient("banner", c =>
                {
                    c.BaseAddress = new Uri(bannerConfig.Api);
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
                });
                services.AddScoped<IBannerService, LiveBannerService>();
            }
            else
            {
                services.AddScoped<IBannerService, MockBannerService>();
            }

            // Add Quartz services
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<JobRunner>();
            services.AddHostedService<QuartzHostedService>();

            // Add our job
            services.AddScoped<StudentSignOutJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(StudentSignOutJob),
                cronExpression: "0 57 23 * * ?")); // runs at 11:57pm every night

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<TCSContext>(options =>
               options.UseNpgsql(dbConfig.ConnectionString));

            services.AddScoped<IClassTourRepo, ClassTourRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IReasonRepo, ReasonRepo>();
            services.AddScoped<ISessionRepo, SessionRepo>();
            services.AddScoped<IPersonRepo, PersonRepo>();
            services.AddScoped<IScheduleRepo, ScheduleRepo>();
            services.AddScoped<IClassRepo, ClassRepo>();
            services.AddScoped<ISemesterRepo, SemesterRepo>();
            services.AddScoped<IDepartmentRepo, DepartmentRepo>();
            services.AddScoped<ISessionClassRepo, SessionClassRepo>();
            services.AddScoped<ISessionReasonRepo, SessionReasonRepo>();
            services.AddScoped<IUnitOfWorkPerson, UnitOfWorkPerson>();
            services.AddScoped<ICSVParser<CSVSessionUpload>, CSVSessionUploadParser>();
            services.AddScoped<IUnitOfWorkSession, UnitOfWorkSession>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TCS API", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "tcs-service.xml");
                c.IncludeXmlComments(filePath);
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, TCSContext db, IUserRepo repo)
        {
            DbInitializer.InitializeData(db, repo, Environment);

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "TCS API V1");
                c.RoutePrefix = "api/swagger";
            });

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            app.UseErrorWrapping();

            app.UseCors(AllowAnywhere);

            app.UseAuthentication();

            app.UseMvc();

        }
    }
}