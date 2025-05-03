using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "StationSO", menuName = "Station SO", order = 3)]
public class SpaceStationSO : ScriptableObject
{
    public String stationName;
    public int stationNumber;
}
