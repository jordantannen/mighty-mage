using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Transform m_parent; // To put all enemies in one parent
    [SerializeField] private int m_defaultCapacity = 20;
    [SerializeField] private int m_maxSize = 100;
    
    private ObjectPool<GameObject> m_pool;

    private void Awake()
    {
        m_pool = new ObjectPool<GameObject>(
            createFunc: CreatePooledObject,
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: OnDestroyPooledObject,
            collectionCheck: true,
            defaultCapacity: m_defaultCapacity,
            maxSize: m_maxSize
        );
    }

    /// <summary>
    /// Gets an object from the pool
    /// </summary>
    public GameObject Get()
    {
        return m_pool.Get();
    }

    /// <summary>
    /// Returns an object to the pool
    /// </summary>
    /// <param name="obj"> The GameObject being returned </param>
    public void Return(GameObject obj)
    {
        m_pool.Release(obj);
    }

    /// <summary>
    /// Pre-instantiates objects to avoid runtime allocations
    /// </summary>
    /// <param name="count"> The number of GameObjects to prewarm </param>
    public void Preallocate(int count)
    {
        var prewarmObjects = new GameObject[count];
        
        for (int i = 0; i < count; i++)
        {
            prewarmObjects[i] = m_pool.Get();
        }
        
        for (int i = 0; i < count; i++)
        {
            m_pool.Release(prewarmObjects[i]);
        }
    }

    private GameObject CreatePooledObject()
    {
        GameObject obj = Instantiate(m_prefab, m_parent); 
        return obj;
    }

    private void OnGetFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyPooledObject(GameObject obj)
    {
        Destroy(obj);
    }
}

