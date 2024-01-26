using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    private RagdollActivator[] activators;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                GameObject ob = transform.GetChild(i).gameObject;
                
                Rigidbody rb = ob.GetComponent<Rigidbody>();
                Collider col = ob.GetComponent<Collider>();
                if (ob != null && col != null)
                {
                    rb.isKinematic = false;
                    col.enabled = true;
                    Destroy(ob, 2);
                }
            }
        }
    }
}
