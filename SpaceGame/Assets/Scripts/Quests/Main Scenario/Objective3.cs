using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Objective3 : QuestObjective
{
    private bool bossStarted = false;
    private GameObject station2;
    public String objectiveTaskName { get; private set; } = "Return to Portharbor with 1000 credits.";
    private bool FutureWestonCutsceneIsPlaying = false;

    private GameObject[] allStations;
    private int numPackagesDelivered = 0;
    private bool interuptRan = false;
    private PlayableDirector director;
    protected override void OnEnter()
    {
        base.OnEnter();
        director = GameObject.Find("Cinematics/Titan Avanti Intro").GetComponent<PlayableDirector>();
        NarrativeProcessor.sceneComplete += SceneComplete;
        SpaceStation.packageDevlivered += PackageDevlivered;
        allStations = GameObject.FindGameObjectsWithTag("SpaceStation");
        SetupScene("Scene/FirstDayOnTheJob/Scene3.0");
        ShipModificationView.confirmWeponModification += modificationConfirmed;

    }

    private void modificationConfirmed(object sender, EventArgs e)
    {
        SetupScene("Scene/FirstDayOnTheJob/Scene3.2");
    }

    private void PackageDevlivered(object sender, PackageDevliveredArgs e)
    {
        numPackagesDelivered += e.packageIds.Count;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (numPackagesDelivered >= 2 && !interuptRan)
        {
            interuptRan = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<BasicShip_Core>().Repair(100);
            SetupScene("Scene/FirstDayOnTheJob/Scene3.1");
        }

        //Check if we are still playing the cutscene
        if (FutureWestonCutsceneIsPlaying == true)
        {
            if (director.state != PlayState.Playing)
            {
                FutureWestonCutsceneIsPlaying = false;
                SetupScene("Scene/FirstDayOnTheJob/Scene3.1");
            }
        }
    }

    protected override void OnExit()
    {
        SpaceStation.packageDevlivered -= PackageDevlivered;
        NarrativeProcessor.sceneComplete -= SceneComplete;
        ShipModificationView.confirmWeponModification -= modificationConfirmed;
    }

    public override string ObjectiveDescription()
    {
        return objectiveTaskName;
    }

    private void SceneComplete(object sender, SceneCompleteArgs e)
    {
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene3.0")
        {
            foreach (GameObject station in allStations)
            {
                station.GetComponent<SpaceStation>().AddPackages(10);
            }
            GameObject hatchet = Resources.Load<GameObject>("NPC/Hatchet");
            Utilities.GetSpaceStation(236).GetComponent<SpaceStation>().AddNPCToStation(hatchet);
        }
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene3.2")
        {
            //play cutscene
            director.Play();
            FutureWestonCutsceneIsPlaying = true;
        }

        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene3.2")
        {
            parentQuest.CompletedObjective(this);
        }
    }

    public override Transform TrackingPoint()
    {
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("SpaceStation"))
        {
            if (station.GetComponent<SpaceStation>().stationNumber == 236)
            {
                return station.transform;
            }
        }
        return null;
    }
}
