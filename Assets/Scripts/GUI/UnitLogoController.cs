using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLogoController : LogoController<UnitBase>
{
    public UnitBase ControlledObject
    {
        get => tControllerObject;
        set => tControllerObject = value;
    }

    public void Update()
    {
        PositionUpdate(tControllerObject.transform.position);
    }
}
