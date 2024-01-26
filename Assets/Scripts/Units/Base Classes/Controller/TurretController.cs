using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : ShootingController
{
    [Header("Turret Logic")]
    [SerializeField, ChildGameObjectsOnly] private TurretIK turretController;

    public override void Attack(Transform target, Teams team)
    {
        if (animator != null && animator.GetBool("shooting") != true)
            animator.SetBool("shooting", true);
        fTimePassed += Time.deltaTime;

        if (fTimePassed >= fShootingRate)
        {
            source.Play();
            fTimePassed = 0;
            iCurrentMagAmmo--;
            Bullet _bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            _bullet.Owner = team;
            _bullet.GetComponent<Bullet>().BaseDamage = fBaseDamage;
            _bullet.transform.LookAt(target.position + Vector3.up * 0.5f);
        }
    }

    protected override Transform FindClosest(Teams team)
    {
        Transform closest = base.FindClosest(team);
        turretController.Target = closest;
        return closest;
    }
}
