using neeksdk.Scripts.Infrastructure.Services.EventService;
using UnityEngine;

namespace neeksdk.Scripts.Test
{
    public class AddTestEvent : MonoBehaviour
    {
        public EventService EventService;
        public string type = "levelStart";
        public string data = "level:3";

        public void SendTestEvent() =>
            EventService.TrackEvent(type, data);
    }
}
