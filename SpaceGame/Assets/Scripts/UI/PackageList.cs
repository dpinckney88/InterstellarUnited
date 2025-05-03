using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackageStateArgs : EventArgs
{
    public bool isOpen;
}

public class PackageList : MonoBehaviour
{
    [SerializeField] private GameObject packageListMenu;
    [SerializeField] private GameObject packagePrefab;
    [SerializeField] private GameObject content;
    [SerializeField] private List<GameObject> packageList;
    public TMP_Text numberOfPackagesText;
    public int maxCargoSize { get; private set; }
    public int currentCargoSize { get; private set; }
    public static PackageList instance;
    public static event EventHandler<PackageStateArgs> packageListState;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Delivery.packageAccepted += AddPackageToList;
        SpaceStation.packageDevlivered += RemovePackageList;
        StoryEvents.addPackageToShip += AddPackage;
        maxCargoSize = GameObject.FindWithTag("Player").GetComponent<IShipCore>().maxCargoSize;
        currentCargoSize = 0;
    }

    private void AddPackage(object sender, AddPackageToShipArgs e)
    {
        AcceptPackage(e.so);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePackageList();
        }
    }
    //Open or close the package list
    public void TogglePackageList()
    {
        packageListMenu.SetActive(!packageListMenu.activeSelf);
        packageListState?.Invoke(this, new PackageStateArgs() { isOpen = packageListMenu.activeSelf });
    }

    public bool AvailableDeliveriesForStation(int stationNum)
    {
        foreach (GameObject package in packageList)
        {
            if (package.GetComponent<Package>().so.stationToDeliverTo == stationNum)
            {
                return true;
            }
        }
        return false;
    }

    public List<GameObject> DeliverPackages(int stationNum)
    {
        List<GameObject> packagesToDeliver = packageList.FindAll(package => package.GetComponent<Package>().so.stationToDeliverTo == stationNum);
        RemovePackageList(packagesToDeliver);
        return packagesToDeliver;
    }

    //If we can accept the package, return true. Otherwise, return false;
    public bool AcceptPackage(DeliverySO delivery)
    {
        if (CanAcceptPackage(delivery))
        {
            currentCargoSize += delivery.size;
            AddPackageToList(delivery);
            return true;
        }
        return false;
    }

    public bool CanAcceptPackage(DeliverySO delivery)
    {
        if (currentCargoSize + delivery.size <= maxCargoSize)
        {
            return true;
        }
        return false;
    }

    //Create a package and place it in the package list for delviery
    private void AddPackageToList(DeliverySO delivery)
    {
        GameObject package = GameObject.Instantiate(packagePrefab);
        package.GetComponent<Package>().Setup(delivery);
        package.transform.SetParent(content.transform, false);
        packageList.Add(package);
        UpdateNumberOfPackagesText();
    }

    private void AddPackageToList(object sender, PackageAcceptedArgs e)
    {
        GameObject package = GameObject.Instantiate(packagePrefab);
        package.GetComponent<Package>().Setup(e.package);
        package.transform.SetParent(content.transform);
        packageList.Add(package);
        UpdateNumberOfPackagesText();
    }

    private void RemovePackageList(List<GameObject> packages)
    {
        foreach (GameObject package in packages)
        {
            if (package.GetComponent<Package>() != null)
            {
                currentCargoSize -= package.gameObject.GetComponent<Package>().so.size;
                Destroy(package.gameObject);
                packageList.Remove(package);
            }
        }
    }

    private void RemovePackageList(object sender, PackageDevliveredArgs e)
    {
        //Destory the package game object so that you no longer see it in the package list.
        foreach (GameObject p in packageList)
        {
            if (p.gameObject.GetComponent<Package>().so.stationNum == e.stationNum)
            {
                Destroy(p.gameObject);
            }
        }
        //Remove the refrence to the package that was stored
        packageList.RemoveAll(package => package.GetComponent<Package>().so.stationNum == e.stationNum);
        UpdateNumberOfPackagesText();
    }

    private void UpdateNumberOfPackagesText()
    {
        numberOfPackagesText.text = packageList.Count + "/" + maxCargoSize;
    }

    void OnDestory()
    {
        Delivery.packageAccepted -= AddPackageToList;
    }
}
