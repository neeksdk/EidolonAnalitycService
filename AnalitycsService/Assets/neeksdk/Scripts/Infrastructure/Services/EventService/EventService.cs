using System.Collections;
using System.Net;
using System.Text;
using neeksdk.Scripts.Infrastructure.Services.EventService.EventData;
using neeksdk.Scripts.Infrastructure.Services.SaveLoadService;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

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
        private int _sendingEventsCount;

        public void TrackEvent(string type, string data) =>
            _storedEvents.events.Add(new EventData.EventData() { type = type, data = data });

        private IEnumerator SendDataToServer()
        {
            yield return new WaitForSeconds(cooldownBeforeSend);

            _saveDataService.SaveData(_storedEvents, SAVE_EVENT_DATA_FILENAME);
            
            if (Application.internetReachability == NetworkReachability.NotReachable || _storedEvents.events.Count == 0)
            {
                StartCoroutine(SendDataToServer());
                yield break;
            }

            SendPostRequest();
        }

        private void SendPostRequest()
        {
            if (_eventsIsSending)
            {
                return;
            }

            _eventsIsSending = true;
            
            _sendingEventsCount = _storedEvents.events.Count;
            string jsonData = JsonUtility.ToJson(_storedEvents);
            byte[] encodedJson = Encoding.UTF8.GetBytes(jsonData);
            
            LogSendingRequest(jsonData);

            WWWForm formData = new WWWForm();
            UnityWebRequest postRequest = UnityWebRequest.Post(serverUrl, formData);
            postRequest.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            postRequest.uploadHandler = new UploadHandlerRaw(encodedJson);

            postRequest.SendWebRequest().completed += operation =>
            {
                _eventsIsSending = false;
                
                if (postRequest.result != UnityWebRequest.Result.ProtocolError &&
                    postRequest.result != UnityWebRequest.Result.ConnectionError)
                {
                    if (postRequest.responseCode == (long) HttpStatusCode.OK)
                    {
                        postRequest.Dispose();
                        _storedEvents.events.RemoveRange(0, _sendingEventsCount);
                        _saveDataService.SaveData(_storedEvents, SAVE_EVENT_DATA_FILENAME);
                    }
                }
                
                StartCoroutine(SendDataToServer());
            };
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
        
#if UNITY_EDITOR
        [MenuItem("Eidolon/Event service clear saved data")]
        public static void ClearSaveData() {
            string saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_EVENT_DATA_FILENAME);
            File.Delete(saveFilePath);
        }
    }
#endif
}
