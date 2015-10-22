using System;
using System.Collections.Generic;

namespace EventBroker
{
    public class InMemorySubscriptionManager : ISubscriptionManager
    {
        private object syncRoot;
        private IDictionary<string, ClientState> clients;
        private IDictionary<string, TopicState> topics;

        public InMemorySubscriptionManager()
        {
            syncRoot = new object();
            topics = new Dictionary<string, TopicState>(StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetTopics()
        {
            return topics.Keys;
        }

        public IEnumerable<string> GetClients(string topic)
        {
            if (!topics.ContainsKey(topic))
                return new HashSet<string>();

            return topics[topic].Clients;
        }

        public void Subscribe(string clientId, string topic)
        {
            if (!clients.ContainsKey(clientId))
                AddClient(clientId);

            if (!topics.ContainsKey(topic))
                AddTopic(topic);

            lock (syncRoot)
            {
                topics[topic].Clients.Add(clientId);
            }
        }

        public void Unsubscribe(string clientId, string topic)
        {
            if (topics.ContainsKey(topic))
            {
                lock (syncRoot)
                {
                    if (topics.ContainsKey(clientId))
                    {
                        topics[topic].Clients.Remove(clientId);

                        if (topics[topic].Clients.Count == 0)
                            topics.Remove(topic);
                    }
                }
            }
        }

        private void AddClient(string clientId)
        {
            lock (syncRoot)
            {
                if (!clients.ContainsKey(clientId))
                    clients.Add(clientId, new ClientState { });
            }
        }

        private void AddTopic(string topic)
        {
            lock (syncRoot)
            {
                if (topics.ContainsKey(topic))
                    return;

                topics.Add(topic, new TopicState { });
            }
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
