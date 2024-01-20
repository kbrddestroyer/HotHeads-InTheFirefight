using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix;
using Sirenix.OdinInspector;
using static UnityEngine.GraphicsBuffer;

public class TurretIK : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private Transform gunBone;
    [SerializeField, Range(0f, 10f)] private float rotationSpeed;
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private Vector3 gunRotationAxis;
    [SerializeField] private Vector3 initialEulers;
    [SerializeField] private Vector3 gunInitialEulers;

    private Transform target = null;

    public Transform Target { get => target; set => target = value; }

    private Vector3 CountAxis(Vector3 rot, Vector3 fac)
    {
        return new Vector3(rot.x * fac.x, rot.y * fac.y, rot.z * fac.z);
    }

    private void RotateTurret(Vector3 position)
    {
        Vector3 rotateTo = Quaternion.LookRotation(position - transform.position).eulerAngles;
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(initialEulers + CountAxis(rotateTo, rotationAxis)), Time.deltaTime * rotationSpeed);
        gunBone.localRotation = Quaternion.Euler(gunInitialEulers + CountAxis(rotateTo, gunRotationAxis));
    }

    private void Update()
    {
        if (target) 
            RotateTurret(target.position);
    }

#if UNITY_EDITOR
    [Button("Set Test Target")]
    private void SetTestTarget(Transform target)
    {
        Target = target;
    }
#endif
}
