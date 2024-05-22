using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanisedBase : UnitBase, IMechanisedUnit
{
    [SerializeField] private GameObject explode;

    public override void OnDeath()
    {
        Instantiate(explode, transform.position, Quaternion.identity);
    }
}
