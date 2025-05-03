using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRegistry : MonoBehaviour
{
    public List<GameObject> NPCs;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GetNPCByName(string name)
    {
        return NPCs.FindAll(npc => npc.GetComponent<INPC>().so.npcName == name)[0];
    }

    public GameObject GetNPCByID(int id)
    {
        return NPCs.FindAll(npc => npc.GetComponent<INPC>().so.id == id)[0];
    }
}
