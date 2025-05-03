using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveredPackages : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private DeliveredPackage deliveredPackagePrefab;
    [SerializeField] private TMP_Text totalValueEarned;
    private List<DeliveredPackage> packageRef = new List<DeliveredPackage>();
    // Start is called before the first frame update
    void Start()
    {
        SpaceStation.packageDevlivered += PackageDelivered;
        gameObject.SetActive(false);
    }

    private void PackageDelivered(object sender, PackageDevliveredArgs e)
    {
        List<DeliverySO> temp = GlobalPackageRegistry.instance.globalPackageList.FindAll(p => e.packageIds.Contains(p.id));
        foreach (DeliverySO delivered in temp)
        {
            DeliveredPackage pack = GameObject.Instantiate(deliveredPackagePrefab);
            pack.image.sprite = delivered.icon;
            pack.packageName.text = delivered.deliveryName;
            pack.transform.SetParent(content.transform, false);
            packageRef.Add(pack);
        }
        totalValueEarned.text = e.amountEarned.ToString();
    }

    public void Close()
    {
        foreach (DeliveredPackage temp in packageRef)
        {
            Destroy(temp.gameObject);
        }
        packageRef.Clear();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
