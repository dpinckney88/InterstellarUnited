using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeanBugStateController : MonoBehaviour
{

    public WanderingState wanderingState = new WanderingState();
    public IdleState idleState = new IdleState();
    public PursuitState pursuitState = new PursuitState();
    public AttackState attackState = new AttackState();
    public AvoidState avoidState = new AvoidState();
    public State currentState;
    public string visibleState;
    public float distanceToAvoid;

    public SpaceStation stationAvoid;
    private StoryEvents storyEvents;
    void Awake()
    {
        storyEvents = FindObjectOfType<StoryEvents>();
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = idleState;
        currentState.OnStateEnter(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!storyEvents.semiPausedState)
        {
            currentState.OnStateUpdate();
        }
    }

    public void ChangeState(State state)
    {
        currentState.OnStateExit();
        currentState = state;
        state.OnStateEnter(this);
    }

    public void AvoidStation(SpaceStation station)
    {
        if (currentState != avoidState)
        {
            distanceToAvoid = Vector3.Distance(station.transform.position, transform.position);
            stationAvoid = station;
            ChangeState(avoidState);
        }
    }
}
