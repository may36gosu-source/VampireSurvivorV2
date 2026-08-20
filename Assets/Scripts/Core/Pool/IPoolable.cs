namespace VampireSurvivors.Core
{
    public interface IPoolable
    {
        void OnSpawn();

        void OnDespawn();
    }
}