using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected MeanBugStateController controller;
    // Start is called before the first frame update
    public void OnStateEnter(MeanBugStateController sc)
    {
        controller = sc;
        OnEnter();
    }

    protected virtual void OnEnter()
    {

    }

    public void OnStateExit()
    {
        OnExit();
    }
    protected virtual void OnExit()
    {

    }
    public void OnStateUpdate()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }
}
