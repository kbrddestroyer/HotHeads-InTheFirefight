using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class Disclaimer : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private VideoPlayer player;
    [SerializeField] private string mainMenuSceneName;
    
    private IEnumerator awaitDisclaimer(double sec)
    {
        yield return new WaitForSeconds((float) sec);
        while (player.isPlaying) yield return null;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void Start()
    {
        mixer.SetFloat("Master", PlayerPrefs.GetFloat("masterVolume"));

        StartCoroutine(awaitDisclaimer(player.clip.length));
    }
}
