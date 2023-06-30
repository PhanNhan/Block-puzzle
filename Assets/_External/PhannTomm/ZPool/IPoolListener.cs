namespace ZPool
{
    public interface IPoolListener
    {
        void OnSpawn();
        void OnRecycle();
    }
}
