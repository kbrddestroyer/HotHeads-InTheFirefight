using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyCarLogic : MechanisedBase, IDestructable, ISelectable, IUnit
{
    [SerializeField, Range(0f, 10f)] private float supplyDelay;
    [SerializeField, Range(0f, 10f)] private float supplyRange;
    [SerializeField, Range(0, 10)] private int supplyAmount;

    [SerializeField] private Color gizmoColor;
    private LineRenderer lineRenderer;

    private float timePassed;
    #region EXTENDED
    public override void OnDeath()
    {
        // ???
    }

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        lineRenderer.positionCount = 0;
        timePassed += Time.deltaTime;
        LayerMask mask = LayerMask.GetMask("Unit");
        Collider[] colliders = Physics.OverlapSphere(transform.position, supplyRange, mask);
        foreach (Collider collider in colliders)
        {
            ShootingUnitBase shootingUnit = collider.GetComponent<ShootingUnitBase>();
            if (shootingUnit != null && shootingUnit.Team == this.team)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, shootingUnit.transform.position);

                if (timePassed >= supplyDelay)
                {
                    Supply(shootingUnit);
                }
            }
        }

        if (timePassed >= supplyDelay) timePassed = 0;
    }
    #endregion

    private void Supply(ShootingUnitBase shootingUnit)
    {
        shootingUnit.AmmoTotal += supplyAmount;
    }

    private void Supply(MechanisedBase mechanisedUnit)
    {
        // No supply yet
    }

    #region UNITY_EDITOR
#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, supplyRange);
    }
#endif
    #endregion
}
