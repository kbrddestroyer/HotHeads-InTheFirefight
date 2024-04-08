using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;

public class FPS_ModeController : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float mouseSens;
    [SerializeField] private Camera localCamera;
    [SerializeField] private Camera globalCamera;
    [SerializeField] private Transform root;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField, AllowNull] private AIUnitController unitController;
    
    private Quaternion initialRotation;

    private void Awake()
    {
        globalCamera = Camera.main;
        initialRotation = root.rotation;
    }

    private bool State
    {
        set
        {
            globalCamera.enabled = !value;
            localCamera.enabled = value;

            agent.enabled = !value;
            if (unitController) unitController.enabled = !value;
        }
    }

    private void OnEnable()
    {
        State = true;
    }

    private void OnDisable()
    {
        State = false;
    }

    private void Update()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSens;

        if (Input.GetKeyDown(KeyCode.F))
            State = false;
    }
}
