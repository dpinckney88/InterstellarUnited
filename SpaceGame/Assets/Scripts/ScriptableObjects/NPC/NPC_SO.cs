using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
[CreateAssetMenu(fileName = "NPC_SO", menuName = "NPC SO", order = 1)]
public class NPC_SO : ScriptableObject
{
    public Sprite image;
    public string npcName;
    public int id;
}
