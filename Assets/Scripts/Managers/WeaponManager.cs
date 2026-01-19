using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// NOTE TO SELF: Upgrades are defined on type, they probably should have a unique ID or name to bind upgrades
// to specific weapons. Doesn't matter given the scope, but will need a change if extending the game.

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<Weapon> weaponPrefabs = new List<Weapon>();
    [SerializeField] private List<UpgradeData> m_upgradeData = new List<UpgradeData>();
    [SerializeField] private GameObject player;
    [SerializeField] private UpgradeSelection m_upgradeSelection;
    
    private List<Weapon> m_availableWeapons = new List<Weapon>();
    private List<Weapon> m_equippedWeapons = new List<Weapon>();
    private HashSet<string> m_appliedUpgrades = new HashSet<string>();
    
    private void Start()
    {
        m_availableWeapons.AddRange(weaponPrefabs);
    }
    
    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame) 
        {
            EquipRandomWeapon();
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            ApplyRandomUpgrade();
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            m_upgradeSelection.DisplayUpgrades(CreateUpgradeList(3));
        }
    }
    
    private void EquipRandomWeapon()
    {
        if (m_availableWeapons.Count == 0)
        {
            Debug.Log("Player already has all weapons equipped!");
            return;
        }
        
        int randomIndex = Random.Range(0, m_availableWeapons.Count);
        Weapon weaponPrefab = m_availableWeapons[randomIndex];
        m_availableWeapons.RemoveAt(randomIndex);
        
        GameObject weaponInstance = Instantiate(weaponPrefab.gameObject, player.transform);
        m_equippedWeapons.Add(weaponInstance.GetComponent<Weapon>());
    }
    
    private void ApplyRandomUpgrade()
    {
        if (m_equippedWeapons.Count == 0)
        {
            Debug.Log("No weapons equipped to upgrade.");
            return;
        }
        
        List<UpgradeData> availableUpgrades = m_upgradeData
            .Where(upgrade => !m_appliedUpgrades.Contains(upgrade.UpgradeName))
            .Where(upgrade => m_equippedWeapons.Any(weapon => upgrade.CanApplyTo(weapon.WeaponType)))
            .ToList();
        
        if (availableUpgrades.Count == 0)
        {
            Debug.Log("All available upgrades have been applied!");
            return;
        }
        
        UpgradeData upgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
        Weapon targetWeapon = m_equippedWeapons.FirstOrDefault(weapon => weapon && upgrade.CanApplyTo(weapon.WeaponType));
        
        if (targetWeapon && targetWeapon.ApplyUpgrade(upgrade))
        {
            m_appliedUpgrades.Add(upgrade.UpgradeName);
            Debug.Log($"Applied '{upgrade.UpgradeName}' to {targetWeapon.WeaponType} weapon");
        }
        else
        {
            Debug.LogWarning($"Failed to apply upgrade '{upgrade.UpgradeName}' - no matching weapon found");
        }
    }
    
    public List<UpgradeData> CreateUpgradeList(int listSize)
    {
        List<UpgradeData> result = new List<UpgradeData>();
        
        if (m_equippedWeapons.Count == 0)
        {
            Debug.Log("No weapons equipped - cannot create upgrade list.");
            return result;
        }
        
        List<UpgradeData> availableUpgrades = m_upgradeData
            .Where(upgrade => !m_appliedUpgrades.Contains(upgrade.UpgradeName))
            .Where(upgrade => m_equippedWeapons.Any(weapon => upgrade.CanApplyTo(weapon.WeaponType)))
            .ToList();
        
        int count = Mathf.Min(listSize, availableUpgrades.Count);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            result.Add(availableUpgrades[randomIndex]);
            availableUpgrades.RemoveAt(randomIndex);
        }
        return result;
    }
    
}
