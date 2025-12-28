using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private WeaponData m_weaponData;
    [SerializeField] private GameObjectPool m_projectilePool;

    private float m_nextFireTime;
    
    private void Update()
    {
        if (Time.time >= m_nextFireTime)
        {
            if (m_weaponData.Type == WeaponData.TargetingType.NearestEnemy)
            {
                Enemy target = FindNearestEnemy();
                if (target != null)
                {
                    Fire(target);
                    m_nextFireTime = Time.time + (1f / m_weaponData.FireRate);
                }
            }
        }
    }

    private void Fire(Enemy target)
    {
        GameObject projectile = m_projectilePool.Get();
        
        // fire projectile
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
