using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeanBug : MonoBehaviour, IDamageable, IEnemyBasic, ISpaceStationComms
{
    public EnemyBaseStatsSO baseStats;
    private int currentHealth;
    public IEnemyBasic.state currentState { get; set; }
    public IEnemyBasic.AttacKType activeAttackType { get; set; }
    public Collider2D target { get; set; }
    public IEnemyBasic.state testState;



    // Start is called before the first frame update

    void Start()
    {
        StoryEvents.removeObs += RemoveObs;
        currentHealth = baseStats.maxHealth;
        currentState = IEnemyBasic.state.idle;
        activeAttackType = IEnemyBasic.AttacKType.none;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        testState = currentState;
    }

    private void RemoveObs(object sender, EventArgs e)
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
    public void Damage(float damage)
    {
        currentHealth -= (int)damage;
        if (currentHealth <= 0)
        {
            PlayerVault.instance.IncreaseScore(100);
            Destroy(this.gameObject);
        }
    }

    public void InRangeOfStation(SpaceStation station)
    {
        gameObject.GetComponent<MeanBugStateController>().AvoidStation(station);
    }

    void OnDestroy()
    {
        StoryEvents.removeObs -= RemoveObs;
    }
}
