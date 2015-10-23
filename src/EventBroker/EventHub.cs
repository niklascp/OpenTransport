using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBroker
{
    public class EventHub : Hub
    {
        CancellationToken cancellationToken = CancellationToken.None;
        ISubscriptionManager<string> subscriptionManager;

        public EventHub(ISubscriptionManager<string> subscriptionManager)
        {
            this.subscriptionManager = subscriptionManager;            
        }

        public override Task OnConnected()
        {
            Task.Run(() => Deliver(Context.ConnectionId));

            return base.OnConnected();
        }

        private void Deliver(string clientId)
        {
            while (true)
            {
                Clients.All.deliver(clientId);

                if (cancellationToken.IsCancellationRequested)
                    return;

                string message;
                if (subscriptionManager.TryTake(clientId, out message, 1000, cancellationToken))
                {
                    Clients.Client(clientId).vehicleEvent(message);
                }
            }
        }

        public void Publish(string message, string[] topics)
        {
            subscriptionManager.Publish(message, topics);
        }

        public void Subscribe(string[] topics)
        {
            foreach (var topic in topics)
                subscriptionManager.Subscribe(Context.ConnectionId, topic);
        }
    }
}
