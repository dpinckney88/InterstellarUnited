using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utilities
{
    public static readonly int fuelConstant = 16;
    public static readonly int fuelCostPerUnitConstant = 35;
    //Get the min and point of the screen in world view
    public static Vector3[] GetScreenBounds()
    {
        Camera mainCamera = Camera.main;
        Vector3 maxPoint = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3 minPoint = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        return new[] { minPoint, maxPoint };
    }

    public static GameObject GetSpaceStation(int num)
    {
        Span<GameObject> spaceStations = GameObject.FindGameObjectsWithTag("SpaceStation");
        foreach (GameObject station in spaceStations)
        {
            if (station.GetComponent<SpaceStation>().stationNumber == num)
            {
                return station;
            }
        }

        return null;
    }


}
