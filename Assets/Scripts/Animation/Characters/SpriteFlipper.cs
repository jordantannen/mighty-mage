using UnityEngine;
using UnityEngine.Serialization;

public class SpriteFlipper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private bool m_facingLeft;
    
    /// <summary>
    /// Flips a sprite to face its movement direction
    /// </summary>
    /// <param name="horizontalInput">The 'X' value of the movement vector</param>
    public void FlipSprite(float horizontalInput)
    {
        if (horizontalInput == 0) return;

        bool shouldFlip = m_facingLeft ? horizontalInput > 0 : horizontalInput < 0;
        m_spriteRenderer.flipX = shouldFlip;
    }
}
