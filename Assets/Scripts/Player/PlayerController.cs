using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_speed = 0;

    [Header("Visuals")]
    [SerializeField] private SpriteFlipper m_spriteFlipper;
    
    private Rigidbody m_rb; 
    private float m_movementX;
    private float m_movementY;
    
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        m_movementX = movementVector.x; 
        m_movementY = movementVector.y; 
        m_spriteFlipper.FlipSprite(m_movementX);
    }
    
    private void FixedUpdate() 
    {
        Vector3 movement = new Vector3 (m_movementX, 0.0f, m_movementY);
        m_rb.linearVelocity= movement * m_speed; 
    }
}

