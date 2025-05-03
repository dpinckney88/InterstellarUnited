using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PackageDevliveredArgs : EventArgs
{
    public int stationNum { get; set; }
    public List<int> packageIds;
    public int amountEarned;
}


public class SpaceStation : MonoBehaviour
{
    public static event EventHandler<PackageDevliveredArgs> packageDevlivered;
    public static event EventHandler<RefuelArgs> refuel;
    [SerializeField] private List<DeliverySO> DeliveryList = new List<DeliverySO>();
    [SerializeField] private GameObject deliveryButton;
    [SerializeField] private GameObject NPCHubStack;
    [SerializeField] private GameObject NPCHub;
    [SerializeField] private GameObject deliverPackagesButton;
    [SerializeField] private GameObject spaceStationMenu;
    [SerializeField] private GameObject spaceStationMenuContentArea;
    [SerializeField] private Canvas dockUI;
    [SerializeField] private GameObject stationMenuSlot;
    public GameObject noDeliveries;
    [SerializeField] private TMP_Text stationName;
    [SerializeField] private SpaceStationSO so;
    public int stationNumber;
    private bool isMenuOpen = false;
    private bool shouldCheckForDistance = true;
    private int amountToRefuel;
    public IShipCore dockedShip { get; private set; }
    private GameObject globalPackageRegistry;
    private List<GameObject> NPCs = new List<GameObject>();

    void Awake()
    {
        globalPackageRegistry = GameObject.FindWithTag("GlobalPackageRegistry");
    }
    void Start()
    {
        StoryEvents.removeObs += RemoveObs;
        StoryEvents.applyObs += ReapplyObs;
        StoryEvents.addNPCToStation += AddNPCToStation;
        Delivery.packageAccepted += PackageAccepted;
        SetupDeliveryList();
    }

    private void PackageAccepted(object sender, PackageAcceptedArgs e)
    {
        if (DeliveryList.Contains(e.package))
        {
            DeliveryList.Remove(e.package);
        }
        StationCheck();
    }


    // Update is called once per frame
    void Update()
    {
        UnitsInRange();
    }

    private void RemoveObs(object sender, EventArgs e)
    {
        //Close all menus
        CloseStationMenu();
        //Remove the collider
        GetComponent<BoxCollider2D>().enabled = false;
        //Stop checking for ship distance
        shouldCheckForDistance = false;
    }

    private void ReapplyObs(object sender, EventArgs e)
    {
        //Remove the collider
        GetComponent<BoxCollider2D>().enabled = true;
        //Stop checking for ship distance
        shouldCheckForDistance = true;
    }


    private void SetupDeliveryList()
    {
        //Create button for delivery
        foreach (DeliverySO delivery in DeliveryList)
        {
            GameObject tempButton = GameObject.Instantiate(deliveryButton);
            tempButton.SetActive(false);
            tempButton.GetComponent<Delivery>().CreateDelivery(delivery);
            tempButton.transform.SetParent(spaceStationMenuContentArea.transform, false);
            tempButton.SetActive(true);
        }
    }

    public void AddPackage(DeliverySO delivery)
    {
        if (delivery.stationNum != stationNumber)
        {
            //throw an error
            return;
        }

        GameObject tempButton = GameObject.Instantiate(deliveryButton);
        tempButton.SetActive(false);
        tempButton.GetComponent<Delivery>().CreateDelivery(delivery);
        tempButton.transform.SetParent(spaceStationMenuContentArea.transform, false);
        tempButton.SetActive(true);
        DeliveryList.Add(delivery);
    }
    public void AddPackages(int numberOfPackages)
    {
        Span<DeliverySO> temp = globalPackageRegistry.GetComponent<GlobalPackageRegistry>().RequestPacakgesForStation(stationNumber, numberOfPackages).ToArray();
        foreach (DeliverySO delivery in temp)
        {
            GameObject tempButton = GameObject.Instantiate(deliveryButton);
            tempButton.SetActive(false);
            tempButton.GetComponent<Delivery>().CreateDelivery(delivery);
            tempButton.transform.SetParent(spaceStationMenuContentArea.transform, false);
            tempButton.SetActive(true);
            DeliveryList.Add(delivery);
        }
        StationCheck();
    }

    public void OpenStationMenu()
    {
        spaceStationMenu.transform.SetParent(stationMenuSlot.transform);
        spaceStationMenu.SetActive(true);
        Camera.main.GetComponent<MainCamera>().setSubject(spaceStationMenu.transform);
        StationCheck();

        //Set Station Name Text
        stationName.text = so.stationName + " - " + so.stationNumber;
    }

    public void CloseStationMenu()
    {
        spaceStationMenu.SetActive(false);
        Camera.main.GetComponent<MainCamera>().setSubject(null);
    }

    public void DeliverPackages()
    {

        List<GameObject> deliveredPackages = PackageList.instance.DeliverPackages(stationNumber);
        List<int> deliveredPackageIds = new List<int>();

        //Calculate how much money is earned based on package condition
        int amountEarned = 0;
        foreach (GameObject package in deliveredPackages)
        {
            int value = package.gameObject.GetComponent<Package>().so.value;
            float condition = (float)package.gameObject.GetComponent<Package>().currentHP / package.gameObject.GetComponent<Package>().totalHP;
            int earnedAmount = 0;
            if (condition >= .9f)
            {
                earnedAmount = value;

            }
            else if (condition >= .8f && condition < .9f)
            {
                earnedAmount = (int)Mathf.Ceil(value * .9f);
                PlayerVault.instance.IncreaseScore(900);
            }
            else if (condition >= .7f && condition < .8f)
            {
                earnedAmount = (int)Mathf.Ceil(value * .8f);
                PlayerVault.instance.IncreaseScore(800);
            }
            else if (condition > 0 && condition < .7f)
            {
                earnedAmount = (int)Mathf.Ceil(value * condition);
                PlayerVault.instance.IncreaseScore(700);
            }
            else
            {
                //For clarity
                earnedAmount = 0;
                PlayerVault.instance.IncreaseScore(300);
            }
            amountEarned += earnedAmount;
            deliveredPackageIds.Add(package.gameObject.GetComponent<Package>().so.id);
        }
        PlayerVault.instance.AddMoney(amountEarned);
        //Dispatch a message of the report
        CanvasManager.instance.ShowGUI("Delivered");
        packageDevlivered?.Invoke(this, new PackageDevliveredArgs() { stationNum = stationNumber, packageIds = deliveredPackageIds, amountEarned = amountEarned });
    }

    public void Refuel(int overrideFuelAmount = -1, int overrideCost = -1)
    {
        IShipCore ship = GameObject.FindWithTag("Player").GetComponent<IShipCore>();

        amountToRefuel = (overrideFuelAmount > -1) ? overrideFuelAmount / Utilities.fuelConstant : amountToRefuel;

        amountToRefuel = (int)Mathf.Clamp(amountToRefuel, 0, (ship.maxFuel - ship.currentFuel) / Utilities.fuelConstant);

        if (amountToRefuel == 0)
        {
            return;
        }
        int cost = (overrideCost == -1) ? amountToRefuel * Utilities.fuelCostPerUnitConstant : overrideCost;

        //refuel?.Invoke(this, new RefuelArgs() { numberOfFuelPellets = amountToRefuel });

        if (overrideCost == -1) { }
        PlayerVault.instance.SpendMoney(cost);
        ship.Refuel(amountToRefuel * Utilities.fuelConstant);
    }

    private void AddNPCToStation(object sender, AddNPCToStationArgs e)
    {
        if (e.stationNum == stationNumber)
        {

            GameObject temp = GameObject.Instantiate(e.NPC);
            temp.transform.SetParent(NPCHubStack.transform, true);
            //Don't allow a duplicate NPC to be added.
            if (NPCs.Find(npc => npc.GetComponent<NPC>().so.id == temp.GetComponent<NPC>().so.id) == null)
            {
                NPCs.Add(temp);
            }
        }
    }

    public void AddNPCToStation(GameObject NPC)
    {

        GameObject temp = GameObject.Instantiate(NPC);
        temp.transform.SetParent(NPCHubStack.transform, true);
        //Don't allow a duplicate NPC to be added.
        if (NPCs.Find(npc => npc.GetComponent<NPC>().so.id == temp.GetComponent<NPC>().so.id) == null)
        {
            NPCs.Add(temp);
        }
    }

    public void TempRefuel()
    {
        CanvasManager.instance.refuelStation.SetActive(true);
    }

    //Check to see what units are in range of the space station and act accordingly
    private void UnitsInRange()
    {
        //Short circuit this function if we do not want to check what's in range for whatever reason
        if (!shouldCheckForDistance) return;

        Collider2D player = Physics2D.OverlapCircle(transform.position, 3, 1 << LayerMask.NameToLayer("PlayerUnit"));

        //Player unit within range of the space station
        if (player != null && dockUI.gameObject.activeSelf == false && !isMenuOpen)
        {
            dockedShip = player.gameObject.GetComponent<IShipCore>();
            dockUI.gameObject.SetActive(true);

        }
        else if (player == null && dockUI.gameObject.activeSelf)
        {
            dockedShip = null;
            dockUI.gameObject.SetActive(false);
            CloseStationMenu();
        }

        Collider2D[] allUnits = Physics2D.OverlapCircleAll(transform.position, 3);
        foreach (Collider2D collider in allUnits)
        {
            collider.GetComponent<ISpaceStationComms>()?.InRangeOfStation(this);
        }
    }

    private void StationCheck()
    {

        //Show or hide the No Deliveries text
        if (DeliveryList.Count > 0) { noDeliveries.SetActive(false); } else { noDeliveries.SetActive(true); }

        //Decide if NPCs should be shown.
        if (NPCs.Count > 0)
        {
            NPCHub.SetActive(true);
        }
        else
        {
            NPCHub.SetActive(false);
        }

        //Decide if Delivery Button is active or not
        if (PackageList.instance.AvailableDeliveriesForStation(stationNumber))
        {
            deliverPackagesButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            deliverPackagesButton.GetComponent<Button>().interactable = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, 3);
    }

    private void Oestroy()
    {
        Delivery.packageAccepted -= PackageAccepted;
    }
}
