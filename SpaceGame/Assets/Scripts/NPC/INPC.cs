using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public interface INPC
{

    public NPC_SO so { get; }
    public List<System.String> dialoguePaths { get; }
    public void InitConverstaion();
    public void LoadDialogue();
}
