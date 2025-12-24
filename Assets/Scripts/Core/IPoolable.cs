public interface IPoolable
{
    /// <summary>
    /// Called when the object is retrieved from the pool
    /// </summary>
    void OnSpawn();
    
    /// <summary>
    /// Called when the object is returned to the pool
    /// </summary>
    void OnDespawn();
}

