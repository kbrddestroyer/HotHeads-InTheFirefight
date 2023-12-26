using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PointOfInterest : MonoBehaviour
{
    #region VARIABLES
    [Header("Base Settings")]
    [SerializeField, Range(0f, 10f)] private float distance;
    [SerializeField, Range(0f, 10f)] private float pointsRate;
    [SerializeField, Range(0f, 1f)] private float statusPerUnit;
    [SerializeField, Range(0f, 10f)] private float statusRate;
    [SerializeField] GameResources resource;
    [Header("GUI")]
    [SerializeField] private POILogoController poiLabel;

    [SerializeField] private string poiName;
    [SerializeField] private Color gizmoColor;

    [SerializeField, Range(-1f, 1f)] private float status = 0f; // 0 - neutral 1 - player's -1 - enemy's

    private Teams controlTeam = Teams.NONE;

    private List<UnitBase> units = new List<UnitBase>();
    private POILogoController attachedLogo;
    [SerializeField] private int weight;

    private List<PlayerController> A_sidePlayers;
    private List<PlayerController> B_sidePlayers;

    public string Name { get => poiName; }

    public Teams ControlTeam
    {
        get => controlTeam;
        set {
            controlTeam = value;
            if (value == Teams.NONE) return;
            foreach (PlayerController controller in (controlTeam == Teams.TEAM_A) ? A_sidePlayers : B_sidePlayers)
            {
                controller.addResourceAppliance(resource, pointsRate);
            }
            foreach (PlayerController controller in (controlTeam == Teams.TEAM_A) ? B_sidePlayers : A_sidePlayers)
            {
                controller.addResourceAppliance(resource, -pointsRate);
            }
        }
    }

    public float Status
    {
        get => status;
        set
        {
            status = value;
            attachedLogo.setStatus(value);
        }
    }

    public List<UnitBase> Units
    {
        get => units;
    }

    public int Weight
    {
        get => weight;
    }

    #endregion

    private LayerMask mask;
    private float timePassed = 0;
    private void Awake()
    {
        A_sidePlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).ToList();
        B_sidePlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).ToList();
        A_sidePlayers.RemoveAll(item => item.Team != Teams.TEAM_A);
        B_sidePlayers.RemoveAll(item => item.Team != Teams.TEAM_B);

        mask = LayerMask.GetMask("Unit");

        attachedLogo = Instantiate(poiLabel, FindObjectOfType<CameraController>().MainCanvas.transform).GetComponent<POILogoController>();
        attachedLogo.ControllerObject = this;
        attachedLogo.setName(this.poiName);
        string type = "";
        switch (resource) {
            case GameResources.WINPOINTS:
                type = "WPS";
                break;
            case GameResources.MANPOWER:
                type = "MANP";
                break;
            case GameResources.OIL:
                type = "OIL";
                break;
        }

        attachedLogo.setType(type);
    }

    private void Update()
    {
        timePassed += Time.deltaTime;

        if (timePassed >= statusRate)
        {
            timePassed = 0f;
            Collider[] colliders = Physics.OverlapSphere(transform.position, distance, mask);

            float _status = 0.0f;
            weight = 0;
            foreach (Collider col in colliders)
            {
                UnitBase _base = col.GetComponent<UnitBase>();

                if (_base != null)
                {
                    _status += (_base.Team == Teams.TEAM_A) ? 1 : -1;
                    weight += (_base.Team == Teams.TEAM_A) ? 1 : -1 * _base.Weight;
                }
            }

            if ((status < 1 && _status > 0) || (status > -1 && _status < 0))
                Status += _status * statusPerUnit;
            Status = Mathf.Clamp(status, -1, 1);
            if (Mathf.Abs(status) == 1 && controlTeam == Teams.NONE)
            {
                ControlTeam = (status == 1) ? Teams.TEAM_A : Teams.TEAM_B;
            }
            else if (Mathf.Abs(status) < 1) ControlTeam = Teams.NONE;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
#endif
}
