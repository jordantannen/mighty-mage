using UnityEngine;
using UnityEngine.Serialization;

public class FireFlicker : MonoBehaviour
{
    private Light m_fireLight;
    private Vector3 m_initialPosition;
    
    [Header("Flicker Intensity")]
    public float m_minIntensity = 1f;
    public float m_maxIntensity = 3f;
    public float m_flickerSpeed = 1f;
    
    // Random offsets to ensure the motion doesn't match the flicker perfectly
    private float m_randomOffset;

    void Start()
    {
        m_fireLight = GetComponent<Light>();
        m_initialPosition = transform.position; 
        m_randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // PerlinNoise: https://www.reddit.com/r/gamedev/comments/2d284n/understanding_perlin_noise_an_indepth_look_at_the/
        // Lerp: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#:~:text=What%20is%20Lerp%20in%20Unity,However%E2%80%A6

        float noise = Mathf.PerlinNoise(Time.time * m_flickerSpeed, m_randomOffset);
        m_fireLight.intensity = Mathf.Lerp(m_minIntensity, m_maxIntensity, noise);
    }
}