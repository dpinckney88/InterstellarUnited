using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void engage();
    public string weaponDescription { get; set; }
}
