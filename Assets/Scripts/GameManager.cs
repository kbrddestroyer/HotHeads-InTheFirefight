using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Sigleton object

    [SerializeField] protected PlayerController localPlayerController;
    [SerializeField, Range(0f, 1000f)] protected float winpointsRequired;

    [SerializeField] protected GameObject winState;
    [SerializeField] protected GameObject looseState;

    private static PointOfInterest[] poiList;
    public static PointOfInterest[] POI { get => poiList; }

    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    private void Start()
    {
        instance = this;
        poiList = FindObjectsOfType<PointOfInterest>();
    }

    public void ValidateWin(PlayerController controller)
    {
        if (controller.getResource(GameResources.WINPOINTS).Amount >= winpointsRequired)
        {
            if (controller.Team == localPlayerController.Team)
                winState.SetActive(true);
            else looseState.SetActive(true);
        }
    }
}
