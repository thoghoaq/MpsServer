using Hangfire;
using Mps.Api.Workers;

namespace Mps.Api
{
    public class StartWorker()
    {
        public static void Start()
        {
            RecurringJob.AddOrUpdate<MonthlyRequestPayout>("WeeklyRequestPayout", x => x.Process(), Cron.Weekly(DayOfWeek.Monday));
            RecurringJob.AddOrUpdate<MonthlyPayout>("WeeklyPayout", x => x.Process(), Cron.Weekly(DayOfWeek.Tuesday));
        }
    }
}
