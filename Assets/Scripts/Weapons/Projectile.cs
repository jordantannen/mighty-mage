using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_maxLifetime = 5f;
    
    private int m_damage;
    private float m_speed;
    private Transform m_direction;
    private GameObjectPool m_pool;

    public void Initialize()
    {
        
    }
}

