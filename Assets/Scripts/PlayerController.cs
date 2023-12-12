using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Unit info")]
    [SerializeField] private GlobalUnitList allUnits;
    [SerializeField] private Teams team;
    [SerializeField] private bool isControllerByAI;
    [SerializeField, Range(0f, 10f)] private float deltaTime;
    [SerializeField] private GameObject explosion;

    [SerializeField, AllowNull] private TMP_Dropdown menu;

    private float timePassed;
    private LayerMask ground;

    private List<UnitStructureInformation> units;

    public Teams Team { get => team; }

    public bool AIControlled
    {
        get => isControllerByAI;
    }

    [SerializeField] private GameResourceStructure[] resourcesCount;

    public void addResourceAppliance(GameResources resource, float appliance)
    {
        foreach (GameResourceStructure res in resourcesCount)
        {
            if (res.Type == resource)
            {
                res.Appliance += appliance;
                break;
            }
        }
    }

    public GameResourceStructure getResource(GameResources resource)
    {
        foreach (GameResourceStructure _resource in resourcesCount)
        {
            if (_resource.Type == resource)
            {
                return _resource;
            }
        }
        return null;
    }

    public void SpawnUnitFromDropdown()
    {
        UnitBase unit = menu.options[menu.value].ConvertTo<UnitStructureInformation>().unit;
        SpawnSelectedUnit(unit);
    }

    private void SpawnSelectedUnit(UnitBase unitPrefab)
    {
        unitPrefab.Parent = this;
        UnitBase unit = Instantiate(unitPrefab, transform.position, transform.rotation);
        unit.Parent = this;
    }

    public void Awake()
    {
        units = allUnits.Units.ToList<UnitStructureInformation>();
        units.RemoveAll(u => u.team != Team);
        if (menu)
        {
            menu.options.Clear();
            menu.options.AddRange(units);
        }
        foreach (GameResourceStructure _res in resourcesCount)
        {
            _res.Appliance = _res.baseAppliance;
        }

        ground = LayerMask.GetMask("Ground");
    }

    public void Update()
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

        if (isControllerByAI)
        {
            GameResourceStructure manpwr = getResource(GameResources.MANPOWER);
            if (manpwr.Amount >= 30)
            {
                SpawnSelectedUnit(units[0].unit);
                manpwr.Amount -= 30;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, ground))
                {
                    Instantiate(explosion, hit.point, Quaternion.identity);
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UnitLogoController.Enabled = !UnitLogoController.Enabled;
            }
        }
    }
}
