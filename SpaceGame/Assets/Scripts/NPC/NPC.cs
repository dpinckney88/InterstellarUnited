using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class NPC : MonoBehaviour
{
    public GameObject button;
    public NPC_SO so;
    public static event EventHandler<LoadSceneArgs> npcDialogue;
    [SerializeField] private List<String> dialoguePaths;
    // Start is called before the first frame update

    void Awake()
    {
        button.GetComponent<Image>().sprite = so.image;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void InitConverstaion()
    {
        npcDialogue?.Invoke(this, new LoadSceneArgs() { scenePath = dialoguePaths[0] });
    }
}
