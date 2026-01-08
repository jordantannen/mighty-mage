using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackHandler : MonoBehaviour
{
    public event Action OnKnockbackStart;
    public event Action OnKnockbackEnd;
    
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
    
    public void Initialize()
    {
        if (!m_rb)
        {
            m_rb = GetComponent<Rigidbody>();
        }
        
        StopAllCoroutines();
        m_isKnockedBack = false;
        m_knockbackCoroutine = null;
        
        // This check prevents issues with characters on navmesh
        if (!m_rb.isKinematic)
        {
            m_rb.linearVelocity = Vector3.zero;
        }
    }
    
    /// <summary>
    /// Knocks the game object back in a given direction
    /// </summary>
    /// <param name="direction"> Direction where the knockback should move </param>
    /// <param name="force"> Force of the knockback. If not specified, uses the default m_knockbackForce </param>
    public void ApplyKnockback(Vector3 direction, float force = -1f)
    {
        direction.y = 0; // Keep knockback horizontal
        
        // Use default force if not specified
        float knockbackForce = force < 0 ? m_knockbackForce : force;
        
        // If already knocked back, stop the previous coroutine
        if (m_knockbackCoroutine != null)
        {
            StopCoroutine(m_knockbackCoroutine);
        }
        
        m_knockbackCoroutine = StartCoroutine(KnockbackRoutine(direction, knockbackForce));
    }
    
    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        m_isKnockedBack = true;
        OnKnockbackStart?.Invoke();
        
        // Reset velocity to ensure consistent knockback
        m_rb.linearVelocity = Vector3.zero;
        m_rb.AddForce(direction * force, ForceMode.Impulse);
        
        yield return new WaitForSeconds(m_knockbackDuration);
        
        // Stop sliding
        m_rb.linearVelocity = Vector3.zero;
        m_isKnockedBack = false;
        m_knockbackCoroutine = null;
        Debug.Log($"Knockbacked {gameObject.name}");
        OnKnockbackEnd?.Invoke();   
    }
}

