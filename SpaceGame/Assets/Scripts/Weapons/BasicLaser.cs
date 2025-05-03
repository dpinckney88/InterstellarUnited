using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLaser : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject projectile;
    private GameObject ship;

    [field: SerializeField] public string weaponDescription { get; set; } = "STANDARD BLASTER: Single fire per click, basic laser blast. Deals a modest amount of damage but can fire quickly.";

    public void engage()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && ship.GetComponent<IShipCore>().currentState != IShipCore.ShipState.inSafeZone)
        {
            //fire primary gun
            GameObject projectileSpawn = GameObject.Instantiate(projectile, transform.position, transform.rotation);
            projectileSpawn.GetComponent<Projectile_base>().IgnoreCollision(new List<int>() { 8 });
        }
    }
}
