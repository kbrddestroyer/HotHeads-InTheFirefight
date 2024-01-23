using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : UnitBase
{
    [SerializeField] private RagdollActivator physicsRoot;

    public override void OnDeath()
    {
        physicsRoot.Switch(true);
    }
}
