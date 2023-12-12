using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitLogoController : LogoController<UnitBase>
{
    private static bool _enabled = true;
    public static bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            Debug.Log(value);

            UnitLogoController[] unitLogoControllers = FindObjectsByType<UnitLogoController>(FindObjectsSortMode.None);
            foreach (UnitLogoController unitLogo in unitLogoControllers)
            {
                unitLogo.toggle(_enabled && unitLogo.LocalStatus);
            }
        }
    }

    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject guiRoot;

    private bool localStatus = true;

    public bool LocalStatus
    {
        get => localStatus;
        set {
            localStatus = value;

            toggle(_enabled && localStatus);
        }
    }

    public void setName(string name)
    {
        label.text = name;
    }

    public UnitBase ControlledObject
    {
        get => tControllerObject;
        set => tControllerObject = value;
    }

    public void toggle(bool status)
    {
        guiRoot.SetActive(status);
    }

    private new void Awake()
    {
        base.Awake();
        toggle(_enabled && localStatus);
    }

    public void Update()
    {
        PositionUpdate(tControllerObject.transform.position + Vector3.up);
    }
}
