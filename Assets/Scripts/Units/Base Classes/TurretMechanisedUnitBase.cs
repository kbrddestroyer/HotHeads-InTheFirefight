using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMechanisedUnitBase : ShootingUnitBase, IUnit, IDamagable, IMechanisedUnit
{
    [SerializeField] private GameObject explode;

    public override void OnDeath()
    {
        base.OnDeath();
        Instantiate(explode, transform.position, Quaternion.identity);

        RagdollActivator[] activators = transform.GetComponentsInChildren<RagdollActivator>();

        foreach (RagdollActivator activator in activators)
        {
            activator.Switch(true);
        }
    }
}
