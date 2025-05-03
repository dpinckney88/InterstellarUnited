using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssBehavior : MonoBehaviour, IDamageable, IEnemyBasic
{
    public IEnemyBasic.AttacKType activeAttackType { get; set; }
    public IEnemyBasic.state currentState { get; set; }
    public Collider2D target { get; set; }

    public static event EventHandler AbyssDefeated;

    private int currentHP = 1;

    void Awake()
    {
        ToggleActivation(false);
    }
    public void Damage(float damage)
    {
        //throw new System.NotImplementedException();
        currentHP -= (int)damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = IEnemyBasic.state.idle;
        activeAttackType = IEnemyBasic.AttacKType.none;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 0)
        {
            AbyssDefeated?.Invoke(this, new EventArgs());
            Destroy(gameObject);
        }
    }

    public void ToggleActivation(bool activate)
    {
        gameObject.GetComponent<AbyssMovementBehavior>().enabled = activate;
        gameObject.GetComponent<BossOneAuxComponents>().enabled = activate;
    }
}
