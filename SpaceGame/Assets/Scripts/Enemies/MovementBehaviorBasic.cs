using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class MovementBehaviorBasic : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyBaseStatsSO stats;
    private IEnemyBasic behavior;
    private bool avoidingStation = false;
    private Coroutine co;
    private bool movingFromCollision = false;
    private Collision2D currentCollision;
    private bool isMoving = false;
    void Start()
    {
        behavior = GetComponent<IEnemyBasic>();
        if (behavior == null)
        {
            //Throw error
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (behavior.currentState == IEnemyBasic.state.attacking)
        {
            StopCoroutine(co);
            StopAllCoroutines();
            isMoving = false;
        }

        if (!avoidingStation && behavior.currentState != IEnemyBasic.state.attacking)
        {
            //Check if we should be chasing player
            pursuitCheck();

            if (behavior.currentState == IEnemyBasic.state.pursuit && !isMoving)
            {
                InPursuit();
            }
            if (behavior.currentState == IEnemyBasic.state.idle)
            {
                wandering();
            }
        }
    }

    IEnumerator InPursuitCO()
    {
        Vector3 targetPOS = behavior.target.gameObject.transform.position;
        while (transform.position != targetPOS)
        {
            isMoving = true;
            //Rotate towards target
            transform.up = Vector3.RotateTowards(transform.up, targetPOS - transform.position, 10 * Time.deltaTime, 0);
            //Move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetPOS, 3 * Time.deltaTime);
            yield return null;
        }
        isMoving = false;

    }

    private void InPursuit()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }
        co = StartCoroutine(InPursuitCO());
    }

    private void pursuitCheck()
    {
        int layerMask = 1 << LayerMask.NameToLayer("PlayerUnit");
        int pursuitRange = (behavior.currentState == IEnemyBasic.state.pursuit) ? stats.attackingPursuitRange : stats.pursuitRange;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, pursuitRange, layerMask);
        if (collider != null && behavior.currentState != IEnemyBasic.state.attacking)
        {
            behavior.target = collider;
            behavior.currentState = IEnemyBasic.state.pursuit;
        }
        //If the target is outside of the attack range;
        else if (collider == null && behavior.currentState == IEnemyBasic.state.pursuit)
        {
            behavior.currentState = IEnemyBasic.state.idle;
            behavior.target = null;
        }
    }

    public void AvoidStation(Vector3 distance)
    {
        if (movingFromCollision) return;

        if (!avoidingStation)
        {
            if (co != null)
            {
                StopCoroutine(co);
            }
            co = StartCoroutine(AvoidStationCO(distance));
        }
    }

    IEnumerator AvoidStationCO(Vector3 distance)
    {
        avoidingStation = true;
        Vector3 direction = transform.position.normalized * -1;
        Vector3 destination = transform.position + (direction * 2);


        while (destination != transform.position)
        {
            transform.up = Vector3.RotateTowards(transform.up, direction, 10 * Time.deltaTime, 0);
            transform.position = Vector3.MoveTowards(transform.position, destination, 2 * Time.deltaTime);
            yield return null;
        }
        avoidingStation = false;
        wandering();
        yield break;
    }

    IEnumerator WanderingCO()
    {
        behavior.currentState = IEnemyBasic.state.wandering;

        System.Random r = new System.Random();
        //Randomly choose a distance in X and Y direction
        int randomX = r.Next(0, 3);
        int randomY = r.Next(0, 3);

        //Randomly decide if the value is negative or positve;
        randomX *= r.Next(0, 2) * 2 - 1;
        randomY *= r.Next(0, 2) * 2 - 1;

        Vector3 destination = new Vector3(randomX, randomY, 0) + transform.position;

        while (destination != transform.position && behavior.currentState == IEnemyBasic.state.wandering)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 2 * Time.deltaTime);
            yield return null;
        }

        if (behavior.currentState == IEnemyBasic.state.wandering)
        {
            behavior.currentState = IEnemyBasic.state.idle;
        }
    }

    IEnumerator MoveFromCollisionCO()
    {
        Vector3 destination = RandomDirection();

        while (destination != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 2 * Time.deltaTime);
            yield return null;
        }
        movingFromCollision = false;
        avoidingStation = false;
        behavior.currentState = IEnemyBasic.state.idle;
    }

    private void wandering()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }
        co = StartCoroutine(WanderingCO());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (collision != currentCollision)
        {
            if (co != null)
            {
                StopCoroutine(co);
            }
            currentCollision = collision;
            movingFromCollision = true;
            co = StartCoroutine(MoveFromCollisionCO());
        }
        */
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        currentCollision = null;
        movingFromCollision = false;
    }

    private Vector3 RandomDirection()
    {
        System.Random r = new System.Random();
        //Randomly choose a distance in X and Y direction
        int randomX = r.Next(0, 3);
        int randomY = r.Next(0, 3);

        //Randomly decide if the value is negative or positve;
        randomX *= r.Next(0, 2) * 2 - 1;
        randomY *= r.Next(0, 2) * 2 - 1;

        Vector3 destination = new Vector3(randomX, randomY, 0) + transform.position;

        return destination;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stats.pursuitRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stats.attackingPursuitRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
    }
}

