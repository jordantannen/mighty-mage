using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class UpgradeSelection : MonoBehaviour
{
    [SerializeField] private int numberOfButtons = 3;
    
    private VisualElement m_container;
    private UIDocument m_uiDocument;
    private bool isVisible = false;
    
    private void Start()
    {
        m_uiDocument = GetComponent<UIDocument>();
        m_container = m_uiDocument.rootVisualElement.Q<VisualElement>("Container");
        
        Hide();
    }

    // private void Update()
    // {
    //     if (Keyboard.current.gKey.wasPressedThisFrame) 
    //     {
    //         if (isVisible)
    //         {
    //             isVisible = false;
    //             Hide();
    //         }
    //         else
    //         {
    //             isVisible = true;
    //             Show();
    //         }
    //     }
    // }

    public void Show()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    
    public void Hide()
    {
        m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }
    
    public void DisplayUpgrades(List<UpgradeData> upgrades)
    {
        m_container.Clear();
        
        foreach (UpgradeData upgrade in upgrades)
        {
            Button button = new Button();
            button.text = upgrade.UpgradeName;
            button.AddToClassList("button");
            
            // Capture the upgrade for the click handler
            UpgradeData capturedUpgrade = upgrade;
            button.clicked += () => OnUpgradeSelected(capturedUpgrade);
            
            m_container.Add(button);
        }
        
        Show();
    }
    
    private void OnUpgradeSelected(UpgradeData upgrade)
    {
        Debug.Log($"Selected upgrade: {upgrade.UpgradeName}");
        // TODO: Apply the upgrade via WeaponManager
        Hide();
    }
    
    private void GenerateButtons(int count)
    {
        m_container.Clear();
        for (int i = 0; i < count; i++)
        {
            Button button = new Button();
            button.text = $"Option {i + 1}";
            button.AddToClassList("button");
            
            button.clicked += () => Debug.Log("Button clicked!");
            
            m_container.Add(button);
        }
    }
}
