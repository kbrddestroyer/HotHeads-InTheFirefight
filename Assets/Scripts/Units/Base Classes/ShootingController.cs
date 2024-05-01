using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootingController : MonoBehaviour, IShooting, IUnit
{
    /*
     *  Shooting Controller is used in every GameObject that can shoot (emit bullets)
     *  |
     *  |   By:         Keyboard Destroyer
     *  |   Created:    20.01.2024 14:00
     *  TODO:
     *  - Finish class
     *  - Refactor to use this in Supply logic instead of ShootingUnitBase
     */
    [Title("Shooting logic controller, used for shooting units, turrets, etc.")]
    #region EDITOR_VARIABLES
    [Header("Shooting Unit Logic")]
    [SerializeField, Range(0f, 10f), LabelText("Fire rate")] protected float fShootingRate;
    [SerializeField, Range(0f, 10f), LabelText("Reloading time")] protected float fReloadDelay;
    [SerializeField, Range(0f, 100f), LabelText("Base damage")] protected float fBaseDamage;
    [SerializeField, Range(0f, 100f), LabelText("Armor damage")] protected float fArmorDamage;
    [SerializeField, Range(0, 120), LabelText("Maximum ammo in mag")] protected int iAmmoInWeapon;
    [SerializeField, Range(0f, 100f), LabelText("Attack distance")] protected float fAttackDistance;
    [SerializeField] private UnitType attackType;
    [Header("Required")]
    [SerializeField, AssetsOnly] protected Bullet bulletPrefab;
    [SerializeField, ChildGameObjectsOnly] protected Transform bulletSpawnPoint;
    [SerializeField] protected AudioSource source;
    [SerializeField] protected Animator animator;
    [SerializeField, AllowNull] protected ParticleSystem muzzle;
    [SerializeField, LabelText("Unit Layer")] protected LayerMask mask;
    [Header("Gizmos Settings")]
    [SerializeField, ColorUsage(false)] protected Color cGizmoColorAttackDistance = new Color(0, 0, 0, 1);
    #endregion


    #region PROTECTED_VARIABLES
    protected int iCurrentMagAmmo = 0;
    protected int iAmmoTotal = 0;
    protected float fTimePassed = 0f;
    protected bool bFpsMode = false;
    private bool isInReloadingState = false;
    public int AmmoTotal { get => iAmmoTotal; set => iAmmoTotal = value; }

    public float AttackDistance { get => fAttackDistance; }

    public bool FpsMode { get => bFpsMode; set => bFpsMode = value; }

    public float ShootRate { get => fShootingRate; }
    #endregion

    #region INTERFACES

    #region ISHOOTING_IMPLEMENTED
    public virtual void Attack(Transform target, Teams team)
    {
        if (bFpsMode) return;

        if (animator != null && animator.GetBool("shooting") != true)
            animator.SetBool("shooting", true);
        fTimePassed += Time.deltaTime;
        transform.LookAt(target);
        if (fTimePassed >= fShootingRate)
        {
            Bullet _bullet = ShootProjectile(ref fTimePassed, team);
            _bullet.transform.LookAt(target.position + Vector3.up * 0.5f);
        }
    }

    public Bullet ShootProjectile(ref float fTimePassed, Teams team)
    {
        // Exposed to FPS Controller
        if (fTimePassed < fShootingRate) return null;
        if (iAmmoTotal > 0 && iCurrentMagAmmo <= 0)
        {
            if (!isInReloadingState)
                StartCoroutine(AsyncReload());
            return null;
        }
        else if (iAmmoTotal <= 0) return null;
        source.PlayOneShot(source.clip);
        fTimePassed = 0f;
        iCurrentMagAmmo--;
        Bullet _bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        _bullet.Owner = team;
        _bullet.GetComponent<Bullet>().BaseDamage = fBaseDamage;
        _bullet.GetComponent<Bullet>().ArmorDamage = fArmorDamage;

        if (muzzle) muzzle.Play(muzzle);

        return _bullet;
    }

    private IEnumerator AsyncReload()
    {
        isInReloadingState = true;
        if (animator && !animator.GetBool("reload")) animator.SetBool("reload", true);
        yield return new WaitForSeconds(fReloadDelay);
        iCurrentMagAmmo = Mathf.Clamp(iAmmoTotal, 0, iAmmoInWeapon);
        iAmmoTotal -= iCurrentMagAmmo;
        if (animator) animator.SetBool("reload", false);
        isInReloadingState = false;
    }

    private void Reload()
    {
        isInReloadingState = true;
        if (animator && !animator.GetBool("reload")) animator.SetBool("reload", true);
        fTimePassed += Time.deltaTime;
        if (fTimePassed >= fReloadDelay)
        {
            fTimePassed = 0;

            iCurrentMagAmmo = Mathf.Clamp(iAmmoTotal, 0, iAmmoInWeapon);
            iAmmoTotal -= iCurrentMagAmmo;
            if (animator) animator.SetBool("reload", false);
            isInReloadingState = false;
        }
    }

    public virtual void ShootingBaseLogic(Teams team)
    {
        if (bFpsMode) return;
        if (iCurrentMagAmmo > 0)
        {
            Transform closestEnemy = FindClosest(team);

            if (closestEnemy != null)
            {
                Attack(closestEnemy, team);
            }
            else
            {
                if (animator != null && animator.GetBool("shooting") != false)
                    animator.SetBool("shooting", false);
            }
        }
        else if (iAmmoTotal > 0)
        {
            Reload();
        }
        else
        {
            if (animator && animator.GetBool("shooting")) animator.SetBool("shooting", false);
        }
    }
    #endregion

    #endregion

    protected void Start()
    {
        iCurrentMagAmmo = iAmmoInWeapon;
        iAmmoTotal = iAmmoInWeapon * 2;
    }

    #region LOGIC
    protected virtual Transform FindClosest(Teams team)
    {
        /*
         *      The algo could be optimised by using general (may be even static) list of enemies 
         *      thst updates w/ spawning new unit in it's Start/Awake method
         *      Cycle for each unit in enemy list and check if 
         */
        if (bFpsMode) return null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, fAttackDistance, mask);

        if (colliders.Length == 0) return null; // If noone's in range

        Transform closestEnemy = null;
        float fDistanceToClosest = 0f;
        foreach (Collider _collider in colliders)
        {
            UnitBase unitBase = _collider.GetComponent<UnitBase>();          
            if (unitBase != null && unitBase.Team != team && unitBase.enabled && unitBase.Type == attackType)
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
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = cGizmoColorAttackDistance;
        Gizmos.DrawWireSphere(transform.position, fAttackDistance);
    }
#endif
    #endregion
}
