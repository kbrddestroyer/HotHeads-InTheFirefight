using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IShooting
{
    public void Attack(Transform target, Teams team);
    public void ShootingBaseLogic(Teams team);
}
