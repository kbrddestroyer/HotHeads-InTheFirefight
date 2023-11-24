using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Teams team;
    [SerializeField] private bool isControllerByAI;
    [SerializeField, Range(0f, 10f)] private float deltaTime;
    [SerializeField] private GameObject explosion;

    private float timePassed;
    private LayerMask ground;

    public bool AIControlled
    {
        get => isControllerByAI;
    }

    [System.Serializable]
    public class Resource
    {
        [SerializeField] private Resources type;
        [SerializeField] private float amount;
        [SerializeField, Range(0f, 10f)] public float baseAppliance;

        private float appliance;

        [SerializeField, AllowNull] private TMP_Text label;
        [SerializeField, AllowNull] private TMP_Text applianceLabel;

        public Resources Type { get => type; } 

        public float Amount { get => amount; 
            set
            {
                amount = value;
                if (label != null)
                    label.text = amount.ToString();
            }
        }

        public float Appliance
        {
            get => appliance;
            set
            {
                appliance = value;
                if (applianceLabel != null)
                    applianceLabel.text = appliance.ToString();
            }
        }
    }

    [SerializeField] private Resource[] resourcesCount;

    public void Awake()
    {
        foreach (Resource _res in resourcesCount)
        {
            _res.Appliance = _res.baseAppliance;
        }

        ground = LayerMask.GetMask("Ground");
    }

    public void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= deltaTime)
        {
            timePassed = 0f;

            foreach (Resource _res in resourcesCount)
            {
                _res.Amount += _res.Appliance;
            }
        }

        if (isControllerByAI)
        {

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, ground))
                {
                    Instantiate(explosion, hit.point, Quaternion.identity);
                }
            }
        }
    }
}
