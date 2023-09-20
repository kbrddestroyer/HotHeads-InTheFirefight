using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfantryBase : UnitBase, IUnit, ISelectable, IDamagable, IRagdoll
{
    [SerializeField] private RagdollActivator[] ragdollActivators;
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
