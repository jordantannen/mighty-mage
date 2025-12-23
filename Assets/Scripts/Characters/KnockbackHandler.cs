using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackHandler : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float m_knockbackForce = 5f;
    [SerializeField] private float m_knockbackDuration = 0.2f;
    
    public bool IsKnockedBack => m_isKnockedBack;
    
    private Rigidbody m_rb;
    private Coroutine m_knockbackCoroutine;
    private bool m_isKnockedBack;
    
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    
    /// <summary>
    /// Knocks the game object back in a given direction
    /// </summary>
    /// <param name="direction"> Direction where the knockback should move </param>
    public void ApplyKnockback(Vector3 direction)
    {
        direction.y = 0; // Keep knockback horizontal
        
        // If already knocked back, stop the previous coroutine
        if (m_knockbackCoroutine != null)
        {
            StopCoroutine(m_knockbackCoroutine);
        }
        
        m_knockbackCoroutine = StartCoroutine(KnockbackRoutine(direction));
    }
    
    private IEnumerator KnockbackRoutine(Vector3 direction)
    {
        m_isKnockedBack = true;
        
        // Reset velocity to ensure consistent knockback
        m_rb.linearVelocity = Vector3.zero;
        m_rb.AddForce(direction * m_knockbackForce, ForceMode.Impulse);
        
        yield return new WaitForSeconds(m_knockbackDuration);
        
        // Stop sliding
        m_rb.linearVelocity = Vector3.zero;
        m_isKnockedBack = false;
        m_knockbackCoroutine = null;
    }
}

