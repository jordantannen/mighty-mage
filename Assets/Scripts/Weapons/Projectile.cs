using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_maxLifetime = 5f; // In seconds
    
    private int m_damage;
    private float m_speed;
    private Vector3 m_direction;
    private GameObjectPool m_pool;
    private Rigidbody m_rigidbody;

    public void Fire(int damage, float speed, Vector3 launchPosition, Vector3 direction, GameObjectPool pool, Vector3 inheritedVelocity = default)
    {
        m_damage = damage;
        m_speed = speed;
        m_direction = direction;
        m_pool = pool;
        
        transform.position = launchPosition;
        
        StopAllCoroutines();
        StartCoroutine(ReturnAfterLifetime());

        // Add inherited velocity (e.g., from player movement) so projectiles move relative to source
        m_rigidbody.linearVelocity = (m_direction.normalized * m_speed) + inheritedVelocity;
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
            enemy.GetComponent<HealthHandler>().TakeDamage(m_damage);
            ReturnToPool();
        }
    }
    
    private void ReturnToPool()
    {
        m_rigidbody.linearVelocity = Vector3.zero;
        m_pool.Return(gameObject);
    }
    
    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(m_maxLifetime);
        ReturnToPool();
    }
}

