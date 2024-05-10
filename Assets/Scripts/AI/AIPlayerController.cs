using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class AIPlayerController : PlayerController
{
    private Stack<UnitBase> toSpawn = new Stack<UnitBase>();
    [SerializeField] private bool testMyself;
    [SerializeField] private PlayerController[] enemies;

    private int selectedID = 0;

    public override void Awake()
    {
        base.Awake();
        isControllerByAI = true;
    }

    private UnitBase getUnitBase()
    {
        // TODO: Finish algo

        float totalPower = 0f;
        foreach (PlayerController enemy in enemies) 
            foreach (UnitBase unit in enemy.ControlledUnits)
            {
                totalPower += (unit.Armor + unit.HP) * unit.Weight;
            }

        foreach (UnitBase unit in ControlledUnits)
        {
            totalPower -= (unit.Armor + unit.HP) * unit.Weight;
        }

        foreach (UnitStructureInformation unit in units)
        {
            if (unit.team == team)
            {
                // Get resource delta
            }
        }

        return null;
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
        if (testMyself) {
            if (SpawnSelectedUnit(units[selectedID].unit))
            {
                selectedID = Random.Range(0, units.Count);
            }
        }
    }
}
