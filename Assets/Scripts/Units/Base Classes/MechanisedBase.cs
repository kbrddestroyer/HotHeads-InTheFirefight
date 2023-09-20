using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanisedBase : UnitBase, IMechanisedUnit
{
    public override void OnDeath()
    {
        Debug.LogError($"{name} has no OnDeath method");
    }
}
