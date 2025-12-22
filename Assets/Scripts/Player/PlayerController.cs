using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_speed = 3;

    [Header("Visuals")]
    [SerializeField] private SpriteFlipper m_spriteFlipper;
    [SerializeField] private Animator m_spriteAnimator;
    
    // Movement
    private Rigidbody m_rb; 
    private float m_movementX;
    private float m_movementY;
    
    // Visuals
    private int m_isMovingHash;
    private const string k_IsMovingParam = "IsMoving";
    
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_isMovingHash = Animator.StringToHash(k_IsMovingParam);
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
}

