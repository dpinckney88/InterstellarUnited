using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponModChipEventArgs : EventArgs
{
    public string description;
}

public class WeaponModChip : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject weapon;
    private bool parentedToModificationWindow = true;
    public int cost;
    private bool isSelected = false;
    private bool isConsideredForPurchase = false;
    public static event EventHandler<WeaponModChipEventArgs> WeaponModChipEvent;
    public void OnDrag(PointerEventData eventData)
    {
        //See if we can purchase the modificaiton first.
        if (!purchaseCheck() && parentedToModificationWindow)
        {
            Debug.Log("Not enough funds!");
            return;
        }
        isSelected = true;
        //Allows the chip to move out of the content mask
        if (parentedToModificationWindow)
        {
            parentedToModificationWindow = false;
            GameObject shipPlate = GameObject.Find("Ship Modification/Ship Plate");
            transform.SetParent(shipPlate.transform, false);
        }

        Debug.Log("Dragging!!");
        Vector3 mousePos = Input.mousePosition;
        gameObject.transform.position = mousePos;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && isSelected)
        {
            isSelected = false;
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, 1f);
            WeaponSlot modSlot = hit.collider.GetComponent<WeaponSlot>();
            if (hit.collider != null && modSlot != null)
            {
                modSlot.SetNewWeapon(weapon);
                //Only charge for this if the modification wasn't arleady being considered for purchase. In other words, we do not charge if just moving from one slot to another.
                if (!isConsideredForPurchase)
                {
                    ShipModificationView.instance.ManageCost(cost, true);
                }
                isConsideredForPurchase = true;
                transform.position = CenterOnWeaponSlot(modSlot);
            }
            else
            {
                //Put the chip back to the purchase list if not on a weapon slot
                GameObject content = GameObject.Find("Ship Modification/PurchaseableUpgrades/Mask/Content");
                transform.SetParent(content.transform, false);
                isConsideredForPurchase = false;
                ShipModificationView.instance.ManageCost(cost, false);
            }
        }
    }

    //Check to see if the chip is able to be purchased.
    private bool purchaseCheck()
    {
        if (ShipModificationView.instance.totalCostValue + cost > PlayerVault.instance.money)
        {
            return false;
        }
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Dispaly the description of the weapon in the Ship Modification's view.
        WeaponModChipEvent?.Invoke(this, new WeaponModChipEventArgs() { description = weapon.GetComponent<IWeapon>().weaponDescription });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WeaponModChipEvent?.Invoke(this, new WeaponModChipEventArgs() { description = null });
    }

    private Vector2 CenterOnWeaponSlot(WeaponSlot ws)
    {
        float xOffset = gameObject.GetComponent<RectTransform>().rect.width / 10;
        float yOffset = gameObject.GetComponent<RectTransform>().rect.height / 10;
        Vector2 centerPoint = ws.transform.position;
        return new Vector2(centerPoint.x + xOffset, centerPoint.y - yOffset);
    }
}
