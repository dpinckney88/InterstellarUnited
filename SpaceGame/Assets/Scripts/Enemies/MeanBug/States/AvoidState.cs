using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AvoidState : State
{
    GameObject gmObject;
    Coroutine co;
    Vector3 directionOfStation;

    // Start is called before the first frame update
    protected override void OnEnter()
    {
        base.OnEnter();
        controller.visibleState = "Avoid";
        gmObject = controller.gameObject;
        directionOfStation = controller.stationAvoid.gameObject.transform.position - gmObject.transform.position;

    }

    protected override void OnUpdate()
    {

        base.OnUpdate();
        gmObject.transform.up = Vector3.RotateTowards(gmObject.transform.up, directionOfStation * -1, 10 * Time.deltaTime, 0);
        gmObject.transform.position = Vector3.MoveTowards(gmObject.transform.position, gmObject.transform.position + new Vector3(1, 1, 0), 2 * Time.deltaTime);
        if (Vector3.Distance(controller.stationAvoid.gameObject.transform.position, gmObject.transform.position) > controller.distanceToAvoid + 5)
        {
            controller.ChangeState(controller.idleState);
        }


    }
}
