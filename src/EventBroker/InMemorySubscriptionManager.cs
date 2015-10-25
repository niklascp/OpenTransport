using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EventBroker
{
    public class InMemorySubscriptionManager : ISubscriptionManager
    {
        private object syncRoot;
        private ConcurrentDictionary<string, ClientState> clients;
        private ConcurrentDictionary<string, TopicState> topics;

        public InMemorySubscriptionManager()
        {
            syncRoot = new object();
            clients = new ConcurrentDictionary<string, ClientState>(StringComparer.OrdinalIgnoreCase);
            topics = new ConcurrentDictionary<string, TopicState>(StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetTopics()
        {
            return topics.Keys;
        }

        public IEnumerable<string> GetClientsForTopic(Topic topic)
        {
            if (!topics.ContainsKey(topic))
                return new HashSet<string>();

            lock (syncRoot)
            {
                return topics[topic].Clients;
            }
        }

        public void Subscribe(string client, Topic topic, Action<Topic, string> handler)
        {
            var clientState = clients.GetOrAdd(client, _ => new ClientState(client));
            var topicState = topics.GetOrAdd(topic, _ => new TopicState(topic));
            
            lock (syncRoot)
            {
                topics[topic].Add(client, handler);
            }
        }

        public void Unsubscribe(string clientId, Topic topic)
        {
            TopicState topicState;

            if (topics.TryGetValue(topic, out topicState))
            {
                lock (syncRoot)
                {
                    topicState.Remove(clientId);

                    /* This is the last subscriber on this topic, remove the topic. */
                    if (topicState.Clients.Count() == 0)
                        topics.TryRemove(topic, out topicState);

                    if (topicState.Clients.Count() != 0)
                        throw new InvalidOperationException("Subscribed topic was removed.");
                }
            }
        }

        public void Publish(Topic topic, string message)
        {
            TopicState topicState;

            if (topics.TryGetValue(topic, out topicState))
            {
                topicState.Publish(message);
            }            
        }

        private class ClientState
        {
            public string client;
            public DateTime LastDateTime;

            public ClientState(string client)
            {
                this.client = client;
            }
        }

        private class TopicState
        {
            private Topic topic;
            private Dictionary<string, Action<Topic, string>> clients;
            private Action<Topic, string> handler;

            public TopicState(Topic topic)
            {
                this.topic = topic;
                clients = new Dictionary<string, Action<Topic, string>>(StringComparer.OrdinalIgnoreCase);
            }

            internal void Publish(string message)
            {
                if (handler != null)
                    handler(topic, message);                        
            }

            public IEnumerable<string> Clients
            {
                get { return clients.Keys; }
            }

            public void Add(string client, Action<Topic, string> value)
            {
                clients.Add(client, value);
                handler += value;
            }

            public void Remove(string client)
            {
                Action<Topic, string> value;
                if (clients.TryGetValue(client, out value))
                {
                    clients.Remove(client);
                    handler -= value;
                }                
            }
        }

    }
}
