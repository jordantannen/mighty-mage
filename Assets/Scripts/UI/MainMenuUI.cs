using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private AudioClip m_buttonHoverSound;
    [SerializeField] private AudioClip m_buttonConfirmSound;
    [SerializeField] private bool m_hideOnStart;
    
    private AudioSource m_audioSource;
    private VisualElement m_playButton;
    private VisualElement m_quitButton;
    private UIDocument m_uiDocument;
    
    private void Start()
    {
        m_uiDocument = GetComponent<UIDocument>();
        m_audioSource = GetComponent<AudioSource>();
        
        m_playButton = m_uiDocument.rootVisualElement.Q<VisualElement>("Play");
        m_quitButton = m_uiDocument.rootVisualElement.Q<VisualElement>("Quit");

        m_playButton.RegisterCallback<ClickEvent>(evt => Play());
        m_quitButton.RegisterCallback<ClickEvent>(evt => QuitGame());
        
        m_playButton.RegisterCallback<MouseEnterEvent>(evt => PlaySound(m_buttonHoverSound));
        m_quitButton.RegisterCallback<MouseEnterEvent>(evt => PlaySound(m_buttonHoverSound));
        
        if (m_hideOnStart)
        {
            Hide();
        }
    }
    
    public void Show()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    
    public void Hide()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void Play()
    {
        SceneManager.LoadScene("Main");
    }
    
    private void QuitGame()
    {
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
}
