using Sirenix.OdinInspector;
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]

[Title("UnitBase Class Settings", "Settings for Unit basics, such as HP, speed, etc.", horizontalLine: true, bold: true, TitleAlignment = TitleAlignments.Centered)]
public abstract class UnitBase : MonoBehaviour, IUnit, ISelectable, IDamagable
{
    /*
     *  UnitBase - MonoBehaviour class that defines base logic 
     *  of in-game unit behaviour. 
     *  
     *  Works with user input, NavMesh Agent component, defines HP, FOW and attack logic
     *  
     *  TODO: 
     *  1. Shooting scripts: attach to weapon settings
     *  
     *  Added in game by KeyboardDestroyer on 15.09.2023 16:30
     *  
     *  Modifies:
     *  Global edit: 20.09.23 by kbrddestroyer: Watch git commit
     *  
     *  REVIEW THIS PLS
     */

    #region EDITOR_VARIABLES
    [Header("Base Unit Settings")]
    [SerializeField, Range(0f, 100f), LabelText("HP Maximum value")]   private float fMaxHp;
    [SerializeField, Range(0f, 100f), LabelText("FOW Cutoff Distance")]   private float fFowCutoffDistance;
    [SerializeField, Range(0f, 120f), LabelText("Ragdoll lifetime (sec.)")]   private float fRagdollLifetime;
    [Tooltip("Units are using NavMeshAgent and sometimes stuck walking in each other. This parameter always keeps some space between them to prevent this")]
    [SerializeField, Range(0f, 10f), LabelText("Unit Spacing")] private float fMinUnitDistance;
    [SerializeField, Range(0, 10), LabelText("AI Weight")]    private int iWeight;
    [Header("Team Settings")]
    [SerializeField] protected Teams team;
    [SerializeField, SceneObjectsOnly] protected PlayerController parent;
    [Header("Selection Tool")]
    [SerializeField, ChildGameObjectsOnly, AllowNull] protected GameObject selectIcon;
    [InfoBox("Replace this with something. This is incorrect way", InfoMessageType = InfoMessageType.Warning)]
    [SerializeField, LabelText("Can be selected in-game?")] private bool bCanBeSelectedByOnScreenSelector;  // parent.hasAuthority
    [Header("GUI Tools")]
    [SerializeField, AllowNull] private UnitLogoController unitLogo;
    [SerializeField] private Canvas gui;
    [SerializeField] private string unitName;
    [Header("Gizmos (Editor Only)")]
    [SerializeField, ColorUsage(false)] private Color cGizmoColorFOW;
    #endregion

    #region PROTECTED
    // Components
    [Header("Requirements (Fill with button below)")]
    [SerializeField, LabelText("Collider reference")] protected Collider col;
    [SerializeField, LabelText("NavMesh reference")] protected NavMeshAgent agent;
    [SerializeField, LabelText("Main Camera")] protected Camera mainCamera;
    [SerializeField] protected Animator animator;
    [SerializeField] protected UnitLogoController unitLogoController;
    [SerializeField, ChildGameObjectsOnly] protected GameObject mesh;

    protected float   fHp;
    protected bool    bMouseOver = false;
    protected bool    bSelected = false;
    #endregion

    #region GETTER_SETTER
    public Teams Team { get => team; }

    public float HP { 
        get => fHp; 
        set
        {
            fHp = value;
            if (fHp <= 0) this.enabled = false;
        } 
    }

    public bool Selected { 
        get => bSelected; 
        set => bSelected = value; 
    }

    public Vector3 WorldPosition { get => transform.position; }

    public PlayerController Parent
    {
        get => parent;
        set
        {
            parent = value;
        }
    }

    public int Weight
    {
        get => iWeight;
    }

    public UnitLogoController UnitLogoController
    {
        get => unitLogoController;
        set => unitLogoController = value;
    }

    public string UnitName { get => unitName; }
    #endregion

    #region INTERFACES

    #region ISELECTABLE_IMPLEMENTED

    // ISelectable interface is used for on-screen selection

