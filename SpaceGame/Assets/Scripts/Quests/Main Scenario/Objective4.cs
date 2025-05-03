using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Objective4 : QuestObjective
{
    private bool bossStarted = false;
    private GameObject station2;
    private bool hasCollectedEnoughCredits = false;
    private GameObject portharborStation;
    private GameObject player, bossSpawn;
    private bool bossSceneStarted = false;
    private PlayableDirector director;
    public String objectiveTaskName { get; private set; } = "Kill Titan Avanti";
    protected override void OnEnter()
    {
        base.OnEnter();
        //Cutscene
        director = GameObject.Find("Cinematics/Enter Boss Fight").GetComponent<PlayableDirector>();

        //Listen for the Abyss to be defeated
        AbyssBehavior.AbyssDefeated += AbyssDefeated;

        NarrativeProcessor.sceneComplete += SceneComplete;
        player = GameObject.FindGameObjectWithTag("Player");
        bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
    }

    private void AbyssDefeated(object sender, EventArgs e)
    {
        Debug.Log("Got here!!");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        //Run the boss dialouge and cutscene when in range of boss spawn point
        float distance = Vector3.Distance(bossSpawn.transform.position, player.transform.position);
        if (distance <= 1 && !bossSceneStarted)
        {
            bossSceneStarted = true;
            SetupScene("Scene/FirstDayOnTheJob/Scene2.1");
        }
    }

    protected override void OnExit()
    {
        base.OnExit();
        AbyssBehavior.AbyssDefeated -= AbyssDefeated;
        NarrativeProcessor.sceneComplete -= SceneComplete;
    }

    public override string ObjectiveDescription()
    {
        return objectiveTaskName;
    }

    private void SceneComplete(object sender, SceneCompleteArgs e)
    {
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene2.1")
        {
            director.Play();
        }
    }

    public override Transform TrackingPoint()
    {
        MiniMapDestinationPoint.instance.SetTrackingPoint(GameObject.FindGameObjectWithTag("BossSpawn").transform);
        return GameObject.FindGameObjectWithTag("BossSpawn").transform;
    }
}
