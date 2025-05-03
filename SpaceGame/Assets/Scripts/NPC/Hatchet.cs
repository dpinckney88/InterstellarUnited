using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Hatchet : MonoBehaviour, INPC
{
    public GameObject button;
    public bool introComplete = false;
    private bool hasCompletedFirstDayOnTheJobQuest = false;
    [field: SerializeField] public NPC_SO so { get; private set; }

    [field: SerializeField] public List<string> dialoguePaths { get; private set; }


    void Awake()
    {
        button.GetComponent<Image>().sprite = so.image;
    }
    // Start is called before the first frame update
    void Start()
    {
        NarrativeProcessor.sceneComplete += CompletedScene;
    }

    private void CompletedScene(object sender, SceneCompleteArgs e)
    {
        if (e.ScenePath == "Scene/FirstDayOnTheJob/Scene3.1.1")
        {
            //load upgrade screen
            CanvasManager.instance.ShowGUI("ShipModification");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitConverstaion()
    {
        if (!hasCompletedFirstDayOnTheJobQuest)
        {
            if (PlayerVault.instance.money >= 1000)
            {
                hasCompletedFirstDayOnTheJobQuest = true;
                NarrativeProcessor.instence.loadScene("Scene/FirstDayOnTheJob/Scene3.1.1");
            }
            else
            {
                //Not enough dialouge
                NarrativeProcessor.instence.loadScene("Scene/FirstDayOnTheJob/Scene3.1.2");
            }
        }

    }



    public void LoadDialogue()
    {

    }
}
