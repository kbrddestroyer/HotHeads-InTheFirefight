using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOWController : MonoBehaviour
{
    /*  
     *  Separated Logic - FOW Controller
     *  
     *  Fog of War was made incorrectly. This class fixes it
     */

    [SerializeField, Range(0f, 100f), LabelText("FOW Cutoff Distance")] private float fFowCutoffDistance;
    [SerializeField] protected Teams team;
    [SerializeField] protected UnitBase unit;
    [SerializeField] private LayerMask unitMask;
    [SerializeField, ChildGameObjectsOnly] private GameObject meshRoot;
    [SerializeField] private Color gizmoColor = new Color(0, 0, 0, 1.0f);

    private bool state = false;

    protected virtual void ToggleFOWLate(bool state)
    {
        meshRoot.SetActive(state);
        unit.UnitLogoController.gameObject.SetActive(state);
    }

    protected virtual void ToggleFOW(bool state)
    {
        this.state = state;
    }

    private void Update()
    {
        // Do smth with that
        if (unit.Parent.Local)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, fFowCutoffDistance, LayerMask.GetMask("Unit"));

            foreach (Collider col in colliders)
            {
                UnitBase baseUnitController = col.GetComponent<UnitBase>();
                if (baseUnitController && unit.Parent.Team != baseUnitController.Team)
                {
                    baseUnitController.GetComponent<FOWController>().ToggleFOW(true);
                }
            }
        }
    }

    public void LateUpdate()
    {
        if (unit.Parent.AIControlled)
            ToggleFOWLate(state);
        state = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, fFowCutoffDistance);
    }
#endif
}
