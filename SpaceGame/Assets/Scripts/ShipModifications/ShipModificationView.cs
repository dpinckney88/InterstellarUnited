using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipModificationView : MonoBehaviour
{
    public static ShipModificationView instance;
    public List<GameObject> availableModifications = new List<GameObject>();
    [SerializeField] private GameObject weaponModificationContent;
    public static event EventHandler confirmWeponModification;
    [SerializeField] private TMP_Text credits, totalCost, weaponDescription;
    public int totalCostValue { get; private set; } = 0;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        WeaponModChip.WeaponModChipEvent += DisplayWeaponDescription;
        List<String> mods = new List<String>() { "BasicLaser", "BasicLaser", "BasicLaser", "Large Blaster", "Large Blaster" };
        GenerateModifications(mods);
        displayCredits();
        displayTotalCost(0);
    }

    private void DisplayWeaponDescription(object sender, WeaponModChipEventArgs e)
    {

        weaponDescription.text = (e.description != null) ? e.description : "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.FindGameObjectWithTag("ShipModificationUI").SetActive(false);
        }

    }

    private void GenerateModifications(List<String> mods)
    {
        Dictionary<string, GameObject> modTable = new Dictionary<string, GameObject>();

        foreach (String mod in mods)
        {
            GameObject weaponChip;
            if (!modTable.TryGetValue(mod, out weaponChip))
            {
                //If the key isn't here, pull the resource and set it.
                weaponChip = Resources.Load<GameObject>("Weapons/Chips/" + mod);
                if (weaponChip == null)
                {
                    continue;
                }
                modTable.Add(mod, weaponChip);

            }

            GameObject.Instantiate(weaponChip).transform.SetParent(weaponModificationContent.transform, false);

        }
    }

    public void ManageCost(int cost, bool addOrRemove = true)
    {
        if (addOrRemove)
        {
            totalCostValue += cost;
        }
        else
        {
            totalCostValue -= cost;
        }
        displayTotalCost(totalCostValue);
    }

    //Used by confirmation button in UI
    public void confrimModifcations()
    {
        if (PlayerVault.instance.money - totalCostValue < 0)
        {
            Debug.Log("Somehow, not enough credits?!");
            return;
        }
        PlayerVault.instance.SpendMoney(totalCostValue);
        totalCostValue = 0;
        displayCredits();
        displayTotalCost(0);
        confirmWeponModification?.Invoke(this, null);
    }

    public void Exit()
    {
        CanvasManager.instance.CloseGUI("ShipModification");
    }

    private void displayTotalCost(int cost)
    {
        totalCost.text = "Total Cost: " + cost;
    }

    private void displayCredits()
    {
        credits.text = "Credits: " + PlayerVault.instance.money;
    }
}
