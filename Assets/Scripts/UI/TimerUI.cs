using UnityEngine;
using UnityEngine.UIElements;

public class TimerUI : MonoBehaviour
{
    private UIDocument m_uiDocument;
    private Label m_timerLabel;
    private float m_survivalTime;
    private bool m_isRunning;
    
    private void Start()
    {
        m_uiDocument = GetComponent<UIDocument>();
        m_timerLabel = m_uiDocument.rootVisualElement.Q<Label>("Timer");
        
        if (m_timerLabel == null)
        {
            Debug.LogError("TimerUI: Timer label not found in UI Document", this);
        }
        
        m_survivalTime = 0f;
        m_isRunning = true;
        UpdateTimerDisplay();
    }
    
    private void Update()
    {
        if (m_isRunning)
        {
            m_survivalTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    
    public void Show()
    {
        if (m_uiDocument != null)
        {
            m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }
    
    public void Hide()
    {
        if (m_uiDocument != null)
        {
            m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
    
    public void StopTimer()
    {
        m_isRunning = false;
    }
    
    public void ResetTimer()
    {
        m_survivalTime = 0f;
        m_isRunning = true;
        UpdateTimerDisplay();
    }
    
    public float GetSurvivalTime()
    {
        return m_survivalTime;
    }
    
    private void UpdateTimerDisplay()
    {
        if (m_timerLabel != null)
        {
            int minutes = Mathf.FloorToInt(m_survivalTime / 60f);
            int seconds = Mathf.FloorToInt(m_survivalTime % 60f);
            m_timerLabel.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
