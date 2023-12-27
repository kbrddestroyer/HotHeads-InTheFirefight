using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingUnitBase : UnitBase, IUnit, ISelectable, IDamagable, IShooting
{
    /*
     *  ShootingUnitBase - general class for shooting units. Children can override general methods to create unique
     *  shooting logic (e.g. anti-tank infantry, CIWS/CRAM, airforces, etc.)
     *  
     *  Seek-and-destroy algorythm:
     *  1. ShootingBaseLogic [VIRTUAL]
     *  | 1.1. FindClosest
     *  
     * 
     */

    [Space]
    [Title("ShootingUnitBase Settings", "Settings for shooting units, such as fire rate, base damage, etc.", horizontalLine: true, bold: true, TitleAlignment = TitleAlignments.Centered)]

    #region EDITOR_VARIABLES
    [Header("Shooting Unit Logic")] 
    [SerializeField, Range(0f, 10f), LabelText("Fire rate")] private float fShootingRate;
    [SerializeField, Range(0f, 10f), LabelText("Reloading time")] private float fReloadDelay;
    [SerializeField, Range(0f, 100f), LabelText("Base damage")] private float fBaseDamage;
    [SerializeField, Range(0, 120), LabelText("Maximum ammo in mag")] private int iAmmoInWeapon;
    [SerializeField, Range(0f, 100f), LabelText("Attack distance")] private float fAttackDistance;
    [Header("Required")]
    [SerializeField, AssetsOnly] private Bullet bulletPrefab;
    [SerializeField, ChildGameObjectsOnly] private Transform bulletSpawnPoint;
    [SerializeField] protected AudioSource source;
    [SerializeField, LabelText("Unit Layer")] protected LayerMask mask;
    [Header("Gizmos Settings")]
    [SerializeField, ColorUsage(false)] private Color cGizmoColorAttackDistance;
    #endregion

    #region PROTECTED_VARIABLES
    protected int iCurrentMagAmmo = 0;
    protected int iAmmoTotal = 0;
    protected float fTimePassed = 0f;

    public int AmmoTotal { get => iAmmoTotal; set => iAmmoTotal = value; }

    public float AttackDistance { get => fAttackDistance; }
    #endregion

    #region INTERFACES

    #region ISHOOTING_IMPLEMENTED
    public virtual void Attack(Transform target)
    {
        if (agent.velocity.magnitude != 0) { return; }
        if (animator != null && animator.GetBool("shooting") != true)
            animator.SetBool("shooting", true);
        fTimePassed += Time.deltaTime;

        transform.LookAt(target.position + Vector3.up * 0.5f);

        if (fTimePassed >= fShootingRate)
        {
            source.Play();
            fTimePassed = 0;
            iCurrentMagAmmo--;
            Bullet _bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            _bullet.GetComponent<Bullet>().BaseDamage = fBaseDamage;
            _bullet.transform.LookAt(target.position + Vector3.up * 0.5f);
        }
    }

    public virtual void ShootingBaseLogic()
    {
        if (iCurrentMagAmmo > 0)
        {
            Transform closestEnemy = FindClosest();

            if (closestEnemy != null)
            {
                Attack(closestEnemy);
            }
            else
            {
                if (animator != null && animator.GetBool("shooting") != false)
                    animator.SetBool("shooting", false);
            }
        }
        else if (iAmmoTotal > 0)
        {
            if (animator && !animator.GetBool("reload")) animator.SetBool("reload", true);
            fTimePassed += Time.deltaTime;
            if (fTimePassed >= fReloadDelay)
            {
                fTimePassed = 0;

                iCurrentMagAmmo = Mathf.Clamp(iAmmoTotal, 0, iAmmoInWeapon);
                iAmmoTotal -= iCurrentMagAmmo;
                if (animator) animator.SetBool("reload", false);
            }
        }
        else
        {
            if (animator && animator.GetBool("shooting")) animator.SetBool("shooting", false);
        }
    }
    #endregion

    #endregion

    #region UNIT_BASE_EXTENDED

    protected void Start()
    {
        iCurrentMagAmmo = iAmmoInWeapon;
        iAmmoTotal = iAmmoInWeapon * 2;
    }

    protected override void Update()
    {
        // Lifetime

        base.Update();
        ShootingBaseLogic();
    }

    #endregion

    #region LOGIC
    private Transform FindClosest()
    {
        /*
         *      The algo could be optimised by using general (may be even static) list of enemies 
         *      thst updates w/ spawning new unit in it's Start/Awake method
         *      Cycle for each unit in enemy list and check if 
         */

        Collider[] colliders = Physics.OverlapSphere(transform.position, fAttackDistance, mask);

        if (colliders.Length == 0) return null; // If noone's in range

        Transform closestEnemy = null;
        float fDistanceToClosest = 0f;
        foreach (Collider _collider in colliders)
        {
            UnitBase unitBase = _collider.GetComponent<UnitBase>();
            if (unitBase != null && unitBase.Team != this.team)
            {
                float fDistance = Vector3.Distance(transform.position, _collider.transform.position);
                if (
                    closestEnemy == null ||
                    fDistance < fDistanceToClosest
                    )
                {
                    closestEnemy = _collider.transform;
                    fDistanceToClosest = fDistance;
                }
            }
        }

        return closestEnemy;
    }
    #endregion

    #region UNITY_EDITOR
#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = cGizmoColorAttackDistance;
        Gizmos.DrawWireSphere(transform.position, fAttackDistance);
    }

    protected override void FillRequired()
    {
        base.FillRequired();
        source = GetComponent<AudioSource>();
    }
#endif
    #endregion
}
