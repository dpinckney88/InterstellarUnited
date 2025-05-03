using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class Scene0 : Scene
{
    private List<String> MissionDialogue = new List<String>(){
        "Scene/Scene1/Scene1.1",
        "Scene/Scene1/Scene1.2",
        "Scene/Scene1/Scene1.3"
    };
    private int currentDialoge = 0;
    private bool deliverFivePackagesStep = false;
    private bool deilveredThree = false;
    private int deliverFivePackagesStepCount = 0;
    private bool bossStarted = false;
    private bool bossDialogueSeen = false;
    private GameObject station2;
    // Start is called before the first frame update
    protected override void OnEnter()
    {
        base.OnEnter();
        NarrativeProcessor.sceneComplete += SceneComplete;
        SpaceStation.packageDevlivered += DeliveredPackages;
        station2 = storyEvents.allSpaceStations.Find(station => station.GetComponent<SpaceStation>().stationNumber == 9);

        //STEP: 1 - Intial dialogue
        SetupScene(MissionDialogue[currentDialoge]);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        //STEP 3A - Run dialogue at appropirate times
        if (deliverFivePackagesStep)
        {
            if (deliverFivePackagesStepCount >= 3 && !deilveredThree)
            {
                //After 3 packages have been delivered
                deilveredThree = true;
                //STEP 4 - Start Scene 1.3
                SetupScene(MissionDialogue[currentDialoge]);
                GameObject talon = GameObject.Find("NPCRegistry").GetComponent<NPCRegistry>().GetNPCByName("Talon");
                storyEvents.AddNPCToStation(9, talon);
                GameObject[] temp = GameObject.FindGameObjectsWithTag("SpaceStation");
                foreach (GameObject station in temp)
                {
                    if (station.GetComponent<SpaceStation>().stationNumber == 9)
                    {
                        station.GetComponent<SpaceStation>().Refuel(10000, 0);
                    }
                }
            }

            if (deliverFivePackagesStepCount >= 5 && deilveredThree)
            {
                //After 5 packages have been delivered
                deliverFivePackagesStep = false;

                DeliverySO package = Resources.Load<DeliverySO>("ScriptableObjects/Packages/Unknown Package");
                storyEvents.AddPackageToShip(package);
                //We should also handle what happens if the ship has too many packages on board already.
            }
        }
        //Start boss
        if (deliverFivePackagesStepCount >= 5 && !deliverFivePackagesStep && !bossStarted)
        {
            //Get distance to station
            if (Vector3.Distance(storyEvents.playerShip.transform.position, station2.transform.position) <= 10)
            {
                if (!bossDialogueSeen)
                {
                    bossDialogueSeen = true;
                    SetupScene("Scene/Scene1/Boss");
                }
            }

        }
    }

    private void DeliveredPackages(object sender, PackageDevliveredArgs e)
    {
        //STEP: 2 - After package gets delivered, add packages to other stations
        if (e.packageIds.Contains(151))
        {
            SetupScene(MissionDialogue[currentDialoge]);
            storyEvents.AddPackages(null, 2);
            deliverFivePackagesStep = true;
        }

        //STEP: 3 - Track number of deliveries
        if (deliverFivePackagesStep)
        {
            deliverFivePackagesStepCount += e.packageIds.Count;
        }
    }

    private void SceneComplete(object sender, SceneCompleteArgs e)
    {
        storyEvents.ShowVisualNovel(false);
        storyEvents.ShowShipUI(true);

        if (e.ScenePath == "Scene/Scene1/Boss")
        {
            bossStarted = true;
            GameObject boss = Resources.Load<GameObject>("Enemies/Bosses/The Abyss");
            storyEvents.SetupBossBattle(boss);
        }
    }

    private void SetupScene(String scene)
    {
        storyEvents.ShowVisualNovel(true);
        storyEvents.ShowShipUI(false);
        LoadScene(scene);
        currentDialoge++;
    }
}
