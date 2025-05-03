using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    // Start is called before the first frame update
    protected override void OnEnter()
    {
        base.OnEnter();
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
        else
        {
            controller.ChangeState(controller.wanderingState);
        }
    }

}
