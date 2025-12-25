using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthHandler))]
[RequireComponent(typeof(KnockbackHandler))]
public class PlayerController : MonoBehaviour
{
    // Events
    public event Action OnPlayerDeath;
    
    [SerializeField] private float m_speed = 3;
    
    [Header("Visuals")]
    [SerializeField] private SpriteFlipper m_spriteFlipper;
    [SerializeField] private Animator m_spriteAnimator;
    private int m_isMovingHash;
    private const string k_IsMovingParam = "IsMoving";
    
    private Rigidbody m_rb;
    private HealthHandler m_healthHandler;
    private KnockbackHandler m_knockbackHandler;
    private float m_movementX;
    private float m_movementY;
    
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_knockbackHandler = GetComponent<KnockbackHandler>();
        m_healthHandler = GetComponent<HealthHandler>();
        m_healthHandler.Initialize();
        
        m_isMovingHash = Animator.StringToHash(k_IsMovingParam);
    }
    
    private void OnEnable()
    {
        m_healthHandler.OnDeath += PlayerDied;
    }

    private void OnDisable()
    {
        m_healthHandler.OnDeath -= PlayerDied;
    }
    
    private void OnMove(InputValue movementValue)
    {
        // Movement
        Vector2 movementVector = movementValue.Get<Vector2>();
        m_movementX = movementVector.x; 
        m_movementY = movementVector.y; 
        
        // Visuals
        m_spriteFlipper.FlipSprite(m_movementX);
        bool isMoving = movementVector.sqrMagnitude > 0;
        m_spriteAnimator.SetBool(m_isMovingHash, isMoving);
    }
    
    private void FixedUpdate()
    {
        if (m_knockbackHandler.IsKnockedBack) return;
        
        Vector3 movement = new Vector3 (m_movementX, 0.0f, m_movementY);
        m_rb.linearVelocity= movement * m_speed; 
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
            TakeDamage(enemy.AttackPower, knockbackDirection);
        }
    }

    
    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(10, Vector3.forward);
        }
    }
    

    private void PlayerDied()
    {
        OnPlayerDeath?.Invoke();
    }

    private void TakeDamage(int damage, Vector3 knockbackDirection)
    {
        m_healthHandler.TakeDamage(damage);
        m_knockbackHandler.ApplyKnockback(knockbackDirection); 
    }

}

