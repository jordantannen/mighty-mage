using System;
using System.Collections;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    // Events
    public event Action OnDeath;
    
    // Stats
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private int m_currentHealth;
    [SerializeField] private float m_damageFlashDuration = 0.1f;
    
    // IFrames
    [Header("IFrames")]
    [SerializeField] private bool m_hasIFrames;
    [SerializeField] private float m_iFrameDuration = 1f;
    private bool m_isInvincible; 
    
    // Visuals
    [Header("Visuals")] 
    [SerializeField] private SpriteRenderer m_visualRenderer;
    [SerializeField] private Sprite m_damageFlashSprite;
    [SerializeField] private Animator m_spriteAnimator;
    private Sprite m_originalSprite;
    
    private void Awake()
    {
        if (m_visualRenderer != null)
            m_originalSprite = m_visualRenderer.sprite;
    }

    public void Initialize()
    {
        m_currentHealth = m_maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (m_isInvincible) return;
        
        m_currentHealth -= damage;

        if (m_visualRenderer != null && m_damageFlashSprite != null)
        {
            StartCoroutine(DamageFlash());
        }
        
        if (m_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        
        if (m_hasIFrames)
        {
            StartCoroutine(ActivateIFrames());
        }
    }
    
    private IEnumerator DamageFlash()
    {
        m_spriteAnimator.enabled = false;
        m_visualRenderer.sprite = m_damageFlashSprite;

        yield return new WaitForSeconds(m_damageFlashDuration);

        m_visualRenderer.sprite = m_originalSprite;
        m_spriteAnimator.enabled = true;
    }
    
    private IEnumerator ActivateIFrames()
    {
        m_isInvincible = true;
        yield return new WaitForSeconds(m_iFrameDuration);
        m_isInvincible = false;
    }
}