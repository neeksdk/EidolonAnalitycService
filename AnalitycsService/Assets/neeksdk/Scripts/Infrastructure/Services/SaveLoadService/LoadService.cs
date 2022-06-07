using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace neeksdk.Scripts.Infrastructure.Services.SaveLoadService
{
    public class LoadService : ILoadData
    {
        public bool TryToLoadData<T>(string loadFileName, out T loadData) where T : class
        {
            string loadPath = Path.Combine(Application.persistentDataPath, loadFileName);
            bool loadFileExist = File.Exists(loadPath);
            loadData = null;
            
            if (!loadFileExist) return false;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream loadDataStream = File.Open(loadPath, FileMode.Open);

            try
            {
                loadData = (T) bf.Deserialize(loadDataStream);
            } catch (Exception e)
            {
                Debug.Log($" --- failed to load data, file name: {loadFileName}, type: {typeof(T)}, error: {e.Message}");
                return false;
            }
            
            loadDataStream.Close();

            return true;
        }
    }
}