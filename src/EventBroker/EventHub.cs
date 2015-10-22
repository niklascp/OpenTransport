using Microsoft.AspNet.SignalR;
using System.Collections.Generic;

namespace EventBroker
{
    public class EventHub : Hub
    {
        ISubscriptionManager subscriptionManager;

        public EventHub(ISubscriptionManager subscriptionManager)
        {
            this.subscriptionManager = subscriptionManager;
        }

        public void Publish(object message, string[] topics)
        {
            /* TODO: Pub message in work queue */
            var targetTopics = subscriptionManager.GetTopics();

            var clients = new HashSet<string>();

            foreach (var targetTopic in targetTopics)
            {
                foreach (var topic in topics)

                    if (string.Equals(targetTopic, topic))
                    {
                        clients.UnionWith(subscriptionManager.GetClients(targetTopic));
                    }
            }

            foreach (var client in clients)
                Clients.Client(client).vehicleEvent(message);
        }

        public void Subscribe(string[] topics)
        {
            foreach (var topic in topics)
                subscriptionManager.Subscribe(Context.ConnectionId, topic);
        }
    }
}
