using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthHandler))]
[RequireComponent(typeof(KnockbackHandler))]
public class Enemy : MonoBehaviour
{
    public event Action OnDeath;
    
    [Header("Stats")]
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private int m_attackPower = 10;
    public int AttackPower => m_attackPower;
    
    [Header("Visuals")] 
    [SerializeField] private SpriteFlipper m_spriteFlipper;
    
    // [Header("Debugging")]
    // [SerializeField] private Transform m_playerLocation;

    private NavMeshAgent m_agent;
    private Rigidbody m_rb;
    private HealthHandler m_healthHandler;
    private KnockbackHandler m_knockbackHandler;
    private Transform m_target;

    // private void Start()
    // {
    //     SetTarget(m_playerLocation);
    // }

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_knockbackHandler = GetComponent<KnockbackHandler>();
        m_healthHandler = GetComponent<HealthHandler>();
        m_agent = GetComponent<NavMeshAgent>();

        m_agent.speed = m_moveSpeed;
        m_agent.updateRotation = false; // Handled by SpriteFlipper
    }

    private void OnEnable()
    {
        m_healthHandler.OnDeath += Die;
        m_knockbackHandler.OnKnockbackStart += DisableNavAgent;
        m_knockbackHandler.OnKnockbackEnd += EnableNavAgent;
        
        EnableNavAgent();
        
        m_healthHandler.Initialize();
        m_knockbackHandler.Initialize();
    }

    private void OnDisable()
    {
        m_healthHandler.OnDeath -= Die;
        m_knockbackHandler.OnKnockbackStart -= DisableNavAgent;
        m_knockbackHandler.OnKnockbackEnd -= EnableNavAgent;
    }

    public void SetTarget(Transform target)
    {
        m_target = target;
    }
    
    private void Update()
    {
        if (!m_target || m_knockbackHandler.IsKnockedBack) return;
        
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        // Vector3 direction = (m_target.position - transform.position).normalized;
        // Vector3 moveVelocity = direction * m_moveSpeed;
        // m_rb.linearVelocity = moveVelocity;
        //
        // m_spriteFlipper.FlipSprite(direction.x);
        // Use NavMeshAgent for pathfinding
        m_agent.SetDestination(m_target.position);
        
        // Flip sprite based on agent velocity
        if (m_agent.velocity.sqrMagnitude > 0.01f)
        {
            m_spriteFlipper.FlipSprite(m_agent.velocity.x);
        }
    }

    private void Die()
    {
        // Stop movement immediately
        DisableNavAgent();
        m_rb.linearVelocity = Vector3.zero;
        
        // Small delay to let damage sound start playing before enemy is disabled
        StartCoroutine(DelayedDeath());
    }

    // This somewhat solves the weird sound problem, but TODO review this
    private IEnumerator DelayedDeath()
    {
        yield return null;
        OnDeath?.Invoke();
    }

    private void EnableNavAgent()
    {
        m_agent.enabled = true;
        m_rb.isKinematic = true; // Kinematic registers collisions, not forces
    }
    
    private void DisableNavAgent()
    {
        m_agent.enabled = false;
        m_rb.isKinematic = false;
    }
    
}
