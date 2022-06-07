using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace neeksdk.Scripts.Infrastructure.Services.SaveLoadService
{
    public class SaveService : ISaveData
    {
        public void SaveData<T>(T data, string saveFileName) where T : class
        {
            BinaryFormatter bf = new BinaryFormatter();
            string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            bool saveFileExist = File.Exists(savePath);

            FileStream file = saveFileExist 
                ? File.OpenWrite(savePath) 
                : File.Create(savePath);
            
            bf.Serialize(file, data);
            file.Close();
        }
    }
}