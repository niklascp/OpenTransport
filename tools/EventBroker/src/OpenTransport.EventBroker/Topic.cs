using System;
using System.Text;

namespace OpenTransport.EventBroker
{
    public struct Topic
    {
        private readonly byte[] value;

        public Topic(byte[] value)
        {
            this.value = value;
        }

        /// <summary>
        /// Obtains a string representation of the topic
        /// </summary>
        public override string ToString()
        {
            return ((string)this) ?? "(null)";
        }

        /// <summary>
        /// Create a topic from a String
        /// </summary>
        public static implicit operator Topic(string name)
        {
            if (name == null) return default(Topic);
            return new Topic(Encoding.UTF8.GetBytes(name));
        }

        /// <summary>
        /// Create a topic from a Byte[]
        /// </summary>
        public static implicit operator Topic(byte[] key)
        {
            if (key == null) return default(Topic);
            return new Topic(key);
        }

        /// <summary>
        /// Obtain the topic name as a Byte[]
        /// </summary>
        public static implicit operator byte[] (Topic topic)
        {
            return topic.value;
        }

        /// <summary>
        /// Obtain the topic name as a String
        /// </summary>
        public static implicit operator string (Topic topic)
        {
            var arr = topic.value;
            if (arr == null) return null;
            try
            {
                return Encoding.UTF8.GetString(arr);
            }
            catch
            {
                return BitConverter.ToString(arr);
            }
        }
    }
}
