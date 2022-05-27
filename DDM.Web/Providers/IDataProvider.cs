namespace DDM.Web.Providers
{
    public interface IDataProvider
    {
        T GetData<T>(string url);
    }
}