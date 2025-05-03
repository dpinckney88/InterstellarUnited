using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BasicAttacks : MonoBehaviour, IAttackingUnit
{
    // Start is called before the first frame update
    private enum Attacks { ram, tripleHit, none };
    private Attacks currentAttack = Attacks.none;
    private Collider2D target;
    public EnemyBaseStatsSO stats;
    private bool isInAttackRange;
    private IEnemyBasic behavior;
    private bool isCollided = false;
    private Rigidbody2D _rb;
    private Coroutine co;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        behavior = GetComponent<IEnemyBasic>();
        if (behavior == null)
        {
            //throw error;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (attackRangeCheck())
        {
            InitMeleeAttack(behavior.target);
        }

        if (behavior.currentState == IEnemyBasic.state.avoiding)
        {
            if (co != null)
            {
                StopCoroutine(co);
            }
        }

    }

    private bool attackRangeCheck()
    {
        int layerMask = 1 << LayerMask.NameToLayer("PlayerUnit");
        Collider2D collider = Physics2D.OverlapCircle(transform.position, stats.attackRange, layerMask);
        if (collider != null && collider == behavior.target)
        {
            return true;
        }
        return false;
    }

    public void InitMeleeAttack(Collider2D _target)
    {

        //If we are already attacking, do not let another attack happen.
        if (currentAttack != Attacks.none) { return; }
        if (co != null)
        {
            StopCoroutine(co);
        }
        target = _target;

        IAttackingUnit comp = gameObject.GetComponent<IAttackingUnit>();
        if (comp != null)
        {
            behavior.activeAttackType = IEnemyBasic.AttacKType.melee;
            behavior.currentState = IEnemyBasic.state.attacking;
            System.Random r = new System.Random();
            int flip = r.Next(0, 2);
            if (flip == 0)
            {
                currentAttack = Attacks.tripleHit;
                co = StartCoroutine(TripleHit_Melee());
            }
            else
            {
                currentAttack = Attacks.ram;
                co = StartCoroutine(Ram_Melee());
            }
        }
    }
    public void InitRangedAttack()
    {

        //No ranged attacks.
    }

    //A melee attack that hits 3 times
    IEnumerator TripleHit_Melee()
    {
        Vector3 startPoint;
        Vector3 targetPoint = behavior.target.transform.position;
        for (int x = 0; x < 3; x++)
        {
            //Move forward to hit
            while (transform.position != targetPoint && !isCollided)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, 7 * Time.deltaTime);
                yield return null;
            }

            //Check if we hit the player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, .5f, 1 << LayerMask.NameToLayer("PlayerUnit"));
            if (hit.collider != null)
            {
                IDamageable _target = hit.collider.GetComponent<IDamageable>();
                if (_target != null)
                {
                    _target.Damage(5);
                }
            }

            //Move back
            startPoint = transform.position + (-transform.up * 1);
            while (transform.position != startPoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPoint, 7 * Time.deltaTime);
                yield return null;
            }
        }
        //Cooldown
        AttackComplete();
        yield return new WaitForSeconds(2);
    }

    IEnumerator Ram_Melee()
    {
        RaycastHit2D hit;
        //The point we are trying to hit
        Vector3 targetPoint = behavior.target.transform.position;
        //The point to wind-up to
        Vector3 ramPoint = transform.position + (-transform.up * 5);
        while (transform.position != ramPoint && !isCollided)
        {
            transform.position = Vector3.MoveTowards(transform.position, ramPoint, 10 * Time.deltaTime);
            yield return null;
        }
        //Give a window that the player can possibly dodge.
        yield return new WaitForSeconds(.5f);
        //Ram the target point.
        while (transform.position != targetPoint && !isCollided)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, 20 * Time.deltaTime);
            yield return null;
        }
        //If we are in range of the player, deal damage to the ship.
        hit = Physics2D.Raycast(transform.position, transform.up, .5f, 1 << LayerMask.NameToLayer("PlayerUnit"));
        if (hit.collider != null)
        {
            IDamageable _target = hit.collider.GetComponent<IDamageable>();
            if (_target != null)
            {
                _target.Damage(50);
            }
        }
        //Cooldown
        AttackComplete();
        //yield return new WaitForSeconds(2);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isCollided = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isCollided = false;
    }

    private void AttackComplete()
    {
        behavior.currentState = IEnemyBasic.state.idle;
        behavior.activeAttackType = IEnemyBasic.AttacKType.none;
        currentAttack = Attacks.none;
    }

}
