using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private WeaponData m_weaponData;
    [SerializeField] private int m_preallocateCount = 10;
    
    private GameObjectPool m_projectilePool; 
    private float m_nextFireTime;

    public void Initialize(WeaponData data)
    {
        m_weaponData = data;
        CreatePool();
    }

    private void Awake()
    {
        // If weaponData is already set via inspector, create pool now
        if (m_weaponData != null)
        {
            CreatePool();
        }
    }

    private void CreatePool()
    {
        m_projectilePool = gameObject.AddComponent<GameObjectPool>();
        m_projectilePool.Initialize(m_weaponData.ProjectilePrefab, transform);
        m_projectilePool.Preallocate(m_preallocateCount);
    }
    
    private void Update()
    {
        if (Time.time >= m_nextFireTime)
        {
            if (m_weaponData.Type == WeaponData.TargetingType.NearestEnemy)
            {
                Enemy target = FindNearestEnemy();
                if (target != null)
                {
                    Attack(target);
                    m_nextFireTime = Time.time + (1f / m_weaponData.FireRate);
                }
            }
        }
    }

    private void Attack(Enemy target)
    {
        GameObject projectileObj = m_projectilePool.Get();
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (!projectile)
        {
            Debug.LogError($"Projectile component not found on {projectileObj.name}");
            m_projectilePool.Return(projectileObj);
            return;
        }
        
        Vector3 direction = (target.transform.position - transform.position).normalized;
        projectile.Fire(m_weaponData.Damage, m_weaponData.ProjectileSpeed, transform.position, direction, m_projectilePool);
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closestEnemy = null;
        float closestDistance = m_weaponData.MaxRange;

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
}
