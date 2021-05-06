using System;
using AutoMapper;
using CW.ClientAPI.Services.General;
using CW.ClientAPI.Models;
using CW.ClientAPI.Services.MsgContent;
using CW.ClientLibrary.DbContexts;
using CW.ClientLibrary.Services.MessageTracker;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CW.ClientAPI.Extentions;
using System.Net;
using Newtonsoft.Json.Serialization;

namespace CW.ClientAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.RespectBrowserAcceptHeader = true;

            }).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            })
            .AddXmlDataContractSerializerFormatters()
            .AddNewtonsoftJson();


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //ToDo: set up all the different environments prod/train

            string uriString = string.Empty;
            string dbString = string.Empty;
            string accessKey = string.Empty;
            string secretKey = string.Empty;

            //If development environment
            if (_env.IsDevelopment())
            {
                uriString = Configuration.GetSection("DevCaseWorthySettings:AppEndPoint:Train").Value;
                dbString = Configuration.GetSection("DevCaseWorthySettings:Data:FIX_DEV").Value;
                accessKey = Configuration.GetSection("DevCaseWorthySettings:SecurityAccess:AccessKey").Value;
                secretKey = Configuration.GetSection("DevCaseWorthySettings:SecurityAccess:SecretKey").Value;

            }
            else if (_env.IsProduction())
            {
                uriString = Configuration.GetSection("ProdCaseWorthySettings:AppEndPoint:Production").Value;
                dbString = Configuration.GetSection("ProdCaseWorthySettings:Data:FIX").Value;
                accessKey = Configuration.GetSection("ProdCaseWorthySettings:SecurityAccess:AccessKey").Value;
                secretKey = Configuration.GetSection("ProdCaseWorthySettings:SecurityAccess:SecretKey").Value;
            }
            else if (_env.IsStaging())
            {
                uriString = Configuration.GetSection("ETLCaseWorthySettings:AppEndPoint:ETL").Value;
                dbString = Configuration.GetSection("ETLCaseWorthySettings:Data:FIX_DEV").Value;
                accessKey = Configuration.GetSection("ETLCaseWorthySettings:SecurityAccess:AccessKey").Value;
                secretKey = Configuration.GetSection("ETLCaseWorthySettings:SecurityAccess:SecretKey").Value;
            }

                //functionality to inject IOptions<T?
                services.AddOptions();

            //get the email server setting in app settings file
            services.Configure<EmailSettingsModel>(Configuration.GetSection("EmailSettings"));
            //services.Configure<CaseWorthySettingsModel>(Configuration.GetSection("CaseWorthySettings"));
           
            // then apply a configuration function
            services.Configure<CaseWorthySettingsModel>(options =>
            {
                // overwrite previous values
                options.AccessKey =accessKey;  
                options.SecretKey= secretKey;  
                options.BaseAddress = uriString;
                
               
            });

            services.AddDbContext<FixContext>(options =>
            {
                options.UseSqlServer(dbString,
                sqlServerOptionsAction: sqlOptions =>
                    {
                        //retry option if connection fail to sql server
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                            );
                    });

            });

           

            #region Hangfire
            // Add Hangfire services.
            services.AddHangfire(option => option
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(dbString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                })); ;

            // Add the processing server as IHostedService
            services.AddHangfireServer();
            #endregion

            #region Add Scope
            services.AddScoped<IInterval, Interval>();
            services.AddScoped<ITracker, Tracker>();
            services.AddScoped<IContent, Content>();
            services.AddScoped<IFortuneContent, FortuneContent>();
            services.AddScoped<IClientImage, ClientImage>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICaseWorthyService, CaseWorthyService>();

            //services.AddScoped<ClientContent>();
            services.AddScoped<ClientImageContent>();
             services.AddScoped<EntityTrackerContent>();
            services.AddScoped<EntityContent>();

            #endregion


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                                IWebHostEnvironment env,
                                ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();

            }
            else if (env.IsStaging())
            {
                logger.LogInformation("In Staging environment");
                app.UseExceptionHandler("/Error");
                app.UseHsts();

            }
            else if (env.IsProduction())
            {
                logger.LogInformation("In Production environment");
                app.UseExceptionHandler("/Error");
                app.UseHsts();

            }
            else
            {
                logger.LogInformation("Not Devolpment, Staging or Production environment");
            }

            //add middle ware for errors
            app.UseGlobalExceptionMiddleware();

            //app.UseHttpsRedirection();

            app.UseRouting();

         
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Hangfire dashboard
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilterHangFire() }
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                HeartbeatInterval = new System.TimeSpan(0, 1, 0),
                ServerCheckInterval = new System.TimeSpan(0, 1, 0),
                SchedulePollingInterval = new System.TimeSpan(0, 1, 0)
            });
        }
    }
}
