using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthHandler))]
[RequireComponent(typeof(KnockbackHandler))]
public class Enemy : MonoBehaviour
{
    public event Action OnDeath;
    
    [Header("Stats")]
    [SerializeField] private int m_moveSpeed = 2;
    [SerializeField] private int m_attackPower = 10;
    public int AttackPower => m_attackPower;
    
    [Header("Visuals")] 
    [SerializeField] private SpriteFlipper m_spriteFlipper;
    
    private Rigidbody m_rb;
    private HealthHandler m_healthHandler;
    private KnockbackHandler m_knockbackHandler;
    private Transform m_target;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_knockbackHandler = GetComponent<KnockbackHandler>();
        m_healthHandler = GetComponent<HealthHandler>();
    }

    private void OnEnable()
    {
        m_healthHandler.OnDeath += Die;
        m_healthHandler.Initialize();
        m_knockbackHandler.Initialize();
    }

    private void OnDisable()
    {
        m_healthHandler.OnDeath -= Die;
    }

    public void SetTarget(Transform target)
    {
        m_target = target;
    }
    
    private void FixedUpdate()
    {
        if (!m_target || m_knockbackHandler.IsKnockedBack) return;
        
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector3 direction = (m_target.position - transform.position).normalized;
        Vector3 moveVelocity = direction * m_moveSpeed;
        m_rb.linearVelocity = moveVelocity;
        
        m_spriteFlipper.FlipSprite(direction.x);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
    
}
