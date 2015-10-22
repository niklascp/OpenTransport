using System.Collections.Generic;

namespace EventBroker
{
    public interface ISubscriptionManager
    {
        IEnumerable<string> GetClients(string topic);
        IEnumerable<string> GetTopics();
        void Subscribe(string clientId, string topic);
        void Unsubscribe(string clientId, string topic);
    }
}