using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthHandler))]
public class PlayerController : MonoBehaviour
{
    // Events
    public event Action OnPlayerDeath;
    
    [SerializeField] private float m_speed = 3;
    
    [Header("Visuals")]
    [SerializeField] private SpriteFlipper m_spriteFlipper;
    [SerializeField] private Animator m_spriteAnimator;
    
    // Movement
    private Rigidbody m_rb;
    private HealthHandler m_healthHandler;
    private float m_movementX;
    private float m_movementY;
    
    // Visuals
    private int m_isMovingHash;
    private const string k_IsMovingParam = "IsMoving";
    
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
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
        Vector3 movement = new Vector3 (m_movementX, 0.0f, m_movementY);
        m_rb.linearVelocity= movement * m_speed; 
    }

    // private void OnCollisionStay(Collision other)
    // {
    //     // If Other is enemy -> take damage
    // }

    private void PlayerDied()
    {
        OnPlayerDeath?.Invoke();
    }

    private void TakeDamage(int damage)
    {
        m_healthHandler.TakeDamage(damage);
    }

}

