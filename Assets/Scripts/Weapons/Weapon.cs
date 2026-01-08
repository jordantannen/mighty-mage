using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private WeaponData m_weaponData;
    [SerializeField] private int m_preallocateCount = 10;
    
    private WeaponBaseStats m_baseStats;
    private GameObjectPool m_projectilePool; 
    private float m_nextFireTime;
    private Rigidbody m_parentRigidbody;
    private List<OrbitingProjectile> m_activeOrbitingProjectiles = new List<OrbitingProjectile>();
    
    public void Initialize(WeaponData data)
    {
        m_weaponData = data;
        InitializeWeapon();
    }

    // TODO: Remove this when done debugging
    private void Awake()
    {
        // Needed so radial burst can inherit the player's velocity so it doesn't lag behind
        m_parentRigidbody = GetComponentInParent<Rigidbody>();

        // If weaponData is already set via inspector, initialize now
        if (m_weaponData != null)
        {
            InitializeWeapon();
        }
    }

    private void InitializeWeapon()
    {
        m_baseStats = new WeaponBaseStats(m_weaponData);
        
        switch (m_baseStats.Type)
        {
            case WeaponData.TargetingType.NearestEnemy:
            case WeaponData.TargetingType.MouseCursor:
            case WeaponData.TargetingType.RadialBurst:
            case WeaponData.TargetingType.Bouncing:
                CreatePool();
                break;
            case WeaponData.TargetingType.Orbit:
                CreatePool();
                SpawnOrbitingProjectiles();
                break;
        }
    }

    private void CreatePool()
    {
        m_projectilePool = gameObject.AddComponent<GameObjectPool>();
        m_projectilePool.Initialize(m_baseStats.ProjectilePrefab, transform);
        m_projectilePool.Preallocate(m_preallocateCount);
    }
    
    private void Update()
    {
        if (Time.time >= m_nextFireTime)
        {
            if (m_baseStats.Type == WeaponData.TargetingType.NearestEnemy)
            {
                Enemy target = FindNearestEnemy();
                if (target != null)
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    Attack(direction);
                    m_nextFireTime = Time.time + (1f / m_baseStats.FireRate);
                }
            } 
            else if (m_baseStats.Type == WeaponData.TargetingType.MouseCursor)
            {
                // TODO: See how this playtests
                if (Keyboard.current.spaceKey.isPressed) 
                {
                    Vector3 direction = FindCursorDirection();
                    if (direction != Vector3.zero)
                    {
                        Attack(direction);
                        m_nextFireTime = Time.time + (1f / m_baseStats.FireRate);
                    }
                }
            }
            else if (m_baseStats.Type == WeaponData.TargetingType.RadialBurst)
            {
                FireRadialBurst();
                m_nextFireTime = Time.time + (1f / m_baseStats.FireRate);
            }
            else if (m_baseStats.Type == WeaponData.TargetingType.Bouncing)
            {
                Enemy target = FindNearestEnemy();
                if (target != null)
                {
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    AttackBouncing(direction);
                    m_nextFireTime = Time.time + (1f / m_baseStats.FireRate);
                }
            }
        }
    }
    
    private void Attack(Vector3 direction, bool inheritVelocity = false)
    {
        GameObject projectileObj = m_projectilePool.Get();
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (!projectile)
        {
            Debug.LogError($"Projectile component not found on {projectileObj.name}");
            m_projectilePool.Return(projectileObj);
            return;
        }
        
        // Only inherit velocity for radial burst to keep projectiles centered on moving player
        Vector3 inheritedVelocity = Vector3.zero;
        if (inheritVelocity && m_parentRigidbody != null)
        {
            inheritedVelocity = m_parentRigidbody.linearVelocity;
        }

        projectile.Fire(m_baseStats.Damage, m_baseStats.ProjectileSpeed, m_baseStats.KnockbackForce, transform.position, direction, m_projectilePool, inheritedVelocity);
    }

    private void AttackBouncing(Vector3 direction)
    {
        GameObject projectileObj = m_projectilePool.Get();
        BouncingProjectile projectile = projectileObj.GetComponent<BouncingProjectile>();
        if (!projectile)
        {
            Debug.LogError($"BouncingProjectile component not found on {projectileObj.name}. Make sure the prefab has BouncingProjectile, not Projectile.");
            m_projectilePool.Return(projectileObj);
            return;
        }
        
        projectile.Fire(
            m_baseStats.Damage, 
            m_baseStats.ProjectileSpeed,
            m_baseStats.KnockbackForce,
            transform.position, 
            direction, 
            m_projectilePool, 
            m_baseStats.BounceCount, 
            m_baseStats.BounceRange
        );
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closestEnemy = null;
        float closestDistance = m_baseStats.MaxRange;

        foreach (Enemy enemy in allEnemies)
        {
            if (!enemy || !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private Vector3 FindCursorDirection()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = (hitPoint - transform.position).normalized;
            direction.y = 0; // Ensure horizontal
            return direction;
        }
        
        return Vector3.zero;
    }
    
    private void SpawnOrbitingProjectiles()
    {
        int count = m_baseStats.OrbitProjectileCount;
        float angleStep = 360f / count;
        
        for (int i = 0; i < count; i++)
        {
            GameObject projectileObj = m_projectilePool.Get();
            OrbitingProjectile orbitingProjectile = projectileObj.GetComponent<OrbitingProjectile>();
            
            if (!orbitingProjectile)
            {
                Debug.LogError($"OrbitingProjectile component not found on {projectileObj.name}. Make sure the prefab has OrbitingProjectile, not Projectile.");
                m_projectilePool.Return(projectileObj);
                continue;
            }
            
            float startAngle = i * angleStep;
            orbitingProjectile.Initialize(
                m_baseStats.Damage,
                m_baseStats.KnockbackForce,
                m_baseStats.OrbitSpeed,
                m_baseStats.OrbitRadius,
                transform.parent, // Orbit around the player 
                startAngle
            );
            
            m_activeOrbitingProjectiles.Add(orbitingProjectile);
        }
    }

    private void FireRadialBurst()
    {
        int count = m_baseStats.BurstProjectileCount;
        float angleStep = 360f / count;
        
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Attack(direction, inheritVelocity: true);
        }
    }

    private void RespawnOrbitingProjectiles()
    {
        // Return all active orbiting projectiles to pool
        foreach (var projectile in m_activeOrbitingProjectiles)
        {
            if (projectile != null && projectile.gameObject.activeInHierarchy)
            {
                m_projectilePool.Return(projectile.gameObject);
            }
        }
        m_activeOrbitingProjectiles.Clear();
        
        // Spawn fresh with updated stats
        SpawnOrbitingProjectiles();
    }

    /// <summary>
    /// Applies an upgrade to this weapon's stats.
    /// Returns true if the upgrade was successfully applied.
    /// </summary>
    public bool ApplyUpgrade(UpgradeData upgrade)
    {
        StatType? modifiedStat = m_baseStats.ApplyUpgrade(upgrade);
        
        if (modifiedStat == null)
        {
            return false;
        }
        
        // For orbit weapons, respawn projectiles when orbit-related stats change
        if (m_baseStats.Type == WeaponData.TargetingType.Orbit)
        {
            bool isOrbitStat = modifiedStat == StatType.OrbitProjectileCount ||
                               modifiedStat == StatType.OrbitRadius ||
                               modifiedStat == StatType.OrbitSpeed ||
                               modifiedStat == StatType.Damage;
            
            if (isOrbitStat)
            {
                RespawnOrbitingProjectiles();
            }
        }
        
        return true;
    }
}
