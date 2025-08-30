using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities.Enums;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;

namespace WorkforceManagement.Services.Services
{
    public class SendDailyReminderEmailsService : IHostedService
    {
        private readonly IServiceProvider _services;
        private Timer _timer;

        public SendDailyReminderEmailsService(IServiceProvider services)
        {
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
			TimeSpan timeInterval = TimeSpan.FromHours(24);

            var nextRunTime = DateTime.Today.AddDays(1).AddHours(1);
            var currentTime = DateTime.Now;
            var firstInterval = nextRunTime.Subtract(currentTime);

			Action action = () =>
			{
                var sheduledTask = Task.Delay(firstInterval);
                sheduledTask.Wait();

                SendReminderEmailsToLeads(null);

				_timer = new Timer(
					SendReminderEmailsToLeads,
					null,
					TimeSpan.Zero,
					timeInterval
				);
			};

			Task.Run(action);
			return Task.CompletedTask;
		}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask; throw new NotImplementedException();
        }

        private void SendReminderEmailsToLeads(object state)
        {
			using (var scope = _services.CreateScope())
			{
				var scopedTimeOffRequestService =
				scope.ServiceProvider
					.GetRequiredService<ITimeOffRequestService>();

                var scopedMailService =
                scope.ServiceProvider
                    .GetRequiredService<IMailService>();

                var pendingRequests = scopedTimeOffRequestService.GetAllPendingTimeOffRequests();

                foreach (var request in pendingRequests)
                {
                    foreach (var approval in request.Approvals)
                    {
                        if (approval.IsApproved == false)
                        {
                            scopedMailService.SendEmail(request.Requester, approval.Approver.Email, request, EmailType.Default);
                        }
                    }
                }
			}
		}
    }
}
