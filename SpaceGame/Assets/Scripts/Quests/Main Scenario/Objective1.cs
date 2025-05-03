using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


//Objective: Deliver package 151
public class Objective1 : QuestObjective
{

    private List<String> MissionDialogue = new List<String>(){
        "Scene/Scene1/Scene1.1",
        "Scene/Scene1/Scene1.2",
    };

    public String objectiveTaskName { get; private set; } = "Deliver the Package to Station 207";

    protected override void OnEnter()
    {
        base.OnEnter();
        SpaceStation.packageDevlivered += DeliveredPackages;
        NarrativeProcessor.sceneComplete += SceneComplete;
        SetupScene("Scene/FirstDayOnTheJob/Scene1.0");
    }

    protected override void OnExit()
    {
        SpaceStation.packageDevlivered -= DeliveredPackages;
        NarrativeProcessor.sceneComplete -= SceneComplete;
    }

    public override string ObjectiveDescription()
    {
        return objectiveTaskName;
    }

    private void DeliveredPackages(object sender, PackageDevliveredArgs e)
    {
        //After package gets delivered, add packages to other stations
        if (e.packageIds.Contains(151))
        {
            SetupScene("Scene/FirstDayOnTheJob/Scene1.1");
        }
    }


    private void SceneComplete(object sender, SceneCompleteArgs e)
    {
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene1.1")
        {
            parentQuest.CompletedObjective(this);
        }
        else if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene1.0")
        {
            MiniMapDestinationPoint.instance.SetTrackingPoint(TrackingPoint());
        }
    }

    public override Transform TrackingPoint()
    {
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("SpaceStation"))
        {
            if (station.GetComponent<SpaceStation>().stationNumber == 207)
            {
                return station.transform;
            }
        }
        return null;
    }


}
