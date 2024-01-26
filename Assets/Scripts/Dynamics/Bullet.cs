using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Base Bullet Settings")]
    [SerializeField, Range(0f, 100f)] private float fSpeed;
    [SerializeField, Range(0f, 10f)] private float fLifetime;

    private Teams ownerTeam;
    public Teams Owner { get => ownerTeam; set => ownerTeam = value; }

    private float fBaseDamage = 0f;
    public float BaseDamage { set => fBaseDamage = value; }
    private float fLifetimeCurrent = 0f;

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (damagable != null && damagable.Team != Owner)
        {
            damagable.HP -= fBaseDamage;

            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        transform.position += transform.forward * fSpeed * Time.deltaTime;
        fLifetimeCurrent += Time.deltaTime;
        if (fLifetimeCurrent >= fLifetime)
        {
            Destroy(this.gameObject);
        }
    }
}
