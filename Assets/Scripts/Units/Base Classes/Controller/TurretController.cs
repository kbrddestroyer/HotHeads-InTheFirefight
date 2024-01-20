using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : ShootingController
{
    [Header("Turret Logic")]
    [SerializeField, ChildGameObjectsOnly] private TurretIK turretController;

    protected override Transform FindClosest(Teams team)
    {
        Transform closest = base.FindClosest(team);
        turretController.Target = closest;
        return closest;
    }
}
