using System.Collections;
using System.Collections.Generic;
using neeksdk.Scripts.Infrastructure.Services.EventService.EventData;
using neeksdk.Scripts.Infrastructure.Services.SaveLoadService;
using UnityEngine;

namespace neeksdk.Scripts.Infrastructure.Services.EventService
{
    public class EventService : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "http://test.server.com/api/send.php";
        [SerializeField] private float cooldownBeforeSend = 60f;

        private readonly ILoadData _loadDataService = new LoadService();
        private readonly ISaveData _saveDataService = new SaveService();
        
        private const string SAVE_EVENT_DATA_FILENAME = "event_data.dat";

        private Events _storedEvents = new Events();
        private Events _sendingEvents = new Events();

        public void TrackEvent(string type, string data) =>
            _storedEvents.events.Add(new EventData.EventData() { type = type, data = data });

        private IEnumerator SendDataToServer()
        {
            yield return new WaitForSeconds(cooldownBeforeSend);
            
            _sendingEvents = _storedEvents;
            _storedEvents.events.Clear();

            string jsonData = JsonUtility.ToJson(_sendingEvents);
            
        }

        private void Start()
        {
            if (_loadDataService.TryToLoadData<Events>(SAVE_EVENT_DATA_FILENAME, out Events events))
            {
                _storedEvents = events;
            }
            
            StartCoroutine(SendDataToServer());
        }

        private void OnEnable() =>
            StartCoroutine(SendDataToServer());

        private void OnDisable() =>
            StopAllCoroutines();

        private void OnDestroy() =>
            QuitService();

        private void OnApplicationQuit() =>
            QuitService();

        private void QuitService()
        {
            StopAllCoroutines();
            if (_sendingEvents != null && _sendingEvents.events.Count > 0)
            {
                _storedEvents.events.AddRange(_sendingEvents.events);
            }
            
            _saveDataService.SaveData<Events>(_storedEvents, SAVE_EVENT_DATA_FILENAME);
        }
    }
}
