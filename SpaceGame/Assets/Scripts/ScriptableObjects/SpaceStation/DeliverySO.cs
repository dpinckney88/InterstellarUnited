using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DeliverySO", menuName = "Delivery SO", order = 2)]
public class DeliverySO : ScriptableObject
{
    public string deliveryName;
    public string deliveryDescription;
    public int stationNum;
    public int stationToDeliverTo;
    public float weight;
    public int size;
    public Sprite icon;
    public bool isFragile;
    public int hp;
    public int value;
    public int id;
}
