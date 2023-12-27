using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class UnitStructureInformation : TMP_Dropdown.OptionData
{
    public UnitBase unit;
    public Teams team;

    public void OnValidate()
    {
        text = unit.name;
    }
}
