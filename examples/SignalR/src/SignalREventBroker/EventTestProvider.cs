using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using OpenTransport.EventBroker;
using Newtonsoft.Json;

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

                subscriptionManager.Publish("line.300s", JsonConvert.SerializeObject(new {
                    time = DateTime.Now,
                    line = new { number = 300, designation = "300S" },
                }));

                Task.Delay(5000).Wait();
            }
        }
    }
}
