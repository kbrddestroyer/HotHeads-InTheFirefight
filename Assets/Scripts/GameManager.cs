using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Sigleton object

    [SerializeField] private PlayerController localPlayerController;
    [SerializeField, Range(0f, 1000f)] private float winpointsRequired;

    [SerializeField] private GameObject winState;
    [SerializeField] private GameObject looseState;

    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    private void Start()
    {
        instance = this;
    }

    public void ValidateWin(PlayerController controller)
    {
        Debug.Log($"validation {controller.getResource(GameResources.WINPOINTS).Amount} ");
        if (controller.getResource(GameResources.WINPOINTS).Amount >= winpointsRequired)
        {
            if (controller.Team == localPlayerController.Team)
                winState.SetActive(true);
            else looseState.SetActive(true);
        }
    }
}
