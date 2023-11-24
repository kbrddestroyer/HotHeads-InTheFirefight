using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class InfantryBase : ShootingUnitBase, IUnit, ISelectable, IDamagable, IRagdoll, IShooting
{
    [SerializeField] private RagdollActivator[] ragdollActivators;
    // IRagdoll implemented
    public void Switch(bool bSwitch)
    {
        GetComponent<Rigidbody>().isKinematic = bSwitch;
        GetComponent<Collider>().enabled = !bSwitch;
        GetComponent<Animator>().enabled = !bSwitch;
        foreach (RagdollActivator activator in ragdollActivators)
        {
            activator.Switch(bSwitch);
        }
    }

    // UnitBase implemented
    public override void OnDeath()
    {
        Switch(true);
        Destroy(this);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ragdollActivators = GetComponentsInChildren<RagdollActivator>().ToArray();
    }
#endif
}
