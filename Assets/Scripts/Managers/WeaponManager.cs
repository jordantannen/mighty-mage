using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

// NOTE TO SELF: Upgrades are defined on type, they probably should have a unique ID or name to bind upgrades
// to specific weapons. Doesn't matter given the scope, but will need a change if extending the game.

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<Weapon> weaponPrefabs = new List<Weapon>();
    [SerializeField] private List<UpgradeData> m_upgradeData = new List<UpgradeData>();
    [SerializeField] private GameObject player;
    [FormerlySerializedAs("m_upgradeSelection")] [SerializeField] private UpgradeSelectionUI m_upgradeSelectionUI;
    
    private HashSet<Weapon> m_availableWeapons = new HashSet<Weapon>();
    private HashSet<Weapon> m_equippedWeapons = new HashSet<Weapon>();
    private HashSet<string> m_appliedUpgrades = new HashSet<string>();
    
    private void Awake()
    {
        foreach (var weapon in weaponPrefabs)
        {
            m_availableWeapons.Add(weapon);
        }
    }

    private void OnEnable()
    {
        if (m_upgradeSelectionUI != null)
        {
            m_upgradeSelectionUI.OnUpgradeChosen += ApplyUpgrade;
            m_upgradeSelectionUI.OnWeaponChosen += EquipWeapon;
        }
    }

    private void OnDestroy()
    {
        if (m_upgradeSelectionUI != null)
        {
            m_upgradeSelectionUI.OnUpgradeChosen -= ApplyUpgrade;
            m_upgradeSelectionUI.OnWeaponChosen -= EquipWeapon;
        }
    }
    
    // FOR DEBUGGING!!! REMEMBER TO REMOVE!
    private void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            m_upgradeSelectionUI.DisplayWeapons(m_availableWeapons);
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            m_upgradeSelectionUI.DisplayUpgrades(CreateUpgradeList(3));
        }
    }
    
    public void EquipWeapon(Weapon weaponPrefab)
    {
        if (!m_availableWeapons.Contains(weaponPrefab))
        {
            Debug.LogWarning($"Weapon '{weaponPrefab.WeaponName}' is not available to equip.");
            return;
        }
        
        m_availableWeapons.Remove(weaponPrefab);
        
        GameObject weaponInstance = Instantiate(weaponPrefab.gameObject, player.transform);
        m_equippedWeapons.Add(weaponInstance.GetComponent<Weapon>());
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
    
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        Weapon targetWeapon = m_equippedWeapons.FirstOrDefault(weapon => weapon && upgrade.CanApplyTo(weapon.WeaponType));
        
        if (targetWeapon && targetWeapon.ApplyUpgrade(upgrade))
        {
            m_appliedUpgrades.Add(upgrade.UpgradeName);
        }
        else
        {
            Debug.LogWarning($"Failed to apply upgrade '{upgrade.UpgradeName}' - no matching weapon found");
        }
    }

    public HashSet<Weapon> GetAvailableWeapons()
    {
        return m_availableWeapons;
    }
}
