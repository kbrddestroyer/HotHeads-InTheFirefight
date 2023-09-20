using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{
    /*
     *  BlastWave - Dynamic effect with damage counting
     *  BASE CLASS:     MonoBehaviour
     *  TYPE:           Dynamic effect
     *  LAST MODIFIED:  06.05.23 20:00
     *  
     *  BlastWave class controls dynamic effect, attached to particle system
     *  and calculates damage, dealed to dynamic props and units, depending on
     *  their distance to the center of explodion
     *  _______________________________________________________________________________________________________________
     *                  TODO:
     *             1. Optimisation
     *  _______________________________________________________________________________________________________________
     */
    // -----                    SERIALIZABLE                    ----- //
    [SerializeField, Range(1f, 360f)] private int pointsCount;        // Smoothness
    [SerializeField, Range(1f, 20f)]  private float maxRadius;        // Maximum explodion range
    [SerializeField, Range(1f, 10f)]  private float startWidth;       // Starting line width
    [SerializeField, Range(1f, 50f)]  private float speed;            // Spreading speed
    [SerializeField, Range(0f, 10f)]  private float force;            // Applies to rigidbodies
    [SerializeField, Range(0f, 100f)] private float maxDamage;        // Damage in center. Recalculated on distance
    // -----                    PRIVATE                         ----- //
    private LineRenderer lineRenderer;                                  

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = pointsCount + 1;
        StartCoroutine(Blast());
    }

    private IEnumerator Blast()
    {
        for (float currentRadius = 0f; currentRadius < maxRadius; currentRadius += Time.deltaTime * speed)
        {
            Draw(currentRadius);
            DealDamage(currentRadius);
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }

    private void DealDamage(float radius)
    {
        Collider[] hitting = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in hitting)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();

            float damage = (maxRadius - radius) * maxDamage;
            if (rb)
            {
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                rb.AddForce(direction * force, ForceMode.Impulse);
            }
        }
    }

    private void Draw(float radius)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for (int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * radius;

            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - radius / maxRadius);
    }
}
