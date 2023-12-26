using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitBase))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIUnitController : MonoBehaviour
{
    private UnitBase unitBase;
    private NavMeshAgent agent;
    private float fTriggerDistance = 0;
    private LayerMask unitLayer;
    private static List<PointOfInterest> poiList;
    private int teamID;

    private void Awake()
    {
        unitBase = GetComponent<UnitBase>();
        agent = GetComponent<NavMeshAgent>();

        ShootingUnitBase shooting = unitBase as ShootingUnitBase;
        if (shooting != null)
        {
            fTriggerDistance = shooting.AttackDistance;
        }

        if (!unitBase.Parent.AIControlled)
            this.enabled = false;

        if (poiList == null)
        {
            poiList = new List<PointOfInterest>(FindObjectsOfType<PointOfInterest>());
        }
        unitLayer = LayerMask.GetMask("Unit");

        teamID = (unitBase.Team == Teams.TEAM_A) ? 1 : -1;
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
        PointOfInterest optimal = poiList[0];
        if (agent.velocity.magnitude == 0)
        {
            foreach (PointOfInterest poi in poiList)
            {
                Debug.Log($"{poi.Name} - {poi.Weight}");
                if (poi.Weight * teamID < optimal.Weight * teamID && poi.Weight >= 0)
                    optimal = poi;
            }

            agent.destination = optimal.transform.position;
        }
    }
}
