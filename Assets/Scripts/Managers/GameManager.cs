using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private WeaponManager m_weaponManager;
    [SerializeField] private UpgradeSelectionUI m_upgradeSelectionUI;
    
    
    [Header("Game Settings")]
    [SerializeField] private float m_roundDuration = 15f;
    [SerializeField] private int m_upgradesPerRound = 3;
    [SerializeField] private List<int> m_weaponUpgradeRounds;
    [SerializeField] private Weapon m_starterWeapon;
    
    private int m_currentRound = 1;
    private float m_timeRemaining;

    public enum GameState
    {
        InRound,
        UpgradeSelection,
        WeaponSelection,
        GameOver,
    }

    private GameState m_currentState;
    
    private void OnEnable()
    {
        Time.timeScale = 0f;
        if (m_upgradeSelectionUI != null)
        {
            m_upgradeSelectionUI.OnUpgradeChosen += OnUpgradeSelected;
            m_upgradeSelectionUI.OnWeaponChosen += OnWeaponSelected;
        }
    }
    
    private void OnDisable()
    {
        if (m_upgradeSelectionUI != null)
        {
            m_upgradeSelectionUI.OnUpgradeChosen -= OnUpgradeSelected;
            m_upgradeSelectionUI.OnWeaponChosen -= OnWeaponSelected;
        }
    }
    
    private void Start()
    {
        m_weaponManager.EquipWeapon(m_starterWeapon);
        StartRound();
    }
    
    private void Update()
    {
        if (m_currentState == GameState.InRound)
        {
            m_timeRemaining -= Time.deltaTime;
            if (m_timeRemaining <= 0)
            {
                m_timeRemaining = 0;
                EndRound();
            }
        }
    }
    
    private void StartRound()
    {
        Time.timeScale = 1f;
        m_timeRemaining = m_roundDuration;
        m_currentState = GameState.InRound;
        Debug.Log($"Starting round {m_currentRound}");
    }

    private void EndRound()
    {
        Debug.Log($"Completed round {m_currentRound}");
        if (m_weaponUpgradeRounds.Contains(m_currentRound))
        {
            SelectWeapons();
        }
        else
        {
            SelectUpgrades();
        }
        m_currentRound++;
    }

    private void SelectUpgrades()
    {
        m_currentState = GameState.UpgradeSelection;
        
        List<UpgradeData> upgradeList = m_weaponManager.CreateUpgradeList(m_upgradesPerRound);
        if (upgradeList.Count != 0)
        {
            m_upgradeSelectionUI.DisplayUpgrades(upgradeList);
            Time.timeScale = 0f;
        }
        else
        {
            // No upgrades available, start next round
            StartRound();
        }
    }

    private void SelectWeapons()
    {
        m_currentState = GameState.WeaponSelection;
        
        HashSet<Weapon> weaponList = m_weaponManager.GetAvailableWeapons();
        if (weaponList.Count != 0)
        {
            m_upgradeSelectionUI.DisplayWeapons(weaponList);
            Time.timeScale = 0f;
        }
        else
        {
            // No weapons available, start next round
            StartRound();
        }
    }
    
    private void OnUpgradeSelected(UpgradeData upgrade)
    {
        StartRound();
    }
    
    private void OnWeaponSelected(Weapon weapon)
    {
        StartRound();
    }
}
