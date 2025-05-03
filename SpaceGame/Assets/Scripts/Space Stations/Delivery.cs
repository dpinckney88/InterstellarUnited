using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackageAcceptedArgs : EventArgs
{

    public DeliverySO package { get; set; }
}

public class Delivery : MonoBehaviour
{
    public GameObject description;
    public GameObject stationNum;
    public GameObject packageName;
    public GameObject frame;
    public GameObject icon;
    public TMP_Text credits;
    public TMP_Text durability;
    [SerializeField] private GameObject fragileIndicator;
    public float weight;
    public int size;
    private DeliverySO deliverySO;
    public static event EventHandler<PackageAcceptedArgs> packageAccepted;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Set up the delivery object
    public void CreateDelivery(DeliverySO so, GameObject frame = null, GameObject icon = null)
    {
        stationNum.GetComponent<TMP_Text>().text = so.stationToDeliverTo.ToString();
        packageName.GetComponent<TMP_Text>().text = so.deliveryName;
        credits.text = "Credits: " + so.value.ToString();
        durability.text = "Durability: " + so.hp.ToString();
        fragileIndicator.SetActive(so.isFragile);
        weight = so.weight;
        size = so.size;
        deliverySO = so;
    }

    public void AcceptDelivery()
    {
        if (PackageList.instance.CanAcceptPackage(deliverySO))
        {
            packageAccepted?.Invoke(this, new PackageAcceptedArgs() { package = deliverySO });
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Can't Deliver");
        }
    }
}
