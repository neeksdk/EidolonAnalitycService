using neeksdk.Scripts.Infrastructure.Services.EventService;
using UnityEngine;

namespace neeksdk.Scripts.Test
{
    public class AddTestEvent : MonoBehaviour
    {
        public EventService EventService;

        public void SendTestEvent() =>
            EventService.TrackEvent("levelStart", "level:3");
    }
}
