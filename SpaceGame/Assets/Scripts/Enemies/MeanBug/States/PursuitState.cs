using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState : State
{
    MeanBug mBug;
    // Start is called before the first frame update
    protected override void OnEnter()
    {
        controller.visibleState = "Pursuit";
        mBug = controller.gameObject.GetComponent<MeanBug>();
    }

    protected override void OnUpdate()
    {
        Vector3 targetPOS = controller.gameObject.GetComponent<MeanBug>().target.gameObject.transform.position;

        //Rotate towards target
        controller.gameObject.transform.up = Vector3.RotateTowards(controller.gameObject.transform.up, targetPOS - controller.gameObject.transform.position, 10 * Time.deltaTime, 0);
        //Move towards target
        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, targetPOS, 3 * Time.deltaTime);

        if (AttackRangeCheck())
        {
            //Change to attack state
            controller.ChangeState(controller.attackState);
            return;
        }

        if (!PursuitCheck())
        {
            controller.ChangeState(controller.idleState);
        }

        //Go to idle state if object ahs arrived at the destination.
        if (controller.gameObject.transform.position == targetPOS)
        {
            controller.ChangeState(controller.idleState);
        }
    }

    private bool AttackRangeCheck()
    {
        int layerMask = 1 << LayerMask.NameToLayer("PlayerUnit");
        Collider2D collider = Physics2D.OverlapCircle(controller.transform.position, mBug.baseStats.attackRange, layerMask);
        if (collider != null)
        {
            return true;
        }
        return false;
    }

    private bool PursuitCheck()
    {
        int layerMask = 1 << LayerMask.NameToLayer("PlayerUnit");
        int pursuitRange = controller.gameObject.GetComponent<MeanBug>().baseStats.pursuitRange;
        Collider2D collider = Physics2D.OverlapCircle(controller.gameObject.transform.position, pursuitRange, layerMask);
        //The player is in range
        if (collider != null)
        {
            return true;
        }
        return false;
    }
}
