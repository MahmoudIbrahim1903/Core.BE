﻿//using MassTransit;
//using Microsoft.Extensions.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Emeint.Core.BE.InterCommunication.Concretes
//{
//    public class BusService : Microsoft.Extensions.Hosting.IHostedService
//    {
//        private readonly IBusControl _busControl;

//        public BusService(IBusControl busControl)
//        {
//            _busControl = busControl;
//        }

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            return _busControl.StartAsync(cancellationToken);
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            return _busControl.StopAsync(cancellationToken);
//        }
//    }
//}
