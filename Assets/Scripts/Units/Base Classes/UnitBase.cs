using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
     */

    [Header("Base Unit Settings")]
    [SerializeField, Range(0f, 100f)]   private float fMaxHp;
    [SerializeField, Range(0f, 100f)]   private float fFowCutoffDistance;
    [SerializeField, Range(0f, 100f)]   private float fAttackDistance;
    [SerializeField, Range(0f, 120f)]   private float fRagdollLifetime;
    [SerializeField, Range(0f, 10f)]    private float fShootingRate;
    [SerializeField, Range(0f, 10f)]    private float fReloadDelay;
    [SerializeField, Range(0f, 100f)]   private float fBaseDamage;
    [SerializeField, Range(0f, 10f)]    private float fMinUnitDistance;
    [SerializeField, Range(0, 120)]     private int iAmmoInWeapon;
    [Header("Required Components")]
    [SerializeField] private Teams team;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [Header("Selection Tool")]
    [SerializeField, AllowNull] protected GameObject selectIcon;
    [SerializeField]            private bool bCanBeSelectedByOnScreenSelector;
    [Header("Gizmos (Editor Only)")]
    [SerializeField] private Color cGizmoColorFOW;
    [SerializeField] private Color cGizmoColorAttackDistance;

    private int iCurrentMagAmmo = 0;
    private int iAmmoTotal = 0;
    private float attackTimeDelay = 0f;
    private bool bMouseOver = false;
    public Teams Team { get => team; }

    [SerializeField] private float fHp;

    public float HP { 
        get => fHp; 
        set
        {
            fHp = value;
            if (fHp <= 0) this.enabled = false;
        } 
    }

    protected bool bSelected;
    public bool Selected { 
        get => bSelected; 
        set => bSelected = value; 
    }

    public Vector3 WorldPosition { get => transform.position; }

    // Components
    protected Rigidbody rb;
    protected Collider col;
    protected NavMeshAgent agent;
    protected Camera mainCamera;
    protected Animator animator;

    public virtual void Register()
    {
        if (bCanBeSelectedByOnScreenSelector)
            Selection.RegisterNewSelectable(this);
    }

    public virtual void ToggleSelection(bool state)
    {
        bSelected = state;
        selectIcon.SetActive(state);
    }

    // IUnit implemented
    public abstract void OnDeath();

    // MonoBehaviour lifecycle

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        fHp = fMaxHp;
        Register();

        if (agent == null)
            Debug.LogWarning($"Warning! {name} unit has no NavMesh agent!");

        iCurrentMagAmmo = iAmmoInWeapon;
        iAmmoTotal = iAmmoInWeapon * 2;
    }

    protected virtual void Attack(Transform target)
    {
        attackTimeDelay += Time.deltaTime;

        transform.LookAt(target.position + Vector3.up * 0.5f);

        if (attackTimeDelay >= fShootingRate)
        {
            attackTimeDelay = 0;
            iCurrentMagAmmo--;
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>().BaseDamage = fBaseDamage;
        }
        
    }

    protected virtual void ShootingBase()
    {
        if (iCurrentMagAmmo > 0)
        {
            LayerMask mask = LayerMask.GetMask("Unit");
            Collider[] colliders = Physics.OverlapSphere(transform.position, fAttackDistance, mask);

            if (colliders.Length == 0) return;
            Transform closestEnemy = null;

            foreach (Collider _collider in colliders)
            {
                UnitBase unitBase = _collider.GetComponent<UnitBase>();
                if (unitBase != null && unitBase.team != this.team)
                {
                    if (
                        closestEnemy == null ||
                        Vector3.Distance(transform.position, _collider.transform.position) < Vector3.Distance(transform.position, closestEnemy.position)
                        )
                    {
                        closestEnemy = _collider.transform;
                    }
                }
            }
            if (animator != null) animator.SetBool("shooting", (closestEnemy != null));
            if (closestEnemy != null)
            {
                Attack(closestEnemy);
            }
        }
        else if (iAmmoTotal > 0)
        {
            animator.SetBool("reload", true);
            attackTimeDelay += Time.deltaTime;
            if (attackTimeDelay >= fReloadDelay)
            {
                // Reload

                attackTimeDelay = 0;

                iCurrentMagAmmo = Mathf.Clamp(iAmmoTotal, 0, iAmmoInWeapon);
                Debug.Log("Reloaded!");
                iAmmoTotal -= iCurrentMagAmmo;
                animator.SetBool("reload", false);
            }
        }
        else
        {
            animator.SetBool("shooting", false);
        }
    }

    private void OnMouseEnter()
    {
        bMouseOver = true;
    }

    private void OnMouseExit()
    {
        bMouseOver = false;
    }

    protected virtual void Update()
    {
        if (bCanBeSelectedByOnScreenSelector)
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
            ShootingBase();
            if (agent != null) animator.SetFloat("speed", agent.velocity.magnitude);
        }
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
                animator.SetFloat("speed", direction.normalized.magnitude * agent.speed);
                transform.position += direction.normalized * agent.speed * Time.deltaTime;
            }
        }
    }

    protected virtual void OnDisable()
    {
        OnDeath();
        Destroy(this.gameObject, fRagdollLifetime);
    }

    // Editor GUI

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = cGizmoColorFOW;

        Gizmos.DrawWireSphere(transform.position, fFowCutoffDistance);
        Gizmos.color = cGizmoColorAttackDistance;
        Gizmos.DrawWireSphere(transform.position, fAttackDistance);
        Gizmos.DrawWireSphere(transform.position, fMinUnitDistance);
    }
#endif
}
