using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxComponents : MonoBehaviour
{
    [SerializeField] private Transform primaryGunPos;
    [SerializeField] private GameObject projectile;
    private StoryEvents storyEvents;
    // Start is called before the first frame update
    void Awake()
    {
        storyEvents = FindObjectOfType<StoryEvents>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (storyEvents.semiPausedState) return;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && gameObject.GetComponent<IShipCore>().currentState != IShipCore.ShipState.inSafeZone)
        {
            //fire primary gun
            GameObject projectileSpawn = GameObject.Instantiate(projectile, primaryGunPos.position, transform.rotation);
            projectileSpawn.GetComponent<Projectile_base>().IgnoreCollision(new List<int>() { 8 });
        }
    }
}
