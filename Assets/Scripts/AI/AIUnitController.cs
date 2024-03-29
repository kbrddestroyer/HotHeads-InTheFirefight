using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitBase))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIUnitController : MonoBehaviour
{
    [Header("Unit Dependensies")]
    [SerializeField] private UnitBase unitBase;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask unitLayer;

    [SerializeField] private int teamID;
    [SerializeField, Range(0f, 10f)] private float fTriggerDistance;

    [SerializeField] private PointOfInterest currentPoi;

    private void Awake()
    {
        if (!unitBase.Parent.AIControlled)
            this.enabled = false;
    }

    private void Start()
    {
        currentPoi = SelectOptimalPoint();
        agent.destination = currentPoi.transform.position;
    }

    private PointOfInterest SelectOptimalPoint()
    {
        PointOfInterest optimal = GameManager.POI[0];

        foreach (PointOfInterest poi in GameManager.POI)
        {
            if (poi.Status == -1) continue;
            if (poi.Weight * teamID < optimal.Weight * teamID && poi.Weight >= 0)
                optimal = poi;
        }

        return optimal;
    }

    private void Update()
    {
        // Attack logic side

        Collider[] colliders = Physics.OverlapSphere(transform.position, fTriggerDistance, unitLayer);
        bool attack = false;
        foreach (Collider collider in colliders)
        {
            UnitBase _base = collider.GetComponent<UnitBase>(); 
            if (_base != null && _base.Team != unitBase.Team)
            {
                agent.destination = transform.position;
                attack = true;
            }
        }
        if (attack) return;

        // Destination choose
        
        if ((agent.velocity.magnitude == 0) && (currentPoi && currentPoi.Status == -1))
        {
            currentPoi = SelectOptimalPoint(); 
            agent.destination = currentPoi.transform.position;
        }
    }

#if UNITY_EDITOR
    [Button("Get Dependencies")]
    private void GetDependencies()
    {
        unitBase = GetComponent<UnitBase>();
        agent = GetComponent<NavMeshAgent>();
        unitLayer = LayerMask.GetMask("Unit");
        teamID = (unitBase.Team == Teams.TEAM_A) ? 1 : -1;
    }

#endif
}
