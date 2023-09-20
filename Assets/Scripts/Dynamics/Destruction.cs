using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
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
                }
            }
        }
    }
}
