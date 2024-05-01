using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FPS_ModeController : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float mouseSens;
    [SerializeField, Range(0f, 10f)] private float walkSpeed;
    [SerializeField] private Camera localCamera;
    [SerializeField] private Camera globalCamera;
    [SerializeField] private Transform root;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField, AllowNull] private AIUnitController aiUnitController;
    [SerializeField] private UnitBase unitController;
    [SerializeField, AllowNull] private ShootingController shootingController;
    [SerializeField, AllowNull] private Animator animator;
    [SerializeField] private new Collider collider;
    [SerializeField] private Rigidbody rb;

    private float fTimePassed = 0f;
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
            // On change any state we need to reset rotation to idle idfk

            globalCamera.enabled = !value;
            globalCamera.transform.root.gameObject.SetActive(!value);
            localCamera.enabled = value;
            localCamera.gameObject.SetActive(value);

            agent.enabled = !value;
            if (aiUnitController) aiUnitController.enabled = !value;
            if (shootingController) shootingController.FpsMode = value;

            collider.isTrigger = !value;
            rb.isKinematic = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
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

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * walkSpeed * Time.deltaTime;

        transform.position += transform.right * movement.x + transform.forward * movement.y;

        // Rotate

        transform.localRotation = Quaternion.Euler(Vector3.up * mouseInput.x + Vector3.left * mouseInput.y + transform.localRotation.eulerAngles);

        if (Input.GetKeyDown(KeyCode.F))
            enabled = false;

        if (Input.GetKeyDown(KeyCode.Mouse0) && animator)
        {
            animator.SetBool("shooting", true);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            fTimePassed += Time.deltaTime;
            shootingController.ShootProjectile(ref fTimePassed, unitController.Team);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            fTimePassed = 0f;
            if (animator)
                animator.SetBool("shooting", false);
        }
    }

#if UNITY_EDITOR
    [Button("Fill requirements")]
    private void Fill()
    {
        unitController = GetComponent<UnitBase>();
        aiUnitController = GetComponent<AIUnitController>();
        shootingController = GetComponent<ShootingController>();
        agent = GetComponent<NavMeshAgent>();
        localCamera = GetComponentInChildren<Camera>();
        globalCamera = Camera.main;
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if (!root)
        {
            Debug.LogWarning("Autofill can't find root transform. Please, input manually");
            EditorUtility.DisplayDialog("Warning", "Autofill can't find root transform. Please, input manually", "Ok");
        }
    }
#endif
}
