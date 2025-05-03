using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public GameObject playerUnit;
    public float speed;
    public float rotationSpeed;
    private float fuelConsumptionRate, maxFuel, currentFuel;
    private IShipCore ship;
    private StoryEvents storyEvents;
    // Start is called before the first frame update
    void Awake()
    {
        storyEvents = FindObjectOfType<StoryEvents>();
    }
    void Start()
    {
        ship = gameObject.GetComponent<IShipCore>();
        speed = ship.speed;
        rotationSpeed = ship.rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (storyEvents.semiPausedState) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerUnit.transform.rotation = Quaternion.LookRotation(playerUnit.transform.forward, mousePos - playerUnit.transform.position);

        if (Input.GetKey(KeyCode.D))
        {
            if (!ship.ConsumeFuel()) { return; }
            ;
            playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, playerUnit.transform.position + Vector3.right, speed * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.A))
        {
            if (!ship.ConsumeFuel()) { return; }
            ;
            playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, playerUnit.transform.position + Vector3.left, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (!ship.ConsumeFuel()) { return; }
            ;
            playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, playerUnit.transform.position + Vector3.up, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (!ship.ConsumeFuel()) { return; }
            ;
            playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, playerUnit.transform.position + Vector3.down, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            playerUnit.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            playerUnit.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.R))
        {
            playerUnit.transform.rotation = Quaternion.identity;
        }
    }

}
