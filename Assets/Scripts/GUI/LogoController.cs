using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogoController<T> : MonoBehaviour
{
    [SerializeField] protected T tControllerObject;

    private Camera mainCamera;

    public T ControllerObject
    {
        get => tControllerObject;
        set
        {
            tControllerObject = value;
        }
    }

    public void Awake()
    {
        mainCamera = Camera.main;
    }

    protected void PositionUpdate(Vector3 _position)
    {
        Vector3 position = mainCamera.WorldToScreenPoint(_position);
        transform.position = position;
    }
}
