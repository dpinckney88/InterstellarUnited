using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingState : State
{
    GameObject gmObject;
    Vector3 destination;
    int randomX, randomY;
    protected override void OnEnter()
    {
        base.OnEnter();
        controller.visibleState = "Wandering";
        System.Random r = new System.Random();

        gmObject = controller.gameObject;
        //Randomly choose a distance in X and Y direction
        randomX = r.Next(0, 3);
        randomY = r.Next(0, 3);

        //Randomly decide if the value is negative or positve;
        randomX *= r.Next(0, 2) * 2 - 1;
        randomY *= r.Next(0, 2) * 2 - 1;

        destination = new Vector3(randomX, randomY, 0) + gmObject.transform.position;
    }

    protected override void OnUpdate()
    {

        //If not pursing, keep moving to destination
        gmObject.transform.up = Vector3.RotateTowards(gmObject.transform.up, destination - controller.gameObject.transform.position, 10 * Time.deltaTime, 0);
        gmObject.transform.position = Vector3.MoveTowards(gmObject.transform.position, destination, 2 * Time.deltaTime);

        if (gmObject.transform.position == destination)
        {
            controller.ChangeState(controller.wanderingState);
        }

        //Check if should be pursuing
        int layerMask = 1 << LayerMask.NameToLayer("PlayerUnit");
        int pursuitRange = controller.gameObject.GetComponent<MeanBug>().baseStats.pursuitRange;
        Collider2D collider = Physics2D.OverlapCircle(controller.gameObject.transform.position, pursuitRange, layerMask);
        //The player is in range
        if (collider != null)
        {
            controller.gameObject.GetComponent<MeanBug>().target = collider;
            //Switch to pursuit state
            controller.ChangeState(controller.pursuitState);
        }

    }
}
