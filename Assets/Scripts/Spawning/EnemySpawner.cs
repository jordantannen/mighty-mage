using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private Transform m_playerTarget;
    [SerializeField] private float m_spawnDistance = 10f; // From player
    [SerializeField] private float m_spawnRadiusVariation = 0.5f; 
    [SerializeField] private float m_spawnRate = 2f;
    [SerializeField] private int m_preallocateCount = 20;
    
    private GameObjectPool m_enemyPool;
    private readonly Dictionary<GameObject, Action> m_deathHandlers = new Dictionary<GameObject, Action>();

    private void Awake()
    {
        CreatePool();
    }

    private void Start()
    {
        StartSpawning(m_spawnRate);
    }

    private void CreatePool()
    {
        m_enemyPool = gameObject.AddComponent<GameObjectPool>();
        m_enemyPool.Initialize(m_enemyPrefab, transform);
        m_enemyPool.Preallocate(m_preallocateCount);
    }

    /// <summary>
    /// Starts spawning enemies continuously at a given rate
    /// </summary>
    /// <param name="spawnRate"> Time in seconds between spawns </param>
    public void StartSpawning(float spawnRate)
    {
        StartCoroutine(SpawnRoutine(spawnRate));
    }

    /// <summary>
    /// Stops the spawning coroutine
    /// </summary>
    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine(float spawnRate)
    {
        var wait = new WaitForSeconds(spawnRate);
        
        while (true)
        {
            SpawnEnemy();
            yield return wait;
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemyObj = m_enemyPool.Get();
        enemyObj.transform.position = FindSpawnPosition();
        
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetTarget(m_playerTarget);
        
        // Store the delegate to unsubscribe later
        Action deathHandler = () => ReturnEnemyToPool(enemyObj);
        m_deathHandlers[enemyObj] = deathHandler;
        enemy.OnDeath += deathHandler; // When enemy dies, call the deathHandler, to unsub from this action and return to pool
    }

    private void ReturnEnemyToPool(GameObject enemyObj)
    {
        // Unsubscribe using the stored delegate
        if (m_deathHandlers.TryGetValue(enemyObj, out Action deathHandler))
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.OnDeath -= deathHandler;
            m_deathHandlers.Remove(enemyObj);
        }
        
        m_enemyPool.Return(enemyObj);
    }

    private Vector3 FindSpawnPosition()
    {
        float radiusVariation = UnityEngine.Random.Range(-m_spawnRadiusVariation, m_spawnRadiusVariation); // Makes the spawn radius a bit more natural
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * (m_spawnDistance + radiusVariation);
        
        return m_playerTarget.position + new Vector3(randomPoint.x, 0f, randomPoint.y);
    }
}
