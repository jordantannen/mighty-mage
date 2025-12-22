using UnityEngine;

public class TreeSway : MonoBehaviour
{
    public float m_rotationSpeed = 1.0f;
    public float m_rotationAmount = 5.0f; 
    
    // Random offset so all trees don't sway in perfect unison
    private float m_randomOffset;

    void Start()
    {
        m_randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Simple Harmonic Motion https://www.youtube.com/watch?v=m463X1cqV6s
        float zRotation = Mathf.Sin((Time.time + m_randomOffset) * m_rotationSpeed) * m_rotationAmount;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}