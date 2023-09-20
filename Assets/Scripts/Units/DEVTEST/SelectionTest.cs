using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTest : UnitBase, ISelectable
{
    private new MeshRenderer renderer;

    protected override void Awake()
    {
        base.Awake();
        renderer = GetComponent<MeshRenderer>();
    }

    public override void ToggleSelection(bool state)
    {
        renderer.material.color = (state ? Color.green : Color.red);
    }

    public override void OnDeath()
    {
        Destroy(this.gameObject);
    }
}
