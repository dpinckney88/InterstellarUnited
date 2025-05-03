using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AddNPCToStationArgs : EventArgs
{
    public int stationNum;
    public GameObject NPC;
}

public class AddPackageToShipArgs : EventArgs
{
    public DeliverySO so;
}

public class StoryEvents : MonoBehaviour
{
    public static event EventHandler removeObs;
    public static event EventHandler applyObs;
    public static event EventHandler<AddNPCToStationArgs> addNPCToStation;
    public static event EventHandler<AddPackageToShipArgs> addPackageToShip;
    public GameObject playerShip;
    [SerializeField] private Camera mainCamera;
    private bool ScreenBoundrySet = false;
    private bool doThisOnce = true;
    private IEnumerator co;
    [SerializeField] private GameObject visualNovelCanvas;
    [SerializeField] private GameObject shipUICanvas;
    public List<GameObject> allSpaceStations;
    public bool semiPausedState = false;
    private Objective1 objective1 = new Objective1();

    private List<QuestObjective> questObjectives = new List<QuestObjective>();
    public List<QuestObjective> activeObjectives { get; private set; }

    //private Scene currentScene;
    private Scene scene0 = new Scene0();
    private IQuest quest = new FirstDayOnTheJob();
    private List<QuestObjective> currentObjectives = new List<QuestObjective>();
    private List<IQuest> currentQuests = new List<IQuest>();
    [SerializeField] private GameObject boss;
    // Start is called before the first frame update
    void Start()
    {

        NarrativeProcessor.sceneStart += sceneStart;
        NarrativeProcessor.sceneComplete += sceneComplete;

        allSpaceStations = GameObject.FindGameObjectsWithTag("SpaceStation").ToList();

        playerShip = GameObject.FindWithTag("Player");

        if (playerShip == null)
        {
            //Throw an error
        }
    }

    public void StartChapter()
    {
        quest.Setup();
        TrackQuestObjectives(quest);
    }

    private void questObjectiveComplete(object sender, QuestcompleteArgs e)
    {
        throw new NotImplementedException();
    }

    private void QuestObjectiveComplete(object sender, QuestcompleteArgs e)
    {
        //Stop tracking the objective as it is now complete
        currentObjectives.Remove(e.questObjective);

        //Go to the next objective of the quest if it has one
        TrackQuestObjectives(e.quest);
    }

    public void EnableBoss()
    {
        SetupBossBattle(null);
        boss.SetActive(true);
        boss.GetComponent<AbyssBehavior>().ToggleActivation(true);
    }


    public void TrackQuestObjectives(IQuest _quest)
    {
        //If the quest isn't being tracked, track it
        if (!currentQuests.Contains(_quest))
        {
            //Register Listener for the Quest
            _quest.questObjectiveComplete += QuestObjectiveComplete;

            //Track the quest
            currentQuests.Add(_quest);
        }

        //Assume quest is complete until the foreach runs
        bool questComplete = true;

        //Initialize the objective and track it
        foreach (QuestObjective objective in _quest.GetNextObjective())
        {
            objective.OnStateEnter(this, _quest);
            currentObjectives.Add(objective);

            //If we are in the foreach, then we found one or more objectives and the quest is not complete.
            questComplete = false;
        }

        //Remove completed quests from the tracking list
        if (questComplete)
        {
            currentQuests.Remove(_quest);

            //Unregister Listener for the Quest
            _quest.questObjectiveComplete -= QuestObjectiveComplete;
        }
        else
        {
            ObjectiveDisplay.instance.DisplayQuest(_quest);
        }
    }


    private void sceneComplete(object sender, SceneCompleteArgs e)
    {
        semiPausedState = false;
    }

    private void sceneStart(object sender, SceneStartArgs e)
    {
        semiPausedState = true;
    }

    void Update()
    {
        for (int x = 0; x < currentObjectives.Count(); x++)
        {
            if (currentObjectives[x] != null)
            {
                currentObjectives[x].OnStateUpdate();
            }
        }
    }

    public void AddNPCToStation(int station, GameObject npc)
    {
        addNPCToStation?.Invoke(this, new AddNPCToStationArgs() { stationNum = station, NPC = npc });
    }

    public void ShowVisualNovel(bool show = true)
    {
        visualNovelCanvas.SetActive(show);
    }


    public void ShowShipUI(bool show = true)
    {
        shipUICanvas.SetActive(show);
    }

    private void RemoveObsticles()
    {
        if (removeObs != null && doThisOnce)
        {
            removeObs(this, null);
        }
    }

    private void ApplyObsticles()
    {
        if (applyObs != null)
        {
            applyObs(this, null);
        }
    }

    private IEnumerator ScreenBoundryLock()
    {
        if (!ScreenBoundrySet)
        {
            //Stop the camera from moving with the ship
            mainCamera.GetComponent<MainCamera>().ShouldTrack(false);
            ScreenBoundrySet = true;
        }
        while (ScreenBoundrySet)
        {
            //Get the bottom right coordinates (max point) of the screen in world space
            Vector2 screenBoundriesMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            //Get the mid point of the screen in worldspace
            Vector2 midScreen = new Vector3(0, 0, Camera.main.transform.position.z);
            //Get the top left of the screen in world space
            Vector2 topLeft = midScreen - screenBoundriesMax;
            //Get the min point of the screen in world space
            Vector2 screenBoundriesMin = Camera.main.ScreenToWorldPoint(topLeft);

            Vector3 shipPos = playerShip.transform.position;

            //Get the mid point of the ship so we can calculate the true bounds of the screen
            float xOffset = playerShip.GetComponent<SpriteRenderer>().bounds.size.x / 2;
            float yOffset = playerShip.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            //Do not let the ship move outside of the screenbounds offset by the size of the ship
            shipPos.x = Mathf.Clamp(shipPos.x, screenBoundriesMin.x + xOffset, screenBoundriesMax.x - xOffset);
            shipPos.y = Mathf.Clamp(shipPos.y, screenBoundriesMin.y + yOffset, screenBoundriesMax.y - yOffset);
            playerShip.transform.position = shipPos;


            yield return null;
        }
    }

    public void SetupBossBattle(GameObject boss)
    {
        RemoveObsticles();
        co = ScreenBoundryLock();
        StartCoroutine(co);
        //SpawnBoss(boss);
    }

    private void SpawnBoss(GameObject boss)
    {
        Vector3[] screenBounds = Utilities.GetScreenBounds();
        float xOffset = boss.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float yOffset = boss.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        boss = GameObject.Instantiate(boss);
        boss.transform.position = new Vector3(screenBounds[1].x - xOffset, screenBounds[1].y - yOffset);
    }

    public void AddPackages(int[] stationNums, int numberOfPackages)
    {
        if (stationNums == null)
        {
            foreach (GameObject station in allSpaceStations)
            {
                station.GetComponent<SpaceStation>().AddPackages(numberOfPackages);
            }
        }
        else
        {
            allSpaceStations.Find(station => stationNums.Contains(station.GetComponent<SpaceStation>().stationNumber));
        }
    }

    public void AddPackageToShip(DeliverySO so)
    {
        addPackageToShip?.Invoke(this, new AddPackageToShipArgs() { so = so });
    }

}
