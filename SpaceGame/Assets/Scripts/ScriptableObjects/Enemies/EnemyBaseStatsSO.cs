using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy Base Stats SO", order = 1)]
public class EnemyBaseStatsSO : ScriptableObject
{
    public int maxHealth;
    public int pursuitRange;
    public int attackingPursuitRange;
    public int attackRange;
}
