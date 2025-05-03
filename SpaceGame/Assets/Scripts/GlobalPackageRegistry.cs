using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalPackageRegistry : MonoBehaviour
{
    public List<DeliverySO> globalPackageList;
    public List<DeliverySO> distributedPackages;
    public static GlobalPackageRegistry instance;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<DeliverySO> RequestPacakgesForStation(int station, int numberOfPackages = 50)
    {
        List<DeliverySO> tempPackages = globalPackageList.FindAll(package => package.stationNum == station);

        if (tempPackages.Count > numberOfPackages)
        {
            tempPackages.RemoveRange(numberOfPackages, tempPackages.Count - numberOfPackages);
        }

        distributedPackages.AddRange(tempPackages);
        globalPackageList.RemoveAll(package => tempPackages.Contains(package));

        return tempPackages;

    }

    public List<DeliverySO> GetPackageByID(int[] id)
    {
        return globalPackageList.FindAll(package => id.Contains(package.id));
    }

    private bool PackageMatch(DeliverySO so)
    {
        Debug.Log(so);
        return false;
    }
}
