# Mighty Mage

A fast-paced HD-2D _Brotato_-like action game where you play as a powerful mage fending off endless waves of enemies using an arsenal of magical weapons. Originally created for the 2025 [Secret Santa Game Jam](https://itch.io/jam/secret-santa-2025). 

<br>
<p align="center">
<img width="637" height="346" alt="2026-08-29 15 41 50" src="https://github.com/user-attachments/assets/5b94c9f8-d115-4643-a109-ded7c72fc4f1" />
</p>

## Gameplay

Survive increasingly difficult waves of enemies while earning powerful weapons and upgrades between rounds. 

### Features

- **5 Unique Weapon Types**
  - 🎯 **Auto-Aim** – Automatically targets nearest enemy
  - 🖱️ **Cursor-Based** – Fire toward mouse position
  - 🔄 **Orbital** – Projectiles orbit around the player
  - 💥 **Radial Burst** – Fire projectiles in all directions
  - ⛓️ **Chain Bounce** – Heat-seeking projectiles that bounce between enemies

- **Dynamic Upgrade System** – Percentage and flat stat modifiers with weapon-type filtering
- **Wave-Based Survival** – Progressive difficulty with configurable spawn rates per round
- **Physics-Based Combat** – Knockback, invincibility frames, and collision-based damage

## Technical Highlights

### Object Pooling
Custom `GameObjectPool` implementation using Unity's `ObjectPool<T>` to eliminate runtime allocations. Includes pre-allocation support for zero GC pressure during gameplay.

```csharp
// Pre-warm the pool during loading
m_projectilePool.Preallocate(20);

// Zero allocations during gameplay
GameObject projectile = m_pool.Get();
m_pool.Return(projectile);
```

### Data-Driven Design
All weapons and upgrades are defined as **ScriptableObjects**, enabling designers to create and balance content without code changes.

```csharp
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/New Weapon")]
public class WeaponData : ScriptableObject
{
    public TargetingType Type;
    public int Damage;
    public float FireRate;
    // ...
}
```

### Event-Driven Architecture
Decoupled systems communicate through C# events/delegates:

```csharp
// HealthHandler broadcasts death
public event Action OnDeath;

// GameManager subscribes
m_playerController.OnPlayerDeath += GameOver;
```
