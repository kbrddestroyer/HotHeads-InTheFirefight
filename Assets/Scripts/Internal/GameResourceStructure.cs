using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GameResourceStructure
{
    [SerializeField] private GameResources type;
    [SerializeField] private float amount;
    [SerializeField, Range(0f, 10f)] public float baseAppliance;

    private float appliance;

    [SerializeField, AllowNull] private TMP_Text label;
    [SerializeField, AllowNull] private TMP_Text applianceLabel;

    public GameResources Type { get => type; }

    public float Amount
    {
        get => amount;
        set
        {
            amount = (value > 0) ? value : 0;
            if (label != null)
                label.text = amount.ToString();
        }
    }

    public float Appliance
    {
        get => appliance;
        set
        {
            appliance = (value > 0) ? value : 0;
            if (applianceLabel != null)
                applianceLabel.text = appliance.ToString();
        }
    }
}
