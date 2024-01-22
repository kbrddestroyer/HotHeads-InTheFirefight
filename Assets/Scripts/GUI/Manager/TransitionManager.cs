using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public void Invoke(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
