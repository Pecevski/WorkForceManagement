using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkforceManagement.Services.Contracts;

namespace WorkforceManagement.Services.Services
{
    public class ScheduleRemoveTimeOffRequestsService : IHostedService
    {
        private Timer _timer;
        public IServiceProvider _services;

        public ScheduleRemoveTimeOffRequestsService(IServiceProvider services)
        {
			_services = services;
		}

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var dateNow = DateTime.UtcNow;
            var nextRunTime = DateTime.Today.AddMinutes(1).AddSeconds(1);
            var secondInterval = dateNow.Subtract(nextRunTime);

            TimeSpan newInterval = TimeSpan.FromHours(24);

            Action action = () =>
			{
                var task = Task.Delay(secondInterval);
                task.Wait();

                _timer = new Timer(
					RemoveTimeOffRequestAfterSixMonths,
					null,
					TimeSpan.Zero,
					newInterval
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

		private void RemoveTimeOffRequestAfterSixMonths(object state)
		{
			using (var scope = _services.CreateScope())
			{
				var scopedService =
				scope.ServiceProvider
					.GetRequiredService<ITimeOffRequestService>();

				scopedService.DeleteTimeOffRequestAfterSixMonths().GetAwaiter().GetResult();
			}
		}
	}
}
