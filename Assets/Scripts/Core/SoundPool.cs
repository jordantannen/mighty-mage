using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// A singleton pool for playing one-shot sounds without interruption.
/// Sounds are played on pooled AudioSource objects that automatically return to the pool when finished.
/// </summary>
public class SoundPool : MonoBehaviour
{
    public static SoundPool Instance { get; private set; }

    [SerializeField] private int m_defaultCapacity = 10;
    [SerializeField] private int m_maxSize = 50;

    private ObjectPool<AudioSource> m_pool;
    private Transform m_soundParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create a parent for organization
        m_soundParent = new GameObject("PooledSounds").transform;
        m_soundParent.SetParent(transform);

        m_pool = new ObjectPool<AudioSource>(
            createFunc: CreateAudioSource,
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: OnDestroyAudioSource,
            collectionCheck: true,
            defaultCapacity: m_defaultCapacity,
            maxSize: m_maxSize
        );
    }

    /// <summary>
    /// Plays a sound effect at a specified position with optional pitch variation.
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="position">World position to play the sound at</param>
    /// <param name="volume">Volume of the sound (0-1)</param>
    /// <param name="pitch">Pitch of the sound (1 = normal)</param>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = m_pool.Get();
        source.transform.position = position;
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(clip);

        // Return to pool after clip finishes
        StartCoroutine(ReturnAfterPlay(source, clip.length / pitch));
    }

    private System.Collections.IEnumerator ReturnAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration + 0.1f); // Small buffer
        
        if (source != null && source.gameObject.activeInHierarchy)
        {
            m_pool.Release(source);
        }
    }

    private AudioSource CreateAudioSource()
    {
        GameObject obj = new GameObject("PooledAudioSource");
        obj.transform.SetParent(m_soundParent);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; 
        return source;
    }

    private void OnGetFromPool(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }

    private void OnReturnToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
    }

    private void OnDestroyAudioSource(AudioSource source)
    {
        if (source != null)
        {
            Destroy(source.gameObject);
        }
    }
}

