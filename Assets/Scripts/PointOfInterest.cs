using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Color gizmoColor;

    [SerializeField, Range(-1f, 1f)] private float status = 0f; // 0 - neutral 1 - player's -1 - enemy's

    private List<UnitBase> units = new List<UnitBase>();
    private int weight;

    public float Status
    {
        get => status;
        set
        {
            status = value;
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
        mask = LayerMask.GetMask("Unit");
    }

    private void Update()
    {
        timePassed += Time.deltaTime;

        if (timePassed >= statusRate)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, distance, mask);

            float _status = 0.0f;

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
                status += _status * statusPerUnit;
            status = Mathf.Clamp(status, -1, 1);
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
