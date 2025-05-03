using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    public GameObject refuelStation;
    public GameObject playerHUD;
    public GameObject narrativeUI;
    public GameObject shipModification;
    public GameObject delivered;
    [SerializeField] private TMP_Text totalScore;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateScore();
        NarrativeProcessor.sceneStart += NarrativeSceneStarted;
        NarrativeProcessor.sceneComplete += NarrativeSceneComplete;
    }

    private void NarrativeSceneComplete(object sender, SceneCompleteArgs e)
    {
        if (!playerHUD.activeSelf)
        {
            playerHUD.SetActive(true);
        }
    }

    private void NarrativeSceneStarted(object sender, SceneStartArgs e)
    {
        //playerHUD.SetActive(false);
    }

    public void CloseAllGUI()
    {
        refuelStation.SetActive(false);
        narrativeUI.SetActive(false);
        shipModification.SetActive(false);
        narrativeUI.SetActive(false);
    }

    public void ShowGUI(string GUI)
    {
        CloseAllGUI();
        switch (GUI)
        {
            case "ShipModification":
                shipModification.SetActive(true);
                break;
            case "Visual Novel":
                narrativeUI.SetActive(true);
                break;
            case "Delivered":
                delivered.SetActive(true);
                break;
        }
    }


    public void CloseGUI(string GUI)
    {
        switch (GUI)
        {
            case "ShipModification":
                shipModification.SetActive(false);
                break;
            case "Visual Novel":
                narrativeUI.SetActive(false);
                break;
        }
    }

    public void UpdateScore()
    {
        totalScore.text = PlayerVault.instance.score.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
