using UnityEngine;

public enum UpgradeType
{
    Percentage,
    Flat
}

public enum StatType
{
    Damage,
    FireRate,
    ProjectileSpeed,
    MaxRange,
    KnockbackForce,
    OrbitRadius,
    OrbitSpeed,
    OrbitProjectileCount,
    BurstProjectileCount,
    BounceCount,
    BounceRange
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Weapons/New Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Display")]
    [SerializeField] private string m_upgradeName;
    [SerializeField] private string m_description;
    [SerializeField] private Sprite m_icon;

    [Header("Upgrade Settings")]
    [SerializeField] private UpgradeType m_upgradeType;
    [SerializeField] private StatType m_statType;
    [SerializeField] private float m_value;

    [Header("Weapon Filter")]
    [Tooltip("Leave empty to apply to all weapon types. Otherwise, only applies to listed types.")]
    [SerializeField] private WeaponData.TargetingType[] m_applicableWeaponTypes;

    // Public accessors
    public string UpgradeName => m_upgradeName;
    public string Description => m_description;
    public Sprite Icon => m_icon;
    public UpgradeType Type => m_upgradeType;
    public StatType Stat => m_statType;
    public float Value => m_value;
    public WeaponData.TargetingType[] ApplicableWeaponTypes => m_applicableWeaponTypes;

    /// <summary>
    /// Checks if this upgrade can be applied to the given weapon type.
    /// Returns true if no filter is set (applies to all) or if the weapon type is in the filter list.
    /// </summary>
    public bool CanApplyTo(WeaponData.TargetingType weaponType)
    {
        // Empty array means applies to all weapon types
        if (m_applicableWeaponTypes == null || m_applicableWeaponTypes.Length == 0)
        {
            return true;
        }

        foreach (var applicableType in m_applicableWeaponTypes)
        {
            if (applicableType == weaponType)
            {
                return true;
            }
        }

        return false;
    }
}

