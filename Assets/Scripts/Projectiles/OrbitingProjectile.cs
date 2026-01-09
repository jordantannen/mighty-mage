using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OrbitingProjectile : MonoBehaviour
{

    [SerializeField] private float m_yOffset = 0;
    
    private int m_damage;
    private float m_knockbackForce;
    private float m_orbitSpeed; // Degrees per second
    private float m_orbitRadius;
    private Transform m_centerPoint;
    private float m_currentAngle;

    /// <summary>
    /// Initializes the orbiting projectile
    /// </summary>
    /// <param name="damage"> Damage dealt on hit </param>
    /// <param name="knockbackForce"> Force applied to enemies on hit </param>
    /// <param name="orbitSpeed"> Rotation speed in degrees per second </param>
    /// <param name="orbitRadius"> Distance from center point </param>
    /// <param name="centerPoint"> Transform to orbit around </param>
    /// <param name="startAngle"> Initial angle in degrees </param>
    public void Initialize(int damage, float knockbackForce, float orbitSpeed, float orbitRadius, Transform centerPoint, float startAngle)
    {
        m_damage = damage;
        m_knockbackForce = knockbackForce;
        m_orbitSpeed = orbitSpeed;
        m_orbitRadius = orbitRadius;
        m_centerPoint = centerPoint;
        m_currentAngle = startAngle;
        
        UpdatePosition();
    }

    private void Update()
    {
        if (m_centerPoint == null) return;
        
        m_currentAngle += m_orbitSpeed * Time.deltaTime;
        UpdatePosition();
    }

    // Remember the Unit Circle babyyyy: 
    // https://www.khanacademy.org/math/algebra2/x2ec2f6f830c9fb89:trig/x2ec2f6f830c9fb89:unit-circle/v/unit-circle-definition-of-trig-functions-1
    private void UpdatePosition()
    {
        float radians = m_currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians) * m_orbitRadius, m_yOffset, Mathf.Sin(radians) * m_orbitRadius);
        transform.position = m_centerPoint.position + offset;
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
                // Apply knockback away from the orbit center
                Vector3 knockbackDirection = (enemy.transform.position - m_centerPoint.position).normalized;
                enemy.GetComponent<KnockbackHandler>().ApplyKnockback(knockbackDirection, m_knockbackForce);
            }
        }
    }
}

