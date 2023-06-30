namespace ZUserData
{
    public interface ILocalPersistentService<T>
        where T : class, new()
    {
        void Load();
        void Save();
        void Renew();
        T Get();
        string ToRaw();
        void SetRaw(string content);
    }
}
