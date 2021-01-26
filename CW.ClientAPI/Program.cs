using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace CW.ClientAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: exceptions
                logger.Error(ex, $"Stopped program because of exception at {DateTime.UtcNow}");
                throw;
            }
            finally
            {
                //stop internal timers/threads before app exit
                NLog.LogManager.Shutdown();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return Host.CreateDefaultBuilder(args)
                 .ConfigureLogging((context, logging) =>
                  {
                      logging.ClearProviders();
                      logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); //Nlog
                  })
                 .UseNLog() //NLog used for dependency injection
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
