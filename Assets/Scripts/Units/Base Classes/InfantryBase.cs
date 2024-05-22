using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class InfantryBase : ShootingUnitBase, IUnit, ISelectable, IDamagable, IRagdoll, IShooting
{
    [Space]
    [Title("InfantryBase Class Settings", "Settings for infantry specific settings", horizontalLine: true, bold: true, TitleAlignment = TitleAlignments.Centered)]
    [SerializeField, Tooltip("Ragdoll activator is a component, that toggles ragdoll physics on specific event")] private RagdollActivator[] ragdollActivators;
    [SerializeField, LabelText("Rigidbody")] private Rigidbody rb;

    // IRagdoll implemented
    public void Switch(bool bSwitch)
    {
        foreach (RagdollActivator activator in ragdollActivators)
        {
            activator.Switch(bSwitch);
        }
    }

    // UnitBase implemented
    public override void OnDeath()
    {
        base.OnDeath();
        Switch(true);
    }

#if UNITY_EDITOR
    [Button("Get Ragdoll Activators")]
    protected void GetActivators()
    {
        ragdollActivators = GetComponentsInChildren<RagdollActivator>().ToArray();
    }

    protected override void FillRequired()
    {
        base.FillRequired();
        rb = GetComponent<Rigidbody>();
    }
#endif
}
