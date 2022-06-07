using System.Collections;
using System.Net;
using System.Text;
using neeksdk.Scripts.Infrastructure.Services.EventService.EventData;
using neeksdk.Scripts.Infrastructure.Services.SaveLoadService;
using UnityEngine;
using UnityEngine.Networking;

namespace neeksdk.Scripts.Infrastructure.Services.EventService
{
    public class EventService : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "http://test.server.com/api/send.php";
        [SerializeField] private float cooldownBeforeSend = 60f;

        private readonly ILoadData _loadDataService = new LoadService();
        private readonly ISaveData _saveDataService = new SaveService();
        
        private const string SAVE_EVENT_DATA_FILENAME = "event_data.dat";

        private bool _eventsIsSending;

        private Events _storedEvents = new Events();
        private Events _sendingEvents = new Events();

        public void TrackEvent(string type, string data) =>
            _storedEvents.events.Add(new EventData.EventData() { type = type, data = data });

        private IEnumerator SendDataToServer()
        {
            yield return new WaitForSeconds(cooldownBeforeSend);

            if (Application.internetReachability == NetworkReachability.NotReachable || _storedEvents.events.Count == 0)
            {
                StartCoroutine(SendDataToServer());
                yield break;
            }
            
            _sendingEvents.events.AddRange(_storedEvents.events);
            _storedEvents.events.Clear();

            SendPostRequest();
        }

        private void SendPostRequest()
        {
            if (_eventsIsSending)
            {
                return;
            }

            _eventsIsSending = true;
            
            string jsonData = JsonUtility.ToJson(_sendingEvents);
            byte[] encodedJson = Encoding.UTF8.GetBytes(jsonData);
            
            LogSendingRequest(jsonData);

            WWWForm formData = new WWWForm();
            UnityWebRequest postRequest = UnityWebRequest.Post(serverUrl, formData);
            postRequest.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            postRequest.uploadHandler = new UploadHandlerRaw(encodedJson);

            postRequest.SendWebRequest().completed += operation =>
            {
                _eventsIsSending = false;
                
                if (postRequest.result == UnityWebRequest.Result.ProtocolError ||
                    postRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    RestartSendingRequest();
                }
                else
                {
                    if (postRequest.responseCode == (long) HttpStatusCode.OK)
                    {
                        postRequest.Dispose();
                        _sendingEvents.events.Clear();
                        StartCoroutine(SendDataToServer());
                    }
                    else
                    {
                        RestartSendingRequest();
                    }
                }
            };
        }

        private void RestartSendingRequest()
        {
            _storedEvents.events.AddRange(_sendingEvents.events);
            _sendingEvents.events.Clear();
            StartCoroutine(SendDataToServer());
        }

        private void Start()
        {
            if (_loadDataService.TryToLoadData(SAVE_EVENT_DATA_FILENAME, out Events events))
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

        private void QuitService()
        {
            StopAllCoroutines();
            if (_sendingEvents != null && _sendingEvents.events.Count > 0)
            {
                _storedEvents.events.AddRange(_sendingEvents.events);
            }
            
            _saveDataService.SaveData(_storedEvents, SAVE_EVENT_DATA_FILENAME);
        }
        
        private void LogSendingRequest(string jsonData)
        {
            Debug.Log($" ");
            Debug.Log($" --------------------------------------------------------------");
            Debug.Log($" --- Try to send data to server (timeout {cooldownBeforeSend}):");
            Debug.Log($" ---    host: {serverUrl}");
            Debug.Log($" ---    json: {jsonData}");
            Debug.Log($" --------------------------------------------------------------");
            Debug.Log($" ");
        }
    }
}
