using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject buttonTab;
    [SerializeField] private GameObject settingsPanel;

    public void TogglePauseTab()
    {
        pausePanel.SetActive(!pausePanel.activeInHierarchy);
    }

    public void ToggleSettingsTab()
    {
        buttonTab.SetActive(!buttonTab.activeInHierarchy);
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseTab();
        }
    }
}
