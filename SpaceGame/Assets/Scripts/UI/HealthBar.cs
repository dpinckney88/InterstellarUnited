using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject ship;
    public GameObject healthBar;
    public GameObject healthPellets;
    private List<GameObject> healthPelletsList = new List<GameObject>();
    private Vector2 originalSize;
    // Start is called before the first frame update
    void Start()
    {
        BasicShip_Core.UpdateDamageUI += UpdateWithDamage;
        BasicShip_Core.HealthUI += Repair;
        SetupHealthBar();

        //Track the original size
        originalSize = healthPelletsList[0].GetComponent<RectTransform>().sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void SetupHealthBar()
    {
        GameObject meter = healthBar.transform.Find("Meter").gameObject;
        for (int x = 0; x < ship.GetComponent<IShipCore>().healthSteps; x++)
        {
            GameObject temp = GameObject.Instantiate(healthPellets);
            temp.transform.SetParent(meter.transform, false);
            healthPelletsList.Add(temp);
        }
        healthPelletsList.Reverse();
    }

    private void Repair(object sender, HealthArgs e)
    {
        GameObject meter = healthBar.transform.Find("Meter").gameObject;
        Destroy(healthPelletsList[0].gameObject);
        healthPelletsList.RemoveAt(0);
        healthPelletsList.Reverse();

        for (int x = 0; x < e.health; x++)
        {
            GameObject temp = GameObject.Instantiate(healthPellets);
            temp.transform.SetParent(meter.transform, false);
            healthPelletsList.Add(temp);
        }
        healthPelletsList.Reverse();
    }

    private void UpdateWithDamage(object sender, DamageArgs e)
    {
        if (healthPelletsList.Count <= 0) return;
        //How many parts of times we are going to remove some fraction of the bar
        int numberOfSteps = (int)Math.Ceiling(e.damage * e.armor);

        int currentPellet = 0;
        int numberToRemove = 0;

        for (int x = 0; x < numberOfSteps; x++)
        {
            RectTransform tempPellet = healthPelletsList[0].GetComponent<RectTransform>();
            Vector2 size = tempPellet.sizeDelta;
            //Visually remove part of the pellet based on the armor of the ship
            size = new Vector2(originalSize.x, size.y - (originalSize.y * e.armor));
            tempPellet.sizeDelta = size;

            //If the pellet is used up, go to the next one in the list
            if (Math.Floor(size.y) <= 0)
            {
                currentPellet++;
                numberToRemove++;
                Destroy(healthPelletsList[0].gameObject);
                healthPelletsList.RemoveAt(0);
                if (healthPelletsList.Count == 0)
                {
                    return;
                }
            }
        }
        //remove all used up pellets
        //healthPelletsList = healthPelletsList.GetRange(numberToRemove, healthPelletsList.Count - numberToRemove);
    }
}
