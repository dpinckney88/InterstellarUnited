using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MiniMapDestinationPoint : MonoBehaviour
{
    public int radius = 100;
    public Transform destination;
    public static MiniMapDestinationPoint instance;
    [SerializeField] Camera miniMapCamera;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Package.trackPackage += trackPoint;
    }

    private void trackPoint(object sender, TrackingArgs e)
    {
        destination = e.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (destination != null)
        {
            UpdatePosition();
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(transform.position, radius);
    }

    public void SetTrackingPoint(Transform transform)
    {
        destination = transform;
    }

    private void UpdatePosition()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Player");
        GameObject[] temp = GameObject.FindGameObjectsWithTag("SpaceStation");

        Vector2 dir = destination.position - ship.transform.position;
        Vector2 forward = new Vector2(ship.transform.forward.x, ship.transform.forward.z);
        int xOffset = (dir.normalized.x) >= 0 ? 1 : -1;
        int yOffset = (dir.normalized.y) >= 0 ? 1 : -1;
        float angle = 0f;

        if (xOffset * yOffset == 1)
        {
            angle = Vector2.SignedAngle(dir, forward);
        }
        else
        {
            angle = (Vector2.SignedAngle(dir, forward) - 90) * (xOffset * yOffset);
        }

        float radians = angle * Mathf.Deg2Rad;
        float x = radius * Mathf.Cos(radians);
        float y = radius * Mathf.Sin(radians);
        Vector2 pos = new Vector2(x, y);
        GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
