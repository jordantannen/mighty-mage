using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies a red tint overlay to the screen when player health is low.
/// </summary>
public class LowHealthEffect : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private HealthHandler m_playerHealthHandler;
    [SerializeField] private Image m_overlayImage;
    
    [Header("Settings")]
    [SerializeField] private float m_lowHealthThreshold = 0.3f;
    [SerializeField] private float m_maxTintOpacity = 0.5f;
    [SerializeField] private Color m_tintColor = new Color(1f, 0f, 0f, 0f); // This isRed
    [SerializeField] private bool m_enablePulse = true;
    [SerializeField] private float m_pulseThreshold = 0.2f;
    [SerializeField] private float m_pulseSpeed = 2f;
    
    private float m_currentHealthPercent;
    private float m_pulseTimer;
    
    private void Start()
    {
        if (m_overlayImage != null)
        {
            Color color = m_tintColor;
            color.a = 0f;
            m_overlayImage.color = color;
        }
    }
    
    private void Update()
    {
        if (m_playerHealthHandler == null || m_overlayImage == null)
            return;
        
        m_currentHealthPercent = GetHealthPercentage();
        float targetOpacity = CalculateTargetOpacity();
        
        // TODO: Remove Magic Numbers
        if (m_enablePulse && m_currentHealthPercent <= m_pulseThreshold)
        {
            m_pulseTimer += Time.deltaTime * m_pulseSpeed;
            float pulseMultiplier = (Mathf.Sin(m_pulseTimer) + 1f) * 0.5f; 
            targetOpacity *= 0.5f + (pulseMultiplier * 0.5f); 
        }
        
        Color currentColor = m_overlayImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetOpacity, Time.deltaTime * 5f);
        m_overlayImage.color = currentColor;
    }
    
    private float GetHealthPercentage()
    {
        return m_playerHealthHandler.HealthPercentage;
    }
    
    private float CalculateTargetOpacity()
    {
        if (m_currentHealthPercent >= m_lowHealthThreshold)
            return 0f;

        float healthDeficit = 1f - (m_currentHealthPercent / m_lowHealthThreshold);
        return healthDeficit * m_maxTintOpacity;
    }
}
