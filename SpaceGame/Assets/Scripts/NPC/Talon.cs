using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Talon : MonoBehaviour, INPC
{
    public GameObject button;
    public bool introComplete = false;
    [field: SerializeField] public NPC_SO so { get; private set; }

    [field: SerializeField] public List<string> dialoguePaths { get; private set; }


    void Awake()
    {
        button.GetComponent<Image>().sprite = so.image;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitConverstaion()
    {
        NarrativeProcessor.instence.loadScene(dialoguePaths[0]);
    }

    public void LoadDialogue()
    {

    }
}
