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
     *  Works with user input, NavMesh Agent component and defines HP
     *  
     *  TODO: 
     *  1. Shooting scripts: attach to weapon settings
     *  2. Delete all FOW related code from here and migrate to FOWController class
     *  
     *  
     *  Added in game by KeyboardDestroyer on 15.09.2023 16:30
     *  
     *  Modifies:
     *  Global edit: 20.09.23 by kbrddestroyer: Watch git commit
     *  Global edit: 23.01.24 by kbrddestroyer: Removing all FOW-related code and migrating it to separated FOWController
     *  
     *  
     */

    #region EDITOR_VARIABLES
    [Header("Base Unit Settings")]
    [SerializeField] private UnitType type;
    [SerializeField, Range(0f, 100f), LabelText("HP Maximum value")]   private float fMaxHp;
    [SerializeField, Range(0f, 100f), LabelText("Armor Maximum value")]   private float fMaxArmor;
    [SerializeField, Range(0f, 120f), LabelText("Ragdoll lifetime (sec.)")]   private float fRagdollLifetime;
    [Tooltip("Units are using NavMeshAgent and sometimes stuck walking in each other. This parameter always keeps some space between them to prevent this")]
    [SerializeField, Range(0f, 10f), LabelText("Unit Spacing")] private float fMinUnitDistance;
    [SerializeField, Range(0, 10), LabelText("AI Weight")] private int iWeight;
    [Header("Team Settings")]
    [SerializeField] protected Teams team;
    [SerializeField] protected LayerMask unitLayer;
    [SerializeField, SceneObjectsOnly] protected PlayerController parent;
    [Header("Selection Tool")]
    [SerializeField, ChildGameObjectsOnly, AllowNull] protected GameObject selectIcon;
    [InfoBox("Replace this with something. This is incorrect way", InfoMessageType = InfoMessageType.Warning)]
    [SerializeField, LabelText("Can be selected in-game?")] private bool bCanBeSelectedByOnScreenSelector;          // parent.hasAuthority
    [Header("GUI Tools")]
    [SerializeField, AllowNull, AssetsOnly, AssetSelector] private UnitLogoController unitLogo;
    [SerializeField] protected Canvas gui;
    [SerializeField] private string unitName;
    [SerializeField] private Color minDistanceColor = new Color(0, 0, 0, 1f);
    #endregion

    #region PROTECTED
    // Components
    [Header("Requirements (Fill with button below)")]
    [SerializeField, LabelText("Collider reference")] protected Collider col;
    [SerializeField, LabelText("NavMesh reference")] protected NavMeshAgent agent;
    [SerializeField, LabelText("Main Camera")] protected Camera mainCamera;
    [SerializeField] protected Animator animator;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameResourceStructure[] cost;
    [SerializeField, AllowNull] protected FPS_ModeController fps;
    public GameResourceStructure[] Cost { get => cost; }

    protected UnitLogoController unitLogoController;

    protected float   fHp;
    protected float   fArmor;
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

    public float Armor
    {
        get => fArmor;
        set => fArmor = (value > 0) ? value : 0;
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

    public UnitType Type { get => type; }

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

    protected virtual void Awake()
    {
        fHp = fMaxHp;
        mainCamera = Camera.main;
        gui = FindObjectOfType<CameraController>().MainCanvas;

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

    private void MoveToClickPoint(Vector3 mouseClickPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseClickPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, groundMask))
        {
            agent.SetDestination(hit.point);
        }
    }

    protected virtual void Update()
    {
        if (bCanBeSelectedByOnScreenSelector && parent != null && !parent.AIControlled && (fps == null || !fps.enabled))
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

                    MoveToClickPoint(Input.mousePosition);
                }
            }
        }
        
        if (bSelected && !Selection.Instance.IsMulti && fps && Input.GetKeyDown(KeyCode.F))
        {
            fps.enabled = true;
        }

        if (agent != null && agent.enabled && animator != null) animator.SetFloat("speed", agent.velocity.magnitude);
    }

    private void LateUpdate()
    {
        /*
         *  Code to make space between friendly units. Standing units are trying to "flee"
         *  from moving units to give them some space
         */
        if (agent && agent.velocity.magnitude == 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, fMinUnitDistance, unitLayer);
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
        OnDeath();
        if (unitLogoController) Destroy(unitLogoController.gameObject);
        if (GetComponent<AIUnitController>()) Destroy(GetComponent<AIUnitController>());
        if (GetComponent<FOWController>()) GetComponent<FOWController>().enabled = false;
        Selection.RemoveSelectable(this);
        agent.isStopped = true;
        
        Destroy(this.gameObject, fRagdollLifetime);
    }
    #endregion

    #region UNITY_EDITOR
#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = minDistanceColor;

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
        fps = GetComponent<FPS_ModeController>();
        unitLayer = LayerMask.GetMask("Unit");
        groundMask = LayerMask.GetMask("Ground");
    }
#endif
    #endregion
}
