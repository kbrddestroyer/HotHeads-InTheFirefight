using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUnitList : MonoBehaviour
{
    [SerializeField] private UnitStructureInformation[] unitStructure;

    public UnitStructureInformation[] Units { get => unitStructure; }

    public void OnValidate()
    {
        foreach (UnitStructureInformation unit in unitStructure)
        {
            unit.text = unit.unit.UnitName;
        }
    }
}
