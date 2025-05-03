using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RefuelArgs : EventArgs
{
    public int numberOfFuelPellets { get; set; }
}

public class RefuelStation : MonoBehaviour
{
    public TMP_Text currentFuel, maxFuel, cost, availableCredits, costPerCell;
    private float currentFuelValue;
    private float startingFuel;
    private float maxFuelToPurchase;
    private int numSetToPurchase = 0;

    public static RefuelStation instance;

    private IShipCore ship;
    public static event EventHandler<RefuelArgs> refuel;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ship == null)
        {
            ship = GameObject.FindWithTag("Player").GetComponent<IShipCore>();
        }

        startingFuel = (float)Math.Round(ship.currentFuel / Utilities.fuelConstant, 1);
        currentFuelValue = startingFuel;
        maxFuelToPurchase = Mathf.Ceil((ship.maxFuel / Utilities.fuelConstant) - startingFuel);
        currentFuel.text = startingFuel.ToString();
        maxFuel.text = " / " + (ship.maxFuel / Utilities.fuelConstant).ToString();
        cost.text = "Cost: $0";
        availableCredits.text = "Available Credits: $" + PlayerVault.instance.money;
        costPerCell.text = "Cost Per Cell: " + Utilities.fuelCostPerUnitConstant.ToString();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddFuelBypass(int numFuelCells)
    {
        refuel?.Invoke(this, new RefuelArgs() { numberOfFuelPellets = numFuelCells });
    }

    //Increase amount of fuel being purcahsed in "pellets".
    public void addFuel()
    {
        if (numSetToPurchase + 1 > maxFuelToPurchase)
        {
            return;
        }
        //If the player can afford another fuel pellet, let them add fuel;
        if ((numSetToPurchase + 1) * Utilities.fuelCostPerUnitConstant <= PlayerVault.instance.money)
        {


            //If this the first cell being purchased, round up to the nearest whole number, otherwise, increase by 1.
            if (numSetToPurchase == 0)
            {
                currentFuelValue = Mathf.Ceil(currentFuelValue);
            }
            else
            {
                currentFuelValue++;
            }
            numSetToPurchase++;
            currentFuel.text = currentFuelValue.ToString();
        }
        else
        {
            Debug.Log("Cannot afford more");
        }
        cost.text = "Cost: $" + numSetToPurchase * Utilities.fuelCostPerUnitConstant;
    }

    //Decrease the number of fuel cells you are purchasing by one
    public void removeFuel()
    {
        //If we weren't set to purchase any, exit the function
        if (numSetToPurchase == 0)
        {
            return;
        }

        numSetToPurchase--;

        //Do not go below the amount of fuel you started with
        if (currentFuelValue - 1 < startingFuel)
        {
            currentFuelValue = startingFuel;
        }
        else
        {
            currentFuelValue--;
        }
        currentFuel.text = currentFuelValue.ToString();
        cost.text = "Cost: $" + numSetToPurchase * Utilities.fuelCostPerUnitConstant;
    }

    //Purchase fuel cells
    public void PurchaseFuelCells()
    {
        refuel?.Invoke(this, new RefuelArgs() { numberOfFuelPellets = numSetToPurchase });
        PlayerVault.instance.SpendMoney(numSetToPurchase * Utilities.fuelCostPerUnitConstant);
        gameObject.SetActive(false);
    }
}
