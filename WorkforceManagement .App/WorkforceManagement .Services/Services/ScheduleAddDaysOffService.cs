using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkforceManagement.Services.Contracts;

namespace WorkforceManagement.Services.Services
{
    public class ScheduleAddDaysOffService : IHostedService
    {
        private Timer _timer;
		public IServiceProvider _services;

		public ScheduleAddDaysOffService(IServiceProvider services)
        {
			_services = services;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

			var dateNow = DateTime.UtcNow;
			var scheduledDate = DateTime.DaysInMonth(dateNow.Year, dateNow.Month);

			var nextRunTime = DateTime.Today.AddDays(30);
			var firstInterval = dateNow.Subtract(nextRunTime);

			if (scheduledDate != 1)
			{
				scheduledDate -= dateNow.Day;
			}

			TimeSpan interval = TimeSpan.FromDays(scheduledDate);

			Action action = () =>
			{
				var task = Task.Delay(firstInterval);
				task.Wait();

				_timer = new Timer(
					SetAutomaticSumOfPaidDaysOff,
					null,
					TimeSpan.Zero,
					interval
				);
			};

			// no need to await this call here because this task is scheduled to run much later.
			Task.Run(action);
			return Task.CompletedTask;
		}

        public Task StopAsync(CancellationToken cancellationToken)
        {
			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}
		private void SetAutomaticSumOfPaidDaysOff(object state)
        {
			using (var scope = _services.CreateScope())
            {
				var scopedService =
				scope.ServiceProvider
					.GetRequiredService<ITimeOffRequestService>();

				scopedService.SetDaysOffForNewYear().GetAwaiter().GetResult();
			}
		}
    }
}