using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private string pathToOSTFolder;
    [SerializeField] private Slider slider;

    private Object[] clips;
    private float currentClipLength = 0f;
    private float passedSeconds = 0f;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private AudioClip getNewRandomClip()
    {
        AudioClip clip = (AudioClip) clips[Random.Range(0, clips.Length)];
        currentClipLength = clip.length;
        passedSeconds = 0f;
        return clip;
    }

    public void LoadNewClip()
    {
        source.Stop();
        source.clip = getNewRandomClip();
        source.Play();
    }

    private void PlayNewRandomClip()
    {
        if (!source.isPlaying)
        {
            LoadNewClip();
        }
    }

    private void Update()
    {
        passedSeconds += Time.deltaTime;
        slider.value = passedSeconds / currentClipLength;

        if (passedSeconds >= currentClipLength)
        {
            PlayNewRandomClip();
        }
    }

    private void OnValidate()
    {
        Debug.Log("Loading resources...");
        clips = Resources.LoadAll(pathToOSTFolder);
        Debug.Log($"Total: {clips.Length}");
        foreach (Object ob in clips)
        {
            Debug.Log(ob.name);
        }
    }
}
