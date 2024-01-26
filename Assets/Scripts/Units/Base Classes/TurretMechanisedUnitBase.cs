using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMechanisedUnitBase : ShootingUnitBase, IUnit, IDamagable, IMechanisedUnit
{
    public override void OnDeath()
    {
        // Do smth.
    }
}
