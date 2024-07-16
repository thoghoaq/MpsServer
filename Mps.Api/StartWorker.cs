using Hangfire;
using Mps.Api.Workers;
using Mps.Domain.Enums;

namespace Mps.Api
{
    public class StartWorker()
    {
        public static void Start()
        {
            RecurringJob.AddOrUpdate<CustomRequestPayout>("Day1RequestPayout", x => x.Process(PayoutDate.Day1), Cron.Monthly(1));
            RecurringJob.AddOrUpdate<CustomPayout>("Day2Payout", x => x.Process(PayoutDate.Day1), Cron.Monthly(2));

            RecurringJob.AddOrUpdate<CustomRequestPayout>("Day8RequestPayout", x => x.Process(PayoutDate.Day8), Cron.Monthly(8));
            RecurringJob.AddOrUpdate<CustomPayout>("Day9Payout", x => x.Process(PayoutDate.Day8), Cron.Monthly(9));

            RecurringJob.AddOrUpdate<CustomRequestPayout>("Day15RequestPayout", x => x.Process(PayoutDate.Day15), Cron.Monthly(15));
            RecurringJob.AddOrUpdate<CustomPayout>("Day16Payout", x => x.Process(PayoutDate.Day15), Cron.Monthly(16));

            RecurringJob.AddOrUpdate<CustomRequestPayout>("Day22RequestPayout", x => x.Process(PayoutDate.Day22), Cron.Monthly(22));
            RecurringJob.AddOrUpdate<CustomPayout>("Day23Payout", x => x.Process(PayoutDate.Day22), Cron.Monthly(23));
        }
    }
}
