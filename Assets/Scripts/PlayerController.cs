using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Global Unit List Scriptable Object")]
    [SerializeField] protected GlobalUnitList allUnits;
    [Header("Unit info")]
    [SerializeField] protected Teams team;
    [SerializeField] protected bool isControllerByAI;
    [SerializeField] protected bool localPlayer;
    [SerializeField, Range(0f, 10f)] protected float deltaTime;
    [SerializeField] protected GameObject explosion;
    [SerializeField] private LayerMask ground;
    [SerializeField] private List<UnitBase> controlledUnits;
    [SerializeField, AllowNull] protected TMP_Dropdown menu;

    private Firepower firepower;
    public int Infantry
    {
        get => firepower.Infantry;
    }

    public int Mechanised
    {
        get => firepower.Mechanised;
    }

    protected float timePassed;

    protected List<UnitStructureInformation> units;

    public Teams Team { get => team; }

    public bool AIControlled
    {
        get => isControllerByAI;
    }

    public List<UnitBase> ControlledUnits { get => controlledUnits; }

    public bool Local { get => localPlayer; }

    [SerializeField] protected GameResourceStructure[] resourcesCount;

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

    protected bool SpawnSelectedUnit(UnitBase unitPrefab)
    {
        foreach (GameResourceStructure resource in unitPrefab.Cost)
        {
            if (resource.Amount > getResource(resource.Type).Amount)
                return false;
        }
        unitPrefab.Parent = this;
        UnitBase unit = Instantiate(unitPrefab, transform.position, transform.rotation);
        unit.Parent = this;
        controlledUnits.Add(unit);
        foreach (GameResourceStructure resource in unitPrefab.Cost)
        {
            getResource(resource.Type).Amount -= resource.Amount;
        }
        return true;
    }

    public virtual void Awake()
    {
        units = allUnits.Units.ToList();
        units.RemoveAll(u => u.team != Team);      

        // WARNING
        // Replace this with something more...optimised I guess 

        if (menu && !AIControlled)
        {
            menu.options.Clear();
            menu.options.AddRange(units);
        }
        foreach (GameResourceStructure _res in resourcesCount)
        {
            _res.Appliance = _res.baseAppliance;
        }
    }

    public virtual void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= deltaTime)
        {
            timePassed = 0f;

            foreach (GameResourceStructure _res in resourcesCount)
            {
                _res.Amount += _res.Appliance;

                if (_res.Type == GameResources.WINPOINTS)
                {
                    GameManager.Instance.ValidateWin(this);
                }
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
