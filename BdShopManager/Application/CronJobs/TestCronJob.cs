using Quartz;

namespace Application.CronJobs
{
    public class TestCronJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // Your job logic goes here
            Console.WriteLine("Job executed at: " + DateTime.Now);
        }
    }
}
