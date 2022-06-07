namespace neeksdk.Scripts.Infrastructure.Services.SaveLoadService
{
    public interface ILoadData
    {
        bool TryToLoadData<T>(string loadFileName, out T loadData) where T : class;
    }
}