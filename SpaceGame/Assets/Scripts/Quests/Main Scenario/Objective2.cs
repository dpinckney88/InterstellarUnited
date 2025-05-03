using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective2 : QuestObjective
{
    private List<String> MissionDialogue = new List<String>(){
        "Scene/Scene1/Scene1.3"
    };
    private bool deilveredThree = false;
    private int deliverFivePackagesStepCount = 0;
    private bool deilveredFive = false;
    private int numPackagesDeliveredForObjective = 0;
    private bool shouldBeAtHarriatter;
    private GameObject riverstoneStation, harriatterStation, player;

    public String objectiveTaskName { get; private set; } = "Deliver the 2 fragile packages.";

    protected override void OnEnter()
    {
        base.OnEnter();

        SpaceStation.packageDevlivered += DeliveredPackages;
        NarrativeProcessor.sceneComplete += SceneComplete;
        List<DeliverySO> delivery = GlobalPackageRegistry.instance.GetPackageByID(new int[] { 5, 7 });
        riverstoneStation = Utilities.GetSpaceStation(207);
        harriatterStation = Utilities.GetSpaceStation(242);
        player = GameObject.FindGameObjectWithTag("Player");

        //Add packages to station 207
        riverstoneStation.GetComponent<SpaceStation>().AddPackage(delivery[0]);
        riverstoneStation.GetComponent<SpaceStation>().AddPackage(delivery[1]);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (numPackagesDeliveredForObjective == 2 && !NarrativeProcessor.instence.isActive())
        {
            numPackagesDeliveredForObjective = 0;
            SetupScene("Scene/FirstDayOnTheJob/Scene2.0");
        }

        //See when player gets close to St. Harriatter.
        if (shouldBeAtHarriatter)
        {
            float distance = Vector3.Distance(harriatterStation.transform.position, player.transform.position);
            if (distance <= 4)
            {
                shouldBeAtHarriatter = false;
                SetupScene("Scene/FirstDayOnTheJob/Scene2.1");
            }
        }
    }

    public override string ObjectiveDescription()
    {
        return objectiveTaskName;
    }

    private void DeliveredPackages(object sender, PackageDevliveredArgs e)
    {
        //Fragile packages
        if (e.packageIds.Contains(5) || e.packageIds.Contains(7))
        {
            numPackagesDeliveredForObjective++;
        }

        //Secret package and decoy package
        if (e.packageIds.Contains(100) && e.packageIds.Contains(300))
        {
            parentQuest.CompletedObjective(this);
        }

    }

    private void SceneComplete(object sender, SceneCompleteArgs e)
    {
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene2.0")
        {
            objectiveTaskName = "Go to St. Harriatter's Station";
            shouldBeAtHarriatter = true;
            MiniMapDestinationPoint.instance.SetTrackingPoint(Utilities.GetSpaceStation(242).transform);
            ObjectiveDisplay.instance.DisplayQuest(parentQuest);
        }
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene2.1")
        {
            PackageList.instance.AcceptPackage(GlobalPackageRegistry.instance.GetPackageByID(new int[] { 100 })[0]);
            PackageList.instance.AcceptPackage(GlobalPackageRegistry.instance.GetPackageByID(new int[] { 300 })[0]);
            RefuelStation.instance.AddFuelBypass(1000);
        }
    }

    public override Transform TrackingPoint()
    {
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("SpaceStation"))
        {
            if (station.GetComponent<SpaceStation>().stationNumber == 255)
            {
                return station.transform;
            }
        }
        return null;
    }

    protected override void OnExit()
    {
        SpaceStation.packageDevlivered -= DeliveredPackages;
        NarrativeProcessor.sceneComplete -= SceneComplete;
    }
}