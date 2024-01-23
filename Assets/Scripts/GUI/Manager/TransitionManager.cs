using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition Manager Required Components")]
    [SerializeField, SceneObjectsOnly] private Animator transition;
    [SerializeField] private Slider progress;

    private IEnumerator asyncSceneLoader(string sceneName)
    {
        transition.SetTrigger("transition");
        yield return new WaitForSeconds(5.0f);
        progress.gameObject.SetActive(true);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loading.allowSceneActivation = false;
        while (loading.progress < 0.9f)
        {
            progress.value = loading.progress;
            yield return new WaitForEndOfFrame();
        }
        loading.allowSceneActivation = true;
    }

    public void Invoke(string sceneName)
    {
        StartCoroutine(asyncSceneLoader(sceneName));
    }
}
