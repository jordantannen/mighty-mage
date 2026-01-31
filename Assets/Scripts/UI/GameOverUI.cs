using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private AudioClip m_buttonHoverSound;
    [SerializeField] private AudioClip m_buttonConfirmSound;
    [SerializeField] private bool m_hideOnStart = true;
    
    private AudioSource m_audioSource;
    private VisualElement m_playButton;
    private VisualElement m_quitButton;
    private Label m_survivalTimeLabel;
    private UIDocument m_uiDocument;
    
    private void Start()
    {
        m_uiDocument = GetComponent<UIDocument>();
        m_audioSource = GetComponent<AudioSource>();
        
        m_playButton = m_uiDocument.rootVisualElement.Q<VisualElement>("Play");
        m_quitButton = m_uiDocument.rootVisualElement.Q<VisualElement>("Quit");
        m_survivalTimeLabel = m_uiDocument.rootVisualElement.Q<Label>("SurvivalTime");

        m_playButton.RegisterCallback<ClickEvent>(evt => PlayAgain());
        m_quitButton.RegisterCallback<ClickEvent>(evt => QuitGame());
        
        m_playButton.RegisterCallback<MouseEnterEvent>(evt => PlaySound(m_buttonHoverSound));
        m_quitButton.RegisterCallback<MouseEnterEvent>(evt => PlaySound(m_buttonHoverSound));
        
        if (m_hideOnStart)
        {
            Hide();
        }
    }
    
    public void Show(float survivalTime)
    {
        if (m_survivalTimeLabel != null)
        {
            m_survivalTimeLabel.text = FormatTime(survivalTime);
        }
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    
    public void Hide()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void PlayAgain()
    {
        PlaySound(m_buttonConfirmSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void QuitGame()
    {
        PlaySound(m_buttonConfirmSound);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
           Application.Quit();
        #endif
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (m_audioSource != null && clip != null)
        {
            m_audioSource.PlayOneShot(clip);
        }
    }
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"You survived: {minutes:00}:{seconds:00}";
    }
}
