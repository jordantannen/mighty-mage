using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private AudioClip m_pauseSound;
    [SerializeField] private AudioClip m_unpauseSound;
    [SerializeField] private AudioClip m_buttonHoverSound;
    
    private VisualElement m_resumeButton;
    private VisualElement m_quitButton;
    private UIDocument m_uiDocument;
    private AudioSource m_audioSource;
    private bool m_isPaused = false;
    
    private void Start()
    {
        m_uiDocument = GetComponent<UIDocument>();
        m_audioSource = GetComponent<AudioSource>();
        
        m_resumeButton = m_uiDocument.rootVisualElement.Q<VisualElement>("Resume");
        m_quitButton = m_uiDocument.rootVisualElement.Q<VisualElement>("Quit");
        
        m_resumeButton.RegisterCallback<ClickEvent>(evt => Hide());
        m_quitButton.RegisterCallback<ClickEvent>(evt => QuitGame());
        
        m_resumeButton.RegisterCallback<MouseEnterEvent>(evt => PlayButtonHover());
        m_quitButton.RegisterCallback<MouseEnterEvent>(evt => PlayButtonHover());
        Hide(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!m_isPaused)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    private void Show()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        m_isPaused = true;
        PlaySound(m_pauseSound);
    }
    
    private void Hide(bool playSound = true)
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        m_isPaused = false;
        if (playSound)
        {
            PlaySound(m_unpauseSound);
        }
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
    
    private void PlayButtonHover()
    {
        PlaySound(m_buttonHoverSound);
    }
}
