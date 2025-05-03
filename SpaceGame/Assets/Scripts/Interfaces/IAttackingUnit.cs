using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackingUnit
{
    public void InitMeleeAttack(Collider2D target = null);
    public void InitRangedAttack();
}
