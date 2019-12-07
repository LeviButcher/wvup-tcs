using System;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
using Helpers;

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


            if (!Environment.IsDevelopment())
            {
                var bannerAuth = Configuration["Banner:User"] + ":" + Configuration["Banner:Password"];
                var encodedAuth = bannerAuth.ToBase64();


                services.AddHttpClient("banner", c =>
                {
                    c.BaseAddress = new Uri(Configuration["Banner:api"]);
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
                cronExpression: "0 57 23 * * ?"));    // runs at 11:57pm every night

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<TCSContext>(options =>
               options.UseSqlServer(Configuration["DB:connectionString"]));

            services.AddScoped<IClassTourRepo, ClassTourRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ISignInRepo, SignInRepo>();
            services.AddScoped<IReportsRepo, ReportsRepo>();
            services.AddScoped<IReasonRepo, ReasonRepo>();
            services.AddScoped<IScheduleRepo, ScheduleRepo>();
            services.AddScoped<ILookupRepo, LookupRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, TCSContext db, IUserRepo repo)
        {
            DbInitializer.InitializeData(db, repo, Environment);
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseErrorWrapping();

            app.UseCors(AllowAnywhere);

            app.UseAuthentication();

            app.UseMvc();

        }
    }
}