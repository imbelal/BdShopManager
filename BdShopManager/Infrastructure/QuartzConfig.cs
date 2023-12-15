using Application.CronJobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Infrastructure
{
    public static class QuartzConfig
    {
        public static void ConfigureJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                // Uncomment this to run the cron test job.
                //var jobKey = new JobKey("TestCronJob");
                //q.AddJob<TestCronJob>(opts => opts.WithIdentity(jobKey));
                //q.AddTrigger(opts => opts
                //    .ForJob(jobKey)
                //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));
                //.WithIdentity("DemoJob-trigger")
                //.WithCronSchedule("0/5 * * * * ?"));

                q.AddJobListener<CronJobListener>();

            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}
