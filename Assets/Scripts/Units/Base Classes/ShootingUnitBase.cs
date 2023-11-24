using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingUnitBase : UnitBase, IUnit, ISelectable, IDamagable, IShooting
{
    #region EDITOR_VARIABLES
    [Header("Shooting Unit Logic")]
    [SerializeField, Range(0f, 10f)] private float fShootingRate;
    [SerializeField, Range(0f, 10f)] private float fReloadDelay;
    [SerializeField, Range(0f, 100f)] private float fBaseDamage;
    [SerializeField, Range(0, 120)] private int iAmmoInWeapon;
    [SerializeField, Range(0f, 100f)] private float fAttackDistance;
    [Header("Required Prefabs")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [Header("Gizmos Settings")]
    [SerializeField] private Color cGizmoColorAttackDistance;
    #endregion

    #region PROTECTED_VARIABLES
    protected int iCurrentMagAmmo = 0;
    [SerializeField] protected int iAmmoTotal = 0;
    protected float fTimePassed = 0f;

    public int AmmoTotal { get => iAmmoTotal; set => iAmmoTotal = value; }

    public float AttackDistance { get => fAttackDistance; }

    protected AudioSource source;
    #endregion

    #region INTERFACES

    #region ISHOOTING_IMPLEMENTED
    public virtual void Attack(Transform target)
    {
        if (agent.velocity.magnitude != 0) { return; }
        fTimePassed += Time.deltaTime;

        transform.LookAt(target.position + Vector3.up * 0.5f);

        if (fTimePassed >= fShootingRate)
        {
            source.Play();
            fTimePassed = 0;
            iCurrentMagAmmo--;
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>().BaseDamage = fBaseDamage;
        }
    }

    public virtual void ShootingBaseLogic()
    {
        if (iCurrentMagAmmo > 0)
        {
            Transform closestEnemy = FindClosest();
            if (animator != null && animator.GetBool("shooting") != (closestEnemy != null)) animator.SetBool("shooting", (closestEnemy != null));
            if (closestEnemy != null)
            {
                Attack(closestEnemy);
            }
        }
        else if (iAmmoTotal > 0)
        {
            if (animator && !animator.GetBool("reload")) animator.SetBool("reload", true);
            fTimePassed += Time.deltaTime;
            if (fTimePassed >= fReloadDelay)
            {
                // Reload

                fTimePassed = 0;

                iCurrentMagAmmo = Mathf.Clamp(iAmmoTotal, 0, iAmmoInWeapon);
                Debug.Log("Reloaded!");
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

    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();

        iCurrentMagAmmo = iAmmoInWeapon;
        iAmmoTotal = iAmmoInWeapon * 2;
    }

    protected override void Update()
    {
        base.Update();
        ShootingBaseLogic();
    }

    #endregion

    #region LOGIC
    private Transform FindClosest()
    {
        LayerMask mask = LayerMask.GetMask("Unit");
        Collider[] colliders = Physics.OverlapSphere(transform.position, fAttackDistance, mask);

        if (colliders.Length == 0) return null;
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
#endif
    #endregion
}
