using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public float HP { get; set; }

    public void OnDeath();
}
