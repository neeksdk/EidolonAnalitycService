namespace neeksdk.Scripts.Infrastructure.Services.SaveLoadService
{
    public interface ISaveData
    {
        void SaveData<T>(T data, string saveFileName) where T : class;
    }
}