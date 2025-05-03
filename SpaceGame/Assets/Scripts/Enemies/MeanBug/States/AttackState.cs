using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AttackState : State
{
    // Start is called before the first frame update
    public MonoBehaviour monoBehavior;
    private bool isCollided = false;
    public Coroutine co;
    private enum Attacks { ram, tripleHit, none };
    private Attacks currentAttack = Attacks.none;
    GameObject gmObject;
    protected override void OnEnter()
    {

        gmObject = controller.gameObject;
        controller.visibleState = "Attack";
        IAttackingUnit comp = controller.gameObject.GetComponent<IAttackingUnit>();
        if (comp != null)
        {
            System.Random r = new System.Random();
            int flip = r.Next(0, 2);
            if (flip == 0)
            {
                co = controller.StartCoroutine(Ram_Melee());
            }
            else
            {
                co = controller.StartCoroutine(TripleHit_Melee());
            }
        }
    }

    IEnumerator Ram_Melee()
    {
        RaycastHit2D hit;
        //The point we are trying to hit
        Vector3 targetPoint = controller.gameObject.GetComponent<MeanBug>().target.transform.position;
        //The point to wind-up to
        Vector3 ramPoint = gmObject.transform.position + (-gmObject.transform.up * 5);
        while (gmObject.transform.position != ramPoint && !isCollided)
        {
            gmObject.transform.position = Vector3.MoveTowards(gmObject.transform.position, ramPoint, 10 * Time.deltaTime);
            yield return null;
        }
        //Give a window that the player can possibly dodge.
        yield return new WaitForSeconds(.5f);
        //Ram the target point.
        while (gmObject.transform.position != targetPoint && !isCollided)
        {
            gmObject.transform.position = Vector3.MoveTowards(gmObject.transform.position, targetPoint, 20 * Time.deltaTime);
            yield return null;
        }
        //If we are in range of the player, deal damage to the ship.
        hit = Physics2D.Raycast(gmObject.transform.position, gmObject.transform.up, .5f, 1 << LayerMask.NameToLayer("PlayerUnit"));
        if (hit.collider != null)
        {
            IDamageable _target = hit.collider.GetComponent<IDamageable>();
            if (_target != null)
            {
                _target.Damage(50);
            }
        }
        //Cooldown
        yield return new WaitForSeconds(1);
        controller.ChangeState(controller.idleState);
    }

    IEnumerator TripleHit_Melee()
    {
        currentAttack = Attacks.tripleHit;
        Vector3 startPoint;
        Vector3 targetPoint = controller.gameObject.GetComponent<MeanBug>().target.transform.position;

        for (int x = 0; x < 3; x++)
        {
            //Move forward to hit
            while (gmObject.transform.position != targetPoint && !isCollided)
            {
                gmObject.transform.position = Vector3.MoveTowards(gmObject.transform.position, targetPoint, 7 * Time.deltaTime);
                yield return null;
            }

            //Check if we hit the player
            RaycastHit2D hit = Physics2D.Raycast(gmObject.transform.position, gmObject.transform.up, .5f, 1 << LayerMask.NameToLayer("PlayerUnit"));
            if (hit.collider != null)
            {
                IDamageable _target = hit.collider.GetComponent<IDamageable>();
                if (_target != null)
                {
                    _target.Damage(5);
                }
            }

            //Move back
            startPoint = gmObject.transform.position + (-gmObject.transform.up * 1);
            while (gmObject.transform.position != startPoint)
            {
                gmObject.transform.position = Vector3.MoveTowards(gmObject.transform.position, startPoint, 7 * Time.deltaTime);
                yield return null;
            }
        }
        yield return new WaitForSeconds(.5f);
        controller.ChangeState(controller.idleState);
    }

    protected override void OnExit()
    {
        base.OnExit();
        if (co != null)
        {
            controller.StopCoroutine(co);
        }
        isCollided = false;
        currentAttack = Attacks.none;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isCollided = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isCollided = false;
    }
}
