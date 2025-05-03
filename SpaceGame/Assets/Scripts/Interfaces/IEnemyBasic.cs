using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBasic
{
    public enum state { idle, wandering, pursuit, attacking, avoiding };
    public enum AttacKType { melee, ranged, none };
    public AttacKType activeAttackType { get; set; }
    public state currentState { get; set; }
    public Collider2D target { get; set; }

}