    public virtual void Register()
    {
        // Register function should be called on Awake/Start method
        // It's used for object registration in selectable objects list
        // on Selection.cs

        // Sometimes object cannot be selected
        // e.g. enemy unit cannot be selected by the player

        if (bCanBeSelectedByOnScreenSelector)
            Selection.RegisterNewSelectable(this);
    }

    public virtual void ToggleSelection(bool state)
    {
        // Toggle selection method is called in Selection.cs when selection box overlaps this object root point

        bSelected = state;
        selectIcon.SetActive(state);
    }
    #endregion

    #region IUNIT_IMPLEMENTED

    // IUnit implemented
    public abstract void OnDeath(); // OnDeath method is called when HP is less or eq. 0 OR when this script is being disabled

    #endregion
    #endregion

    #region LIFECYCLE

    // MonoBehaviour lifecycle
    // Any of lifecycle methods can be overwritten

    protected virtual void ToggleFOW(bool state)
    {
        mesh.SetActive(state);
        unitLogoController.gameObject.SetActive(state);
    }

    protected virtual void Awake()
    {
        fHp = fMaxHp;
        Register();

        if (agent == null)
            Debug.LogWarning($"Warning! {name} unit has no NavMesh agent!");

        if (unitLogo != null)
        {
            unitLogoController = Instantiate(unitLogo.gameObject, gui.transform).GetComponent<UnitLogoController>();
            unitLogoController.ControllerObject = this;
            unitLogoController.setName(unitName);
        }
    }


    protected virtual void OnMouseEnter()
    {
        bMouseOver = true;
    }

    protected virtual void OnMouseExit()
    {
        bMouseOver = false;
    }

    protected virtual void Update()
    {
        if (bCanBeSelectedByOnScreenSelector && parent != null && !parent.AIControlled)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ToggleSelection(bMouseOver);
            }
            if (Selected)
            {
                // Movement

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    // Compute onTerrain position;

                    LayerMask mask = LayerMask.GetMask("Ground");

                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, mask))
                    {
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
        if (parent.AIControlled)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, fFowCutoffDistance, LayerMask.GetMask("Unit"));
            bool _enable = false;
            
            foreach (Collider col in colliders)
            {
                UnitBase baseUnitController = col.GetComponent<UnitBase>();
                if (baseUnitController && parent.Team != baseUnitController.Team)
                {
                    _enable = true;
                    break;
                }
            }
            ToggleFOW(_enable);
        }
        if (agent != null && animator != null) animator.SetFloat("speed", agent.velocity.magnitude);
    }

    private void LateUpdate()
    {
        if (agent && agent.velocity.magnitude == 0)
        {
            LayerMask mask = LayerMask.GetMask("Unit");
            Collider[] colliders = Physics.OverlapSphere(transform.position, fMinUnitDistance, mask);
            Vector3 direction = Vector3.zero;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != this.gameObject)
                {
                    direction += transform.position - collider.transform.position;
                }
            }
            if (agent)
            {
                if (direction != Vector3.zero) agent.ResetPath();
                if (animator) animator.SetFloat("speed", direction.normalized.magnitude * agent.speed);
                transform.position += direction.normalized * agent.speed * Time.deltaTime;
            }
        }
    }

    protected virtual void OnDisable()
    {
        mesh.SetActive(true);
        OnDeath();
        if (unitLogoController) Destroy(unitLogoController.gameObject);
        if (GetComponent<AIUnitController>()) Destroy(GetComponent<AIUnitController>());
        Selection.RemoveSelectable(this);
        agent.isStopped = true;
        
        Destroy(this.gameObject, fRagdollLifetime);
    }
    #endregion

    #region UNITY_EDITOR
#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = cGizmoColorFOW;

        Gizmos.DrawWireSphere(transform.position, fFowCutoffDistance);
        Gizmos.DrawWireSphere(transform.position, fMinUnitDistance);
    }

    [Button("Fill")]
    protected virtual void FillRequired()
    {
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        gui = FindObjectOfType<CameraController>().MainCanvas;
    }
#endif
    #endregion
}
