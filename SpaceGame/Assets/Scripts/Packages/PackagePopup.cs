using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackagePopup : MonoBehaviour
{
    [SerializeField] private GameObject packageName;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject destination;

    //Fill out the data for the package
    public void Setup(String name, String desc, String dest)
    {
        packageName.GetComponent<TMP_Text>().text = name ?? "Package";
        description.GetComponent<TMP_Text>().text = desc ?? "";
        destination.GetComponent<TMP_Text>().text = "Deliver To - Station " + dest ?? "Deliver To - Station X";
    }
}
