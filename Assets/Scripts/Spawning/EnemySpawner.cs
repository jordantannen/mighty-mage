using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable] public class SpawnRateEntry
    {
        public int round = 1;
        public float spawnInterval = 2f;
    }
    
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Spawn Settings")]
    [SerializeField] private GameObject m_enemyPrefab;
    [SerializeField] private Transform m_playerTarget;
    [SerializeField] private float m_spawnDistance = 10f; // From player
    [SerializeField] private float m_spawnRadiusVariation = 0.5f; 
    [SerializeField] private int m_preallocateCount = 20;
    
    [Header("Round Configuration")]
    [SerializeField] private int m_startRound = 1;
    [SerializeField] private List<SpawnRateEntry> m_spawnRates = new List<SpawnRateEntry>
    {
        new SpawnRateEntry { round = 1, spawnInterval = 2f }
    };
    
    private GameObjectPool m_enemyPool;
    private readonly Dictionary<GameObject, Action> m_deathHandlers = new Dictionary<GameObject, Action>();
    private bool m_isSpawning;

    private void Awake()
    {
        CreatePool();
    }

    private void OnEnable()
    {
        if (m_gameManager != null)
        {
            m_gameManager.OnRoundStarted += OnRoundStarted;
        }
    }

    private void OnDisable()
    {
        if (m_gameManager != null)
        {
            m_gameManager.OnRoundStarted -= OnRoundStarted;
        }
    }

    private void OnRoundStarted(int round)
    {
        StopSpawning();
        
        if (IsActiveForRound(round))
        {
            float spawnInterval = GetSpawnInterval(round);
            StartSpawning(spawnInterval);
        }
    }
    
    private bool IsActiveForRound(int round)
    {
        return round >= m_startRound;
    }
    
    private float GetSpawnInterval(int round)
    {
        m_spawnRates.Sort((a, b) => a.round.CompareTo(b.round));
        
        float currentInterval = m_spawnRates[0].spawnInterval;
        
        foreach (var entry in m_spawnRates)
        {
            if (entry.round <= round)
            {
                currentInterval = entry.spawnInterval;
            }
            else
            {
                break;
            }
        }
        
        return currentInterval;
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
        if (m_isSpawning) return;
        
        m_isSpawning = true;
        StartCoroutine(SpawnRoutine(spawnRate));
    }

    /// <summary>
    /// Stops the spawning coroutine
    /// </summary>
    public void StopSpawning()
    {
        m_isSpawning = false;
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
