using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class ButtonController
{
    [Header("UI Toolkit button controller")]
    [SerializeField] private UIDocument root;
    [SerializeField] private string buttonTag;
    [SerializeField] private UnityEvent action = new UnityEvent();
    [SerializeField] private UnityEvent hover = new UnityEvent();

    private Button button;

    public void Initialise()
    {
        button = root.rootVisualElement.Q<Button>(buttonTag);
        button.RegisterCallback<ClickEvent>(OnClick);
        button.RegisterCallback<MouseOverEvent>(OnHover);
    }

    private void OnClick(ClickEvent e)
    {
        action.Invoke();
    }

    private void OnHover(MouseOverEvent e)
    {
        hover.Invoke();
    }
}
