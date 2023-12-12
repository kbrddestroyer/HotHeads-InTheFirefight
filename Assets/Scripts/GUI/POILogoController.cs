using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class POILogoController : LogoController<PointOfInterest>
{
    [SerializeField] private TMP_Text poiName;
    [SerializeField] private TMP_Text poiType;
    [SerializeField] private Slider status;

    public void setName(string name) { poiName.text = name; }
    public void setType(string type) { poiType.text = type; }
    public void setStatus(float status) { this.status.value = status; }

    private void Update()
    {
        PositionUpdate(tControllerObject.transform.position + Vector3.up);
    }
}
