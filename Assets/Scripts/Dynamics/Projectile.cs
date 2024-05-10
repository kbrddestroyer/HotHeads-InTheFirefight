using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : Bullet
{
    /*
     *  Projectiles are used to imitate tank shells, RPG shots, ATGMs, STAMs, etc.
     *  Anything, that should have an impact effect should be treated as projectile
     */

    [SerializeField] private GameObject impactFX;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        CreateImpact();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpact();
    }

    private void CreateImpact() 
    {
        Instantiate(impactFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
