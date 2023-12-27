using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalUnitList : ScriptableObject
{
    [SerializeField] private UnitStructureInformation[] unitStructure;

    public UnitStructureInformation[] Units { get => unitStructure; }
}
