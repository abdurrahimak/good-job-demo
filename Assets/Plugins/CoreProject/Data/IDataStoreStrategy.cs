
namespace CoreProject.Data
{
    public interface IDataStoreStrategy
    {
        object Read(string key);
        void Write(string key, object value);
        bool Has(string key);
    }
}