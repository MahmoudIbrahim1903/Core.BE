using Emeint.Core.BE.API.Infrastructure.HostedServices;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes
{
    public class BusBackgroundService : BackgroundService
    {
        IBusControl _busControl;
        ILogger<BusBackgroundService> _logger;
        Timer _timer;
        IConfiguration _configuration;
        public BusBackgroundService(IBusControl busControl, ILogger<BusBackgroundService> logger, IConfiguration configuration)
        {
            _busControl = busControl;
            _logger = logger;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string intervalString = _configuration["StartBusTimerIntervalInMinutes"];
            if (string.IsNullOrEmpty(intervalString))
                intervalString = "120";

            int interval = Convert.ToInt32(intervalString);
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(interval));
        }
        private void DoWork(object state)
        {
            try
            {
                _busControl.Start();
                _logger.LogInformation("bus started");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("catch start bus");
                _logger.LogInformation($"start bus ex {ex.Message}");
            }
        }
    }
}
