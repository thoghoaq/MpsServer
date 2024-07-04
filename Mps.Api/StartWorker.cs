using Hangfire;
using Mps.Api.Workers;

namespace Mps.Api
{
    public class StartWorker()
    {
        public static void Start()
        {
            RecurringJob.AddOrUpdate<MonthlyRequestPayout>("MonthlyRequestPayout", x => x.Process(), Cron.Monthly(1));
            RecurringJob.AddOrUpdate<MonthlyPayout>("MonthlyPayout", x => x.Process(), Cron.Monthly(5));
        }
    }
}
