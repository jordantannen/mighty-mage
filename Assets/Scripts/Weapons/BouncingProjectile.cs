using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BouncingProjectile : MonoBehaviour
{
    [SerializeField] private float m_maxLifetime = 5f;
    
    private int m_damage;
    private float m_speed;
    private int m_bouncesRemaining;
    private float m_bounceRange;
    private GameObjectPool m_pool;
    private Rigidbody m_rigidbody;
    private Enemy m_lastHitEnemy;

    public void Fire(int damage, float speed, Vector3 launchPosition, Vector3 direction, GameObjectPool pool, int bounceCount, float bounceRange)
    {
        m_damage = damage;
        m_speed = speed;
        m_bouncesRemaining = bounceCount;
        m_bounceRange = bounceRange;
        m_pool = pool;
        m_lastHitEnemy = null;
        
        transform.position = launchPosition;
        
        StopAllCoroutines();
        StartCoroutine(ReturnAfterLifetime());

        m_rigidbody.linearVelocity = direction.normalized * m_speed;
    }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.activeInHierarchy) return;
        
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // Deal damage
            enemy.GetComponent<HealthHandler>().TakeDamage(m_damage);
            
            // Store as last hit enemy so we don't immediately bounce back
            m_lastHitEnemy = enemy;
            
            // Try to bounce to next target
            TryBounce();
        }
    }

    private void TryBounce()
    {
        if (m_bouncesRemaining <= 0)
        {
            ReturnToPool();
            return;
        }
        
        Enemy nextTarget = FindNextTarget();
        
        if (nextTarget == null)
        {
            ReturnToPool();
            return;
        }
        
        // Redirect toward next target
        Vector3 direction = (nextTarget.transform.position - transform.position).normalized;
        m_rigidbody.linearVelocity = direction * m_speed;
        
        m_bouncesRemaining--;
    }

    private Enemy FindNextTarget()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closestEnemy = null;
        float closestDistance = m_bounceRange;

        foreach (Enemy enemy in allEnemies)
        {
            // Skip inactive enemies
            if (!enemy || !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }
            
            // Skip the last hit enemy to prevent immediate re-hit
            if (enemy == m_lastHitEnemy)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
    
    private void ReturnToPool()
    {
        m_rigidbody.linearVelocity = Vector3.zero;
        m_lastHitEnemy = null;
        m_pool.Return(gameObject);
    }
    
    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(m_maxLifetime);
        ReturnToPool();
    }
}

