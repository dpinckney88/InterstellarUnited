using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShipCore
{

    public string shipName { get; }
    public float startingFuel { get; }
    public float maxUpgradedFuel { get; }
    public float maxWeight { get; }
    public int currentCargoSize { get; }
    public int maxCargoSize { get; }
    public float speed { get; }
    public float rotationSpeed { get; }
    public float fuelConsumptionRate { get; }
    public float maxFuel { get; }
    public float currentFuel { get; }
    public int healthSteps { get; }
    public float currentHealth { get; }
    public enum ShipState { inSafeZone, none }
    public ShipState currentState { get; }
    public bool ConsumeFuel();
    public void Refuel(int fuelAmount);



}
