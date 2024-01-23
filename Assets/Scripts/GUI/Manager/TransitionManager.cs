using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition Manager Required Components")]
    [SerializeField, SceneObjectsOnly] private Animator transition;

    private IEnumerator asyncSceneLoader(string sceneName)
    {
        transition.SetTrigger("transition");
        yield return new WaitForSeconds(5.0f);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loading.allowSceneActivation = false;
        while (loading.progress < 0.9f) 
            yield return null;

        loading.allowSceneActivation = true;
    }

    public void Invoke(string sceneName)
    {
        StartCoroutine(asyncSceneLoader(sceneName));
    }
}
