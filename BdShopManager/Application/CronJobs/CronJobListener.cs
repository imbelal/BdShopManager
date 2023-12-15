using Quartz;

namespace Application.CronJobs
{
    public class CronJobListener : IJobListener
    {
        public string Name => "CronJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // This method is called when a job's execution is vetoed (e.g., by a trigger listener)
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // This method is called when a job is about to be executed
            // You can perform monitoring actions here
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            // This method is called after a job has been executed
            // You can perform monitoring actions here
            return Task.CompletedTask;
        }
    }
}
