using neeksdk.Scripts.Infrastructure.Services.SaveLoadService;
using UnityEngine;

namespace neeksdk.Scripts.Infrastructure.Services.EventService
{
    public class EventService : MonoBehaviour
    {
        [SerializeField] private string serverUrl;
        [SerializeField] private float cooldownBeforeSend;

        private ILoadData _loadDataService = new LoadService();
        private ISaveData _saveDataService = new SaveService();

        public void TrackEvent(string type, string data)
        {
            
        }
    }
}
