using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOWController : MonoBehaviour
{
    /*
     *  
     */

    [Header("FOW Controller values")]
    [Tooltip("Range value (in units)")]
    [SerializeField, Range(0f, 50f), LabelText("Cutoff Range")] private float fCutoffRange;
    [Header("Required")]
    [SerializeField, ChildGameObjectsOnly] private GameObject meshRoot;
    
    
}
