using System;
using System.Collections;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    // Events
    public event Action OnDeath;
    
    [Header("Stats")]
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private int m_currentHealth;
    
    [Header("IFrames")]
    [SerializeField] private bool m_hasIFrames;
    [SerializeField] private float m_iFrameDuration = 1f;
    private bool m_isInvincible; 
    
    [Header("Visuals")] 
    [SerializeField] private Animator m_spriteAnimator;
    [SerializeField] private SpriteRenderer m_visualRenderer;
    [SerializeField] private Sprite m_damageFlashSprite;
    [SerializeField] private float m_damageFlashDuration = 0.1f;
    private Sprite m_originalSprite;

    [Header("Sound")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_damageSound;
    [SerializeField] private float m_pitchLowerBound = 0.9f;
    [SerializeField] private float m_pitchUpperBound = 1.1f;
    
    private void Awake()
    {
        if (m_visualRenderer)
            m_originalSprite = m_visualRenderer.sprite;
    }

    public void Initialize()
    {
        m_currentHealth = m_maxHealth;
        m_isInvincible = false;
        
        // Ensure that if something dies mid-sprite flash, it'll reset
        // upon respawn
        StopAllCoroutines();
        
        // Only reset sprite if we have a valid original sprite
        if (m_visualRenderer && m_originalSprite)
        {
            m_visualRenderer.sprite = m_originalSprite;
        }
        
        if (m_spriteAnimator) m_spriteAnimator.enabled = true;
    }

    /// <summary>
    /// Reduces current health
    /// </summary>
    /// <param name="damage"> Amount of damage to be taken </param>
    public bool TakeDamage(int damage)
    {
        if (m_isInvincible) return false;
        
        m_currentHealth -= damage;

        if (m_audioSource && m_damageSound)
        {
            m_audioSource.pitch = UnityEngine.Random.Range(m_pitchLowerBound, m_pitchUpperBound);
            m_audioSource.PlayOneShot(m_damageSound);
        }
        
        if (m_visualRenderer && m_damageFlashSprite)
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

        return true;
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