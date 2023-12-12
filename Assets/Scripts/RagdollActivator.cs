using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollActivator : MonoBehaviour, IRagdoll
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    public void Awake()
    {
        Switch(false);
    }

    public void Switch(bool bState)
    {
        rb.isKinematic = !bState;
        col.isTrigger = !bState;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
#endif
}
