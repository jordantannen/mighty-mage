using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private WeaponData m_weaponData;
    [SerializeField] private int m_preallocateCount = 10;
    
    // TODO: Replace with VFX
    [Header("Pulse Visuals")]
    [SerializeField] private float m_pulseVisualDuration = 0.3f;
    [SerializeField] private Color m_pulseColor = Color.cyan;
    
    private GameObjectPool m_projectilePool; 
    private float m_nextFireTime;
    
    private LineRenderer m_pulseLineRenderer;
    
    public void Initialize(WeaponData data)
    {
        m_weaponData = data;
        InitializeWeapon();
    }

    // TODO: Remove this when done debugging
    private void Awake()
    {
        // If weaponData is already set via inspector, initialize now
        if (m_weaponData != null)
        {
            InitializeWeapon();
        }
    }

    private void InitializeWeapon()
    {
        switch (m_weaponData.Type)
        {
            case WeaponData.TargetingType.NearestEnemy:
            case WeaponData.TargetingType.MouseCursor:
                CreatePool();
                break;
            case WeaponData.TargetingType.Orbit:
                CreatePool();
                SpawnOrbitingProjectiles();
                break;
            case WeaponData.TargetingType.PulseAOE:
                CreatePulseLineRenderer();
                break;
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
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    Attack(direction);
                    m_nextFireTime = Time.time + (1f / m_weaponData.FireRate); // 1f is 1 second. 
                }
            } 
            else if (m_weaponData.Type == WeaponData.TargetingType.MouseCursor)
            {
                // TODO: See how this playtests
                if (Keyboard.current.spaceKey.isPressed) 
                {
                    Vector3 direction = FindCursorDirection();
                    if (direction != Vector3.zero)
                    {
                        Attack(direction);
                        m_nextFireTime = Time.time + (1f / m_weaponData.FireRate);
                    }
                }
            }
            else if (m_weaponData.Type == WeaponData.TargetingType.PulseAOE)
            {
                Pulse();
                m_nextFireTime = Time.time + (1f / m_weaponData.FireRate);
            }
        }
    }
    
    private void Attack(Vector3 direction)
    {
        GameObject projectileObj = m_projectilePool.Get();
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (!projectile)
        {
            Debug.LogError($"Projectile component not found on {projectileObj.name}");
            m_projectilePool.Return(projectileObj);
            return;
        }
        
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
        int count = m_weaponData.OrbitProjectileCount;
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
                m_weaponData.Damage,
                m_weaponData.OrbitSpeed,
                m_weaponData.OrbitRadius,
                transform.parent, // Orbit around the player 
                startAngle
            );
        }
    }

    private void Pulse()
    {
        Vector3 center = transform.parent.position;
        Collider[] hits = Physics.OverlapSphere(center, m_weaponData.PulseRadius);
        
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.GetComponent<HealthHandler>().TakeDamage(m_weaponData.Damage);
            }
        }
        
        // Show pulse visual
        StartCoroutine(ShowPulseVisual());
    }
    
    private IEnumerator ShowPulseVisual()
    {
        m_pulseLineRenderer.enabled = true;
        yield return new WaitForSeconds(m_pulseVisualDuration);
        m_pulseLineRenderer.enabled = false;
    }
    
    // For debugging and prototyping
    private void CreatePulseLineRenderer()
    {
        GameObject lineObj = new GameObject("PulseVisual");
        lineObj.transform.SetParent(transform.parent); // Follow the player
        lineObj.transform.localPosition = Vector3.zero; // Center on player
        
        m_pulseLineRenderer = lineObj.AddComponent<LineRenderer>();
        m_pulseLineRenderer.useWorldSpace = false; // Positions are relative to parent
        m_pulseLineRenderer.loop = true;
        m_pulseLineRenderer.startWidth = 0.1f;
        m_pulseLineRenderer.endWidth = 0.1f;
        m_pulseLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_pulseLineRenderer.startColor = m_pulseColor;
        m_pulseLineRenderer.endColor = m_pulseColor;
        
        // Create circle points
        int segments = 32;
        m_pulseLineRenderer.positionCount = segments;
        float angleStep = 360f / segments;
        float radius = m_weaponData.PulseRadius;
        
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0.1f, Mathf.Sin(angle) * radius);
            m_pulseLineRenderer.SetPosition(i, point);
        }
        
        m_pulseLineRenderer.enabled = false; // Hidden until pulse fires
    }
}
