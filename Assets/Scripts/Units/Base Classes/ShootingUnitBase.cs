using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingController))]
public abstract class ShootingUnitBase : UnitBase, IUnit, ISelectable, IDamagable
{
    /*
     *  ShootingUnitBase - general class for shooting units. Children can override general methods to create unique
     *  shooting logic (e.g. anti-tank infantry, CIWS/CRAM, airforces, etc.)
     *  
     *  Seek-and-destroy algorythm:
     *  1. ShootingBaseLogic [VIRTUAL]
     *  | 1.1. FindClosest
     *  
     * 
     */

    [Space]
    [Title("ShootingUnitBase Settings", "Settings for shooting units, such as fire rate, base damage, etc.", horizontalLine: true, bold: true, TitleAlignment = TitleAlignments.Centered)]

    #region EDITOR_VARIABLES
    [SerializeField] private ShootingController controller;
    #endregion

    public ShootingController Controller { get => controller; }

    public virtual void Attack(Transform target, Teams team)
    {
        controller.Attack(target, team);
    }

    public virtual void ShootingBaseLogic(Teams team)
    {
        controller.ShootingBaseLogic(team);
    }
    
    #region UNIT_BASE_EXTENDED

    protected override void Update()
    {
        // Lifetime

        base.Update();
        ShootingBaseLogic(team);
    }

    #endregion
}
