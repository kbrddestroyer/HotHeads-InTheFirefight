using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Selection : MonoBehaviour, ISingleton
{
    private static Selection instance;
    public static Selection Instance { get => instance; }

    [SerializeField, Range(0f, 40f)] private float delta;
    [SerializeField] private RectTransform selection;
    // [SerializeField] private GameObject selector;

    private Camera mainCamera;
    private bool isSelection = false;
    private bool selectionEnabled = true;
    private bool multiSelect = false;
    private Vector2 startPos, endPos;

    public bool IsMulti { get => multiSelect; }

    private static List<ISelectable> selectables = new List<ISelectable>();
    public static void RegisterNewSelectable(ISelectable selectable)
    {
        if (!selectables.Contains(selectable))
            selectables.Add(selectable);
    }

    public static void RemoveSelectable(ISelectable selectable)
    {
        if (selectables.Contains(selectable))
            selectables.Remove(selectable);
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        if (instance != null)
            throw new Exception("Not single selection manager on scene!");

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionEnabled)
        {
            Vector2 mousePos = Input.mousePosition;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                startPos = mousePos;
                multiSelect = false;
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (
                    Vector2.Distance(mousePos, startPos) > delta
                    )
                {
                    isSelection = true;
                    UpdateSelectionBox(mousePos);
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0) && isSelection)
            {
                ReleaseSelectionBox();
                isSelection = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) selectionEnabled = !selectionEnabled;
    }

    private void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selection.gameObject.activeInHierarchy)
            selection.gameObject.SetActive(true);
        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;
        selection.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selection.position = startPos + new Vector2(width / 2, height / 2);
    }

    void ReleaseSelectionBox()
    {
        selection.gameObject.SetActive(false);
        Vector2 min = selection.anchoredPosition - (selection.sizeDelta / 2);
        Vector2 max = selection.anchoredPosition + (selection.sizeDelta / 2);

        uint uCount = 0;

        foreach (ISelectable _selectable in selectables)
        {
            Vector2 screenPosition = mainCamera.WorldToScreenPoint(_selectable.WorldPosition);
            if (
                (screenPosition.x > min.x && screenPosition.y > min.y) &&
                (screenPosition.x < max.x && screenPosition.y < max.y)
                )
            {
                _selectable.ToggleSelection(true);
                uCount++;
            }
            else _selectable.ToggleSelection(false);
        }

        multiSelect = uCount > 1;
    }
}
