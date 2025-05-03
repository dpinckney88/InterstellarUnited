using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelBar : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    public GameObject fuelBar;
    public GameObject fuelPellets;
    private List<GameObject> fuelPelletsList = new List<GameObject>();
    private Vector2 originalSize;
    // Start is called before the first frame update
    void Start()
    {
        BasicShip_Core.UpdateFuelUI += UpdateFuel;
        BasicShip_Core.RefuelUI += Refuel;
        //SpaceStation.refuel += Refuel;
        //RefuelStation.refuel += Refuel;
        SetupFuelBar();

        //Track the original size
        originalSize = fuelPelletsList[0].GetComponent<RectTransform>().sizeDelta;
        ship = GameObject.FindGameObjectWithTag("Player");
    }

    private void Refuel(object sender, RefuelArgs e)
    {
        GameObject meter = fuelBar.transform.Find("Meter").gameObject;
        Destroy(fuelPelletsList[0].gameObject);
        fuelPelletsList.RemoveAt(0);
        fuelPelletsList.Reverse();

        for (int x = 0; x < e.numberOfFuelPellets; x++)
        {
            GameObject temp = GameObject.Instantiate(fuelPellets);
            temp.transform.SetParent(meter.transform, false);
            fuelPelletsList.Add(temp);
        }
        fuelPelletsList.Reverse();
    }

    //Update teh fuel bar the fuel is being used
    private void UpdateFuel(object sender, FuelConsumptionArgs e)
    {
        if (fuelPelletsList.Count <= 0) return;

        RectTransform tempPellet = fuelPelletsList[0].GetComponent<RectTransform>();
        Vector2 size = tempPellet.sizeDelta;
        //Visually remove part of the pellet based on the armor of the ship
        size = new Vector2(originalSize.x, size.y - (originalSize.y * e.consumedFuel / Utilities.fuelConstant));
        tempPellet.sizeDelta = size;

        //If the pellet is used up, go to the next one in the list
        if (size.y <= 0)
        {
            //remove all used up pellets
            Destroy(fuelPelletsList[0].gameObject);
            fuelPelletsList.RemoveAt(0);
        }
    }

    private void SetupFuelBar()
    {
        GameObject meter = fuelBar.transform.Find("Meter").gameObject;
        for (int x = 0; x < ship.GetComponent<IShipCore>().maxFuel / Utilities.fuelConstant; x++)
        {
            GameObject temp = GameObject.Instantiate(fuelPellets);
            temp.transform.SetParent(meter.transform, false);
            fuelPelletsList.Add(temp);
        }
        fuelPelletsList.Reverse();
    }
}
