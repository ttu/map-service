using System;

namespace MapApi
{
    // TODO: Find small MessageBus

    public class MessageHub
    {
        public event EventHandler<Tuple<string, string>> Updated;

        public void DataUpdated(string key, string data)
        {
            Updated?.Invoke(this, Tuple.Create(key, data));
        }
    }
}