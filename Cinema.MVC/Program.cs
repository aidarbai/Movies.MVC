using Cinema.BLL.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Cinema.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureServices((hostContext, services) =>
             {
                 services.AddQuartz(q =>
                 {
                     q.UseMicrosoftDependencyInjectionJobFactory();

                     var jobKey = new JobKey("UploadNewMoviesJob");

                     q.AddJob<UploadNewMoviesJob>(opts => opts.WithIdentity(jobKey));

                     q.AddTrigger(opts => opts
                         .ForJob(jobKey)
                         .WithIdentity("UploadNewMoviesJob-trigger")
                         .WithCronSchedule("0 0 6 * * ?"));
                 });

                 services.AddQuartzHostedService(
                     q => q.WaitForJobsToComplete = true);
             })
            .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 config.AddJsonFile("Settings\\config.json", optional: false, reloadOnChange: false);
                 config.AddJsonFile("Settings\\googleapi.json", optional: false, reloadOnChange: false);
                 config.AddJsonFile("Settings\\superadmin.json", optional: false, reloadOnChange: false);
             })
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             });
    }
}
