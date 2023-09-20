using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    public Vector3 WorldPosition { get; }

    public void Register();

    public void ToggleSelection(bool state);
}
