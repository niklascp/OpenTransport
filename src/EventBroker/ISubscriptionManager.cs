using System.Collections.Generic;
using System.Threading;

namespace EventBroker
{
    public interface ISubscriptionManager<T>
    {
        IEnumerable<string> GetClients(string topic);
        IEnumerable<string> GetTopics();
        void Publish(T message, params string[] topics);
        void Subscribe(string clientId, string topic);
        void Unsubscribe(string clientId, string topic);
        bool TryTake(string clientId, out T message, int millisecondsTimeout, CancellationToken cancellationToken);
    }
}