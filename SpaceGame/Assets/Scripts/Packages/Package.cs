using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackingArgs : EventArgs
{
    public Transform transform;
}

public class Package : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject icon;
    [SerializeField] private PackagePopup popup;
    public static event EventHandler<TrackingArgs> trackPackage;
    public DeliverySO so;
    private PackagePopup popupRef;
    private GameObject hpBar;
    public int currentHP { get; private set; }
    public int totalHP { get; private set; }
    public GameObject fraigleIdicator;
    public TMP_Text destinationText;
    private Vector2 hpBarWidth;
    private RectTransform hpBarForegound;

    void Awake()
    {
        hpBar = gameObject.transform.Find("HPBar").gameObject;
        hpBarWidth = hpBar.transform.Find("Bar").gameObject.transform.Find("Foreground").GetComponent<RectTransform>().sizeDelta;
        hpBarForegound = hpBar.transform.Find("Bar").gameObject.transform.Find("Foreground").GetComponent<RectTransform>();
        BasicShip_Core.UpdateDamageUI += Damage;
    }
    // Start is called before the first frame update
    void Start()
    {

        PackageList.packageListState += PackageListStateHandler;


    }

    private void PackageListStateHandler(object sender, PackageStateArgs e)
    {
        if (!e.isOpen && popupRef != null)
        {
            RemovePopup();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TrackPackage()
    {
        GameObject stationToTrack = null;
        GameObject[] temp = GameObject.FindGameObjectsWithTag("SpaceStation");
        foreach (GameObject spaceStation in temp)
        {
            if (spaceStation.GetComponent<SpaceStation>().stationNumber == so.stationToDeliverTo)
            {
                stationToTrack = spaceStation;
                break;
            }

        }

        Transform destination = (stationToTrack != null) ? stationToTrack.transform : null;
        trackPackage?.Invoke(this, new TrackingArgs { transform = destination });
    }

    private void Damage(object sender, DamageArgs e)
    {
        if (currentHP <= 0) return;

        float damageMultiplier = so.isFragile ? .2f : .1f;

        int totalDamage = (int)Math.Floor(e.damage * damageMultiplier);
        totalDamage = (totalDamage < 1) ? 1 : totalDamage;

        currentHP -= totalDamage;
        currentHP = Mathf.Clamp(currentHP, 0, totalHP);

        float hpPrecentage = (float)currentHP / (float)totalHP;

        hpPrecentage = Mathf.Clamp(hpPrecentage, 0, 1);
        hpBarForegound.sizeDelta = new Vector2(hpBarWidth.x * hpPrecentage, hpBarWidth.y);

        if (currentHP <= 0)
        {
            hpBar.transform.Find("Destroyed").gameObject.SetActive(true);
        }
    }

    public void Setup(DeliverySO delivery)
    {
        so = delivery;
        totalHP = so.hp;
        currentHP = totalHP;
        icon.GetComponent<Image>().sprite = so.icon;
        if (delivery.isFragile)
        {
            fraigleIdicator.SetActive(true);
        }
        destinationText.text = delivery.stationToDeliverTo.ToString();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        popupRef = GameObject.Instantiate(popup, transform.position, Quaternion.identity);
        popupRef?.Setup(so.deliveryName, so.deliveryDescription, so.stationToDeliverTo.ToString());
        GameObject menu = GameObject.FindWithTag("PackageList");
        popupRef.transform.SetParent(menu.transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemovePopup();
    }

    private void RemovePopup()
    {
        Destroy(popupRef.gameObject);
    }

    void OnDestroy()
    {

        PackageList.packageListState -= PackageListStateHandler;
    }
}