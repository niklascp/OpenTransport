using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using OpenTransport.EventBroker;

namespace EventBroker
{
    public class EventHub : Hub
    {
        EventTestProvider e;
        CancellationToken cancellationToken = CancellationToken.None;
        ISubscriptionManager subscriptionManager;

        public EventHub(ISubscriptionManager subscriptionManager, EventTestProvider e)
        {
            this.subscriptionManager = subscriptionManager;
            this.e = e;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public void Publish(string topic, string message)
        {
            subscriptionManager.Publish(topic, message);
        }

        public void Subscribe(string topic)
        {
            var clientId = Context.ConnectionId;

            subscriptionManager.Subscribe(Context.ConnectionId, topic, (t, m) =>
            {
                Clients.Client(clientId).vehicleEvent(m);
            });
        }
    }
}
