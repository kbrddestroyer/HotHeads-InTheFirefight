using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDocumentController : MonoBehaviour
{
    [SerializeField] ButtonController[] controllers;

    private void Awake()
    {
        foreach (ButtonController controller in controllers)
        {
            controller.Initialise();
        }
    }
}
