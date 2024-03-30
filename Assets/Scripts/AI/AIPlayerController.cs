using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : PlayerController
{
    private Stack<UnitBase> toSpawn = new Stack<UnitBase>();

    public override void Awake()
    {
        base.Awake();
        isControllerByAI = true;
    }

    public override void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= deltaTime)
        {
            timePassed = 0f;

            foreach (GameResourceStructure _res in resourcesCount)
            {
                _res.Amount += _res.Appliance;
            }
        }

        GameResourceStructure manpwr = getResource(GameResources.MANPOWER);
        if (manpwr.Amount >= 30)
        {
            SpawnSelectedUnit(units[0].unit);
            manpwr.Amount -= 30;
        }
    }
}
