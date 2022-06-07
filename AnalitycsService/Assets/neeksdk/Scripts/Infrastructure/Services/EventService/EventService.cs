using UnityEngine;

namespace neeksdk.Scripts.Infrastructure.Services.EventService
{
    public class EventService : MonoBehaviour
    {
        [SerializeField] private string serverUrl;
        [SerializeField] private float cooldownBeforeSend;
        
        public void TrackEvent(string type, string data)
        {
            
        }
    }
}
