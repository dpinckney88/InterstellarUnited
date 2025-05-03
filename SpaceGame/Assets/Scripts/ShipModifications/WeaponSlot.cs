using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public GameObject weaponSlot;
    private GameObject weaponToAdd;
    private int chipCost;
    // Start is called before the first frame update
    void Start()
    {
        ShipModificationView.confirmWeponModification += ConfirmWeapon;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetNewWeapon(GameObject weapon)
    {
        weaponToAdd = weapon;
    }

    private void ConfirmWeapon(object sender, EventArgs e)
    {
        if (weaponToAdd != null)
        {
            GameObject tempWeapon = GameObject.Instantiate(weaponToAdd);
            tempWeapon.transform.SetParent(weaponSlot.transform, false);
        }
    }

    void OnDestroy()
    {
        ShipModificationView.confirmWeponModification -= ConfirmWeapon;
    }
}
