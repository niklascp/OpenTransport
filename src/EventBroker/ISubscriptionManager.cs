﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace EventBroker
{
    public interface ISubscriptionManager
    {
        IEnumerable<string> GetClientsForTopic(Topic topic);
        IEnumerable<string> GetTopics();
        void Subscribe(string clientId, Topic topic, Action<Topic, string> handler);
        void Unsubscribe(string clientId, Topic topic);
        void Publish(Topic topic, string message);
    }
}