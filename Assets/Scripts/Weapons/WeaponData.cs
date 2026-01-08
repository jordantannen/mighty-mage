using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/New Weapon")]
public class WeaponData : ScriptableObject
{
    public enum TargetingType
    {
        NearestEnemy,
        MouseCursor,
        Orbit,
        RadialBurst,
        Bouncing
    }
    
    [Header("General")]
    [SerializeField] private string m_weaponName;
    [SerializeField, TextArea] private string m_description;
    [SerializeField] private Sprite m_icon;

    [Header("Targeting")]
    [SerializeField] private TargetingType m_targetingType;
    [SerializeField] private float m_maxRange = 5f;

    [Header("Stats")]
    [SerializeField] private int m_damage = 10;
    [SerializeField] private float m_fireRate = 1f;
    [SerializeField] private float m_projectileSpeed = 10f;
    [SerializeField] private float m_knockbackForce = 5f;

    [Header("Projectile")]
    [SerializeField] private GameObject m_projectilePrefab;

    [Header("Orbit Settings")]
    [SerializeField] private float m_orbitRadius = 2f;
    [SerializeField] private float m_orbitSpeed = 180f; // Degrees per second
    [SerializeField] private int m_orbitProjectileCount = 3;

    [Header("Radial Burst Settings")]
    [SerializeField] private int m_burstProjectileCount = 8;

    [Header("Bounce Settings")]
    [SerializeField] private int m_bounceCount = 3;
    [SerializeField] private float m_bounceRange = 5f;

    // Public accessors
    
    // General
    public string WeaponName => m_weaponName;
    public string Description => m_description;
    public Sprite Icon => m_icon;
    
    // Targeting
    public TargetingType Type => m_targetingType;
    public float MaxRange => m_maxRange;
    
    // Stats
    public int Damage => m_damage;
    public float FireRate => m_fireRate;
    public float ProjectileSpeed => m_projectileSpeed;
    public float KnockbackForce => m_knockbackForce;
    
    // Projectile
    public GameObject ProjectilePrefab => m_projectilePrefab;
    
    // Orbit
    public float OrbitRadius => m_orbitRadius;
    public float OrbitSpeed => m_orbitSpeed;
    public int OrbitProjectileCount => m_orbitProjectileCount;
    
    // Radial Burst
    public int BurstProjectileCount => m_burstProjectileCount;
    
    // Bounce
    public int BounceCount => m_bounceCount;
    public float BounceRange => m_bounceRange;
}
