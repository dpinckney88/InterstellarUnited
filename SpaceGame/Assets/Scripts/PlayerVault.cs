using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVault : MonoBehaviour
{
    public static PlayerVault instance;
    public int money { get; private set; }
    public int score { get; private set; } = 0;
    [SerializeField] private List<GameObject> packageList;
    public List<DeliverySO> deliveries { get; private set; }

    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        money = 1001;
        deliveries = new List<DeliverySO>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("You have: $" + money);
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        Debug.Log("You have: $" + money);
    }

    public void IncreaseScore(int s)
    {
        score += s;
        CanvasManager.instance.UpdateScore();
    }



    ///////////////////////////Handle Packages/////////////////////////
    private void AddPackageToList(object sender, PackageAcceptedArgs e)
    {

    }

    public void DeliverPackages(int stationNum)
    {
        deliveries.RemoveAll(delivery => delivery.stationNum == stationNum);
    }

    public void AddDeliveryToList(DeliverySO delivery)
    {
        deliveries.Add(delivery);
    }

    public bool AvailableDeliveriesForStation(int stationNum)
    {
        foreach (DeliverySO delivery in deliveries)
        {
            if (delivery.stationNum == stationNum)
            {
                return true;
            }
        }
        return false;
    }
}
