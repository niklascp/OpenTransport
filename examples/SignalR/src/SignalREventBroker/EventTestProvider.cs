﻿using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using OpenTransport.EventBroker;

namespace EventBroker
{
    public class EventTestProvider 
    {
        ISubscriptionManager subscriptionManager;
        Task task;
        CancellationToken cancellationToken;

        public EventTestProvider(ISubscriptionManager subscriptionManager)
        {
            task = new Task(Loop);
            task.Start();

            this.subscriptionManager = subscriptionManager;
        }

        private void Loop()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                

                Task.Delay(5000);
            }
        }
    }
}