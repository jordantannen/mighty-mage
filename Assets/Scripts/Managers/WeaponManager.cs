using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<Weapon> weaponPrefabs = new List<Weapon>();
    [SerializeField] private GameObject player;
    
    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame) 
        {
            // Instantiate a weapon prefab as a child of the player
            GameObject weaponInstance = Instantiate(weaponPrefabs[0].gameObject, player.transform);
        }
    }
}
