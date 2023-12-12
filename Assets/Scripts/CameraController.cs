using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*  
     *      Main Camera controller class
     *   Capable of camera movement and angle change
     *   
     *   TODO:
     *   1. Add angle change
     *   
     */

    [Header("Base Settings")]
    [SerializeField, Range(0f, 10f)] private float speed;
    [SerializeField, Range(0f, 1000f)] private float smoothness;
    [SerializeField, Range(0f, 100f)] private float mouseWheelSens;
    [Header("Camera Settings")]
    [SerializeField, Range(-25f, 25f)] private float minDistance;
    [SerializeField, Range(-25f, 25f)] private float maxDistance;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform cameraRotationManager;
    [SerializeField] private Canvas mainCanvas;
    [Header("Editor only")]
    [SerializeField] private Color gizmoColor;

    private Quaternion initialRotation;

    public Canvas MainCanvas { get { return mainCanvas; } }

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");

        float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * mouseWheelSens;
        Vector3 destination = transform.forward * axisY * speed + transform.right * axisX * speed;
        Vector3 precomputedPos = transform.position + destination * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, precomputedPos, Time.deltaTime * smoothness);

        Vector3 cameraPrecompute = mainCamera.transform.localPosition + new Vector3(0, 0, 1) * mouseWheel * mouseWheelSens;
        cameraPrecompute.z = Mathf.Clamp(cameraPrecompute.z, minDistance, maxDistance);
        mainCamera.transform.localPosition = cameraPrecompute;

        if (Input.GetKey(KeyCode.Mouse2))
        {
            // Camera rotation goes here

            float mouseAxisX = Input.GetAxis("Mouse X");
            float mouseAxisY = Input.GetAxis("Mouse Y") * -1;
            Vector3 rotationEulers = cameraRotationManager.transform.rotation.eulerAngles + new Vector3(mouseAxisY, mouseAxisX, 0) * 10;
            rotationEulers.x = Mathf.Clamp(rotationEulers.x, 0f, 90f);

            cameraRotationManager.transform.rotation = Quaternion.Euler(new Vector3(rotationEulers.x, cameraRotationManager.transform.rotation.eulerAngles.y, cameraRotationManager.transform.rotation.eulerAngles.z));
            transform.rotation = Quaternion.Euler(new Vector3(0, rotationEulers.y, 0));
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
        
    }
#endif
}
