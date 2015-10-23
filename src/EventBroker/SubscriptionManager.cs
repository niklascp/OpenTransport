using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace EventBroker
{
    public class InMemorySubscriptionManager<T> : ISubscriptionManager<T>
    {
        private object syncRoot;
        private BlockingCollection<T> queue;
        private ConcurrentDictionary<string, ClientState> clients;
        private ConcurrentDictionary<string, TopicState> topics;

        public InMemorySubscriptionManager()
        {
            syncRoot = new object();
            queue = new BlockingCollection<T>();
            clients = new ConcurrentDictionary<string, ClientState>(StringComparer.OrdinalIgnoreCase);
            topics = new ConcurrentDictionary<string, TopicState>(StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetTopics()
        {
            return topics.Keys;
        }

        public IEnumerable<string> GetClients(string topic)
        {
            if (!topics.ContainsKey(topic))
                return new HashSet<string>();

            lock (syncRoot)
            {
                return topics[topic].Clients;
            }
        }

        public void Subscribe(string clientId, string topic)
        {
            var clientState = clients.GetOrAdd(clientId, _ => new ClientState());
            var topicState = topics.GetOrAdd(topic, _ => new TopicState());
            
            lock (syncRoot)
            {
                topics[topic].Clients.Add(clientId);
            }
        }

        public void Unsubscribe(string clientId, string topic)
        {
            TopicState topicState;

            if (topics.TryGetValue(clientId, out topicState))
            {
                lock (syncRoot)
                {
                    topicState.Clients.Remove(clientId);

                    /* This is the last subscriber on this topic, remove the topic. */
                    if (topicState.Clients.Count == 0)
                        topics.TryRemove(topic, out topicState);

                    if (topicState.Clients.Count != 0)
                        throw new InvalidOperationException("Should not happen!");
                }
            }
        }

        public bool TryTake(string clientId, out T message, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return queue.TryTake(out message, millisecondsTimeout, cancellationToken);
        }

        public void Publish(T message, params string[] topics)
        {
            /* TODO: Pub message only to clients with matching topic */
            queue.Add(message);
        }

        private class ClientState
        {
            DateTime LastDateTime;
        }

        private class TopicState
        {
            public TopicState()
            {
                Clients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            public ISet<string> Clients { get; private set; }
        }

    }
}
