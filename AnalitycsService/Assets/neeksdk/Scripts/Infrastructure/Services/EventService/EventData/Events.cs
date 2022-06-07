using System;
using System.Collections.Generic;

namespace neeksdk.Scripts.Infrastructure.Services.EventService.EventData
{
    [Serializable]
    public class Events
    {
        public List<EventData> events = new List<EventData>();
    }

    [Serializable]
    public struct EventData
    {
        public string type;
        public string data;
    }
}
