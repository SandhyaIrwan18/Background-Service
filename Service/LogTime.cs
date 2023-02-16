using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;

namespace _BackgroundService.Service
{
    public class LogTime : BackgroundService, IHostedService
    {
        private CrontabSchedule schedule;
        private DateTime nextRun;
        private readonly IServiceScopeFactory serviceScopeFactory;
        // Active After 10 Second
        private string timer => "*/10 * * * * *";

        public LogTime(IServiceScopeFactory serviceScopeFactory)
        {
            schedule = CrontabSchedule.Parse(timer, new CrontabSchedule.ParseOptions
            {
                IncludingSeconds = true
            });
            nextRun = schedule.GetNextOccurrence(DateTime.Now);
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var nextrun = schedule.GetNextOccurrence(now);
                if (now > nextRun)
                {
                    Process();
                    nextRun = schedule.GetNextOccurrence(DateTime.Now);
                }
                // Active Every 10 Second
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void Process()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    Console.WriteLine("Waktu Sekarang = " + DateTime.Now);
                }
            }
            catch (Exception x)
            {
                Console.Write(x.Message);
            }
        }
    }
}