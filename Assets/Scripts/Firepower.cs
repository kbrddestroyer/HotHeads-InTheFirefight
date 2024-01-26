using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Firepower
{
    private int infantryWeight;
    private int mechanisedWeight;

    public int Infantry
    {
        get => infantryWeight;
        set
        {
            if (value >= 0) infantryWeight = value; 
        }
    }

    public int Mechanised
    {
        get => mechanisedWeight;
        set
        {
            if (value >= 0)
                mechanisedWeight = value;
        }
    }
}
