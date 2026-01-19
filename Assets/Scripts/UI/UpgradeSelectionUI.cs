using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Serialization;


public class UpgradeSelectionUI : MonoBehaviour
{
    [FormerlySerializedAs("numberOfButtons")] [SerializeField] private int m_numberOfButtons = 3;
    
    private VisualElement m_container;
    private UIDocument m_uiDocument;
    
    public event Action<UpgradeData> OnUpgradeChosen;
    public event Action<Weapon> OnWeaponChosen;
    
    private void Start()
    {
        m_uiDocument = GetComponent<UIDocument>();
        m_container = m_uiDocument.rootVisualElement.Q<VisualElement>("Container");
        Hide();
    }
    
    public void Show()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    
    public void Hide()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }
    
    public void DisplayUpgrades(IEnumerable<UpgradeData> upgrades)
    {
        m_container.Clear();
        
        foreach (UpgradeData upgrade in upgrades)
        {
            Button button = new Button();
            button.AddToClassList("button");
            
            // Add upgrade name at the top
            Label nameLabel = new Label(upgrade.UpgradeName);
            nameLabel.AddToClassList("item-name");
            button.Add(nameLabel);
            
            // Add icon in the middle
            if (upgrade.Icon != null)
            {
                VisualElement icon = new VisualElement();
                icon.AddToClassList("item-icon");
                icon.style.backgroundImage = new StyleBackground(upgrade.Icon);
                button.Add(icon);
            }
            
            // Add description at the bottom
            Label descLabel = new Label(upgrade.Description);
            descLabel.AddToClassList("item-description");
            button.Add(descLabel);
            
            UpgradeData capturedUpgrade = upgrade;
            button.clicked += () => OnUpgradeSelected(capturedUpgrade);
            
            m_container.Add(button);
        }
        Show();
    }

    public void DisplayWeapons(IEnumerable<Weapon> weapons)
    {
        m_container.Clear();
        
        foreach (Weapon weapon in weapons)
        {
            Button button = new Button();
            button.AddToClassList("button");
            
            // Add weapon name at the top
            Label nameLabel = new Label(weapon.WeaponName);
            nameLabel.AddToClassList("item-name");
            button.Add(nameLabel);
            
            // Add icon in the middle
            if (weapon.Icon != null)
            {
                VisualElement icon = new VisualElement();
                icon.AddToClassList("item-icon");
                icon.style.backgroundImage = new StyleBackground(weapon.Icon);
                button.Add(icon);
            }
            
            // Add description at the bottom
            Label descLabel = new Label(weapon.Description);
            descLabel.AddToClassList("item-description");
            button.Add(descLabel);
            
            Weapon capturedWeapon = weapon;
            button.clicked += () => OnWeaponSelected(capturedWeapon);
            
            m_container.Add(button);
        }
        Show();
    }
    
    private void OnUpgradeSelected(UpgradeData upgrade)
    {
        Debug.Log($"Selected upgrade: {upgrade.UpgradeName}");
        OnUpgradeChosen?.Invoke(upgrade);
        Hide();
    }
    
    private void OnWeaponSelected(Weapon weapon)
    {
        Debug.Log($"Selected weapon: {weapon.WeaponName}");
        OnWeaponChosen?.Invoke(weapon);
        Hide();
    }
}
