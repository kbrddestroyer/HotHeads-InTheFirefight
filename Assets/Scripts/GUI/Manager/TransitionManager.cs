using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition Manager Required Components")]
    [SerializeField, SceneObjectsOnly] private Animator transition;
    [SerializeField] private Slider progress;

    private async Task sceneLoader(string sceneName)
    {
        transition.SetTrigger("transition");
        await Task.Delay(1500);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loading.allowSceneActivation = false;
        do
        {
            progress.value = loading.progress;
            await Task.Delay(1500);
        } while (loading.progress < 0.9f);
        AIUnitController.ClearPois();
        loading.allowSceneActivation = true;
    }

    public async void Invoke(string sceneName)
    {
        await sceneLoader(sceneName);
    }

    public void InvokeRestart()
    {
        Invoke(SceneManager.GetActiveScene().name);
    }
}
