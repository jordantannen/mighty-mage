using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer m_instance;

    void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject); 
        }
        else
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }
}