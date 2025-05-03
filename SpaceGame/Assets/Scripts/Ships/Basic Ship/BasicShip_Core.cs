using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArgs : EventArgs
{
    public float damage { get; set; }
    public float armor { get; set; }
}

public class HealthArgs : EventArgs
{
    public int health { get; set; }
}
public class FuelConsumptionArgs : EventArgs
{
    public float consumedFuel { get; set; }
}

public class RefuelUIArgs : EventArgs
{
    public float fuelCells { get; set; }
}
public class BasicShip_Core : MonoBehaviour, IShipCore, IDamageable, ISpaceStationComms
{
    public string shipName { get; private set; }

    public float startingFuel { get; private set; }

    public float maxUpgradedFuel { get; private set; }

    public float maxWeight { get; private set; }

    public int currentCargoSize { get; private set; }

    public int maxCargoSize { get; private set; }

    public float speed { get; private set; }

    public int healthSteps { get; private set; }

    public float rotationSpeed { get; private set; }

    public float fuelConsumptionRate { get; private set; }

    public float maxFuel { get; private set; }

    public float currentFuel { get; private set; }
    public IShipCore.ShipState currentState { get; private set; }

    public float currentHealth { get; private set; }

    private float armor = .5f;
    private float maxHealth;

    public static event EventHandler<DamageArgs> UpdateDamageUI;
    public static event EventHandler<FuelConsumptionArgs> UpdateFuelUI;
    public static event EventHandler<RefuelArgs> RefuelUI;
    public static event EventHandler<HealthArgs> HealthUI;

    private PlayerVault playerVault;
    public bool atSpaceStation = false;

    void Awake()
    {
        shipName = "Basic Ship";
        startingFuel = 100f;
        maxUpgradedFuel = 500f;
        maxWeight = 1000;
        currentCargoSize = 0;
        maxCargoSize = 3;
        speed = 3.5f;
        rotationSpeed = 360f;
        fuelConsumptionRate = 2f;
        maxFuel = 14 * Utilities.fuelConstant;
        healthSteps = 18;
        currentFuel = maxFuel;
        maxHealth = ((int)Math.Floor(1 / armor)) * healthSteps;
        playerVault = PlayerVault.instance;
        currentState = IShipCore.ShipState.none;
    }
    // Start is called before the first frame update
    void Start()
    {
        RefuelStation.refuel += Refuel;
        currentHealth = maxHealth;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(float damage)
    {
        //Dispatch message to UI to remove .5 bars per damage point
        UpdateDamageUI?.Invoke(this, new DamageArgs() { damage = damage, armor = armor });

        //Lower HP and track it.
        currentHealth -= damage * armor;

        if (currentHealth <= 0)
        {
            //Space Ship is out of HP
        }

        //Deal damage to all packages
    }
    //Use fuel
    public bool ConsumeFuel()
    {
        //Amount of fuel used on this current frame
        float usedFuel = fuelConsumptionRate * Time.deltaTime;

        currentFuel -= usedFuel;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

        //Update the Fuel UI to reflect how much fuel was used
        UpdateFuelUI?.Invoke(this, new FuelConsumptionArgs() { consumedFuel = usedFuel });

        //If we have fuel still, return true. If we are out of fuel, return false.
        return (currentFuel > 0);

    }

    //Main Refuel. Adjust Interface to handle this refule as opposed to the other
    public void Refuel(object sender, RefuelArgs e)
    {
        int maxPelletsNeeded = (int)Math.Ceiling((maxFuel - currentFuel) / Utilities.fuelConstant);
        currentFuel += e.numberOfFuelPellets * Utilities.fuelConstant;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        int numPelletsAdded = (e.numberOfFuelPellets > maxPelletsNeeded) ? maxPelletsNeeded : e.numberOfFuelPellets;
        RefuelUI?.Invoke(this, new RefuelArgs() { numberOfFuelPellets = numPelletsAdded });
    }


    public void Repair(int numHealthRepair)
    {

        int maxPelletsNeeded = (numHealthRepair + currentHealth > maxHealth) ? (int)Math.Ceiling(maxHealth - currentHealth) : numHealthRepair;
        HealthUI?.Invoke(this, new HealthArgs() { health = maxPelletsNeeded / (int)Math.Floor((1 / armor)) });
    }

    //REMOVE
    public void Refuel(int fuelAmount)
    {
        currentFuel += fuelAmount;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

    }

    public void InRangeOfStation(SpaceStation station)
    {
        if (!atSpaceStation)
        {
            StartCoroutine(StationCheckCO(station));
        }
    }

    IEnumerator StationCheckCO(SpaceStation station)
    {
        while (station.dockedShip != null)
        {
            atSpaceStation = true;
            currentState = IShipCore.ShipState.inSafeZone;
            yield return null;
        }
        atSpaceStation = false;
        currentState = IShipCore.ShipState.none;
        yield break;
    }
}
