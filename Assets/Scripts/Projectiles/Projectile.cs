using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_maxLifetime = 5f; // In seconds
    [SerializeField] private Transform m_visualTransform;
    
    private int m_damage;
    private float m_speed;
    private float m_knockbackForce;
    private Vector3 m_direction;
    private GameObjectPool m_pool;
    private Rigidbody m_rigidbody;

    public void Fire(int damage, float speed, float knockbackForce, Vector3 launchPosition, Vector3 direction, GameObjectPool pool, Vector3 inheritedVelocity = default)
    {
        m_damage = damage;
        m_speed = speed;
        m_knockbackForce = knockbackForce;
        m_direction = direction;
        m_pool = pool;
        
        transform.position = launchPosition;
        
        if (m_visualTransform != null)
        {
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            // Assume the sprite is facing left by default!
            m_visualTransform.rotation = Quaternion.Euler(0, 0, angle + 180f); 
        }
        
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
            
            // Only apply knockback if enemy is still active (not killed by damage)
            if (enemy.gameObject.activeInHierarchy)
            {
                // Apply knockback in the direction the projectile was traveling
                Vector3 knockbackDirection = m_direction.normalized;
                enemy.GetComponent<KnockbackHandler>().ApplyKnockback(knockbackDirection, m_knockbackForce);
            }
            
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

