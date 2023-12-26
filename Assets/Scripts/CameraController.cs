using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.Serialization;

[Tooltip("Camera Controller is a root object which defines it focus point")]
public class CameraController : MonoBehaviour
{
    /*  
     *      Main Camera controller class
     *   Capable of camera movement and angle change
     *   
     *   TODO:
     *   1. Add angle change [DONE]
     *   2. Comment and review the code
     *   3. Add settings tab for controls and store settings for sens
     */

    [Header("Base Settings")]
    [Tooltip("Camera root movement speed")]
    [SerializeField, Range(0f, 10f)] private float speed;
    [Tooltip("Smooth camera movement value")]
    [SerializeField, Range(0f, 1000f)] private float smoothness;
    [Tooltip("Mouse sens")]
    [SerializeField, Range(0f, 100f)] private float mouseWheelSens;
    [SerializeField, Range(0f, 100f)] private float mouseSens;
    [Header("Camera Settings")]
    [SerializeField, Range(-25f, 25f)] private float minDistance;
    [SerializeField, Range(-25f, 25f)] private float maxDistance;
    [Header("Required Objects")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform cameraRotationManager;
    [SerializeField] private Canvas mainCanvas;
    [Header("Editor only")]
    [Tooltip("Color for gizmo in Unity Editor")]
    [SerializeField] private Color gizmoColor;

    public Canvas MainCanvas { get { return mainCanvas; } }

    private Vector3 countDestinationPoint(Vector2 input)
    {
        return (transform.forward * input.y + transform.right * input.x) * speed;
    }

    // CameraController's game cycle consists of 3 main steps:

    private void moveCamera(Vector2 input)   // Moving camera along X-Z axis
    {
        Vector3 precomputedPos = transform.position + countDestinationPoint(input) * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, precomputedPos, Time.deltaTime * smoothness);
    }

    private void zoomCamera(float mouseWheel)   // Zooming camera along local Z axis
    {
        Vector3 cameraPrecompute = mainCamera.transform.localPosition + new Vector3(0, 0, 1) * mouseWheel * mouseWheelSens;
        cameraPrecompute.z = Mathf.Clamp(cameraPrecompute.z, minDistance, maxDistance);
        mainCamera.transform.localPosition = cameraPrecompute;
    }

    private void rotateCamera(Vector2 mousePosition) // Rotating camera with mouse when middle button is being pressed
    {
        Vector3 rotationEulers = cameraRotationManager.transform.rotation.eulerAngles + new Vector3(mousePosition.y, mousePosition.x, 0) * mouseSens;
        rotationEulers.x = Mathf.Clamp(rotationEulers.x, 0f, 90f);
        
        Vector3 eulerAngles = new Vector3(rotationEulers.x, cameraRotationManager.transform.rotation.eulerAngles.y, cameraRotationManager.transform.rotation.eulerAngles.z);
        
        cameraRotationManager.transform.rotation = Quaternion.Euler(eulerAngles);
        
        transform.rotation = Quaternion.Euler(new Vector3(0, rotationEulers.y, 0));
    }


    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * mouseWheelSens;

        moveCamera(input);
        zoomCamera(mouseWheel);
        
        if (Input.GetKey(KeyCode.Mouse2))
        {
            Vector2 mousePosition = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            rotateCamera(mousePosition);
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
