using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMechanisedUnitBase : ShootingUnitBase, IUnit, IShooting, IDamagable, IMechanisedUnit
{
    [Header("Turret Logic")]
    [SerializeField, ChildGameObjectsOnly] private TurretIK turretController;

    public override void OnDeath()
    {
        // Do smth
    }

    protected override Transform FindClosest(Teams team)
    {
        Transform closest = base.FindClosest(team);
        turretController.Target = closest;
        return closest;
    }
}
