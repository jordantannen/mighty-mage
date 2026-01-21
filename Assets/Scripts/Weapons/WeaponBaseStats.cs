using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime wrapper for WeaponData that tracks per-weapon upgrade modifiers.
/// This allows WeaponData to remain immutable.
/// </summary>
public class WeaponBaseStats
{
    private readonly WeaponData m_baseData;
    private readonly Dictionary<StatType, float> m_percentMods = new();
    private readonly Dictionary<StatType, float> m_flatMods = new();

    public WeaponBaseStats(WeaponData baseData)
    {
        m_baseData = baseData;
    }

    /// <summary>
    /// Applies an upgrade to this weapon's stats.
    /// Returns the StatType that was modified, or null if the upgrade couldn't be applied.
    /// </summary>
    public StatType? ApplyUpgrade(UpgradeData upgrade)
    {
        if (!upgrade.CanApplyTo(m_baseData.Type))
        {
            Debug.LogWarning($"Upgrade '{upgrade.UpgradeName}' cannot be applied to weapon type '{m_baseData.Type}'");
            return null;
        }

        if (upgrade.Type == UpgradeType.Percentage)
        {
            m_percentMods.TryGetValue(upgrade.Stat, out float current);
            m_percentMods[upgrade.Stat] = current + upgrade.Value;
        }
        else
        {
            m_flatMods.TryGetValue(upgrade.Stat, out float current);
            m_flatMods[upgrade.Stat] = current + upgrade.Value;
        }

        return upgrade.Stat;
    }

    /// <summary>
    /// Calculates a stat value with modifiers applied.
    /// Formula: base * (1 + percentMod/100) + flatMod
    /// Note: percentMod is stored as whole numbers (e.g., 20 = 20%)
    /// </summary>
    private float CalculateStat(float baseValue, StatType stat)
    {
        m_percentMods.TryGetValue(stat, out float percentMod);
        m_flatMods.TryGetValue(stat, out float flatMod);
        return baseValue * (1f + percentMod / 100f) + flatMod;
    }

    private int CalculateStatInt(int baseValue, StatType stat)
    {
        m_percentMods.TryGetValue(stat, out float percentMod);
        m_flatMods.TryGetValue(stat, out float flatMod);
        return Mathf.RoundToInt(baseValue * (1f + percentMod / 100f) + flatMod);
    }

    // ============================================
    // Computed Stat Getters (base + modifiers)
    // ============================================

    // General Stats
    public int Damage => CalculateStatInt(m_baseData.Damage, StatType.Damage);
    public float FireRate => CalculateStat(m_baseData.FireRate, StatType.FireRate);
    public float ProjectileSpeed => CalculateStat(m_baseData.ProjectileSpeed, StatType.ProjectileSpeed);
    public float MaxRange => CalculateStat(m_baseData.MaxRange, StatType.MaxRange);
    public float KnockbackForce => CalculateStat(m_baseData.KnockbackForce, StatType.KnockbackForce);

    // Orbit Stats
    public float OrbitRadius => CalculateStat(m_baseData.OrbitRadius, StatType.OrbitRadius);
    public float OrbitSpeed => CalculateStat(m_baseData.OrbitSpeed, StatType.OrbitSpeed);
    public int OrbitProjectileCount => CalculateStatInt(m_baseData.OrbitProjectileCount, StatType.OrbitProjectileCount);

    // Radial Burst Stats
    public int BurstProjectileCount => CalculateStatInt(m_baseData.BurstProjectileCount, StatType.BurstProjectileCount);

    // Bounce Stats
    public int BounceCount => CalculateStatInt(m_baseData.BounceCount, StatType.BounceCount);
    public float BounceRange => CalculateStat(m_baseData.BounceRange, StatType.BounceRange);

    // ============================================
    // Pass-through Properties (non-stat fields)
    // ============================================

    public WeaponData.TargetingType Type => m_baseData.Type;
    public GameObject ProjectilePrefab => m_baseData.ProjectilePrefab;
    public string WeaponName => m_baseData.WeaponName;
    public string Description => m_baseData.Description;
    public Sprite Icon => m_baseData.Icon;
}
