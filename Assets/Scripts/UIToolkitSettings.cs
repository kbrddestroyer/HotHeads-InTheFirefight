using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class UIToolkitSettings : MonoBehaviour
{
    [SerializeField] private string masterTag;
    [SerializeField] private string musicTag;
    [SerializeField] private string sfxTag;
    [SerializeField] private UIDocument root;
    [SerializeField] private AudioMixer mixer;

    private const string masterKey = "masterVolume";
    private const string musicKey = "musicVolume";
    private const string sfxKey = "sfxVolume";

    private void Start()
    {
        Slider master = root.rootVisualElement.Q<Slider>(masterTag);
        master.RegisterValueChangedCallback((ev) => { OnMasterValueChanged(master.value); });
        Slider music = root.rootVisualElement.Q<Slider>(musicTag);
        music.RegisterValueChangedCallback((ev) => { OnMusicValueChanged(music.value); });
        Slider sfx = root.rootVisualElement.Q<Slider>(sfxTag);
        sfx.RegisterValueChangedCallback((ev) => { OnSFXValueChanged(sfx.value); });
        float masterVol;
        float musicVol;
        float sfxVol;

        masterVol = PlayerPrefs.GetFloat(masterKey);
        musicVol = PlayerPrefs.GetFloat(musicKey);
        sfxVol = PlayerPrefs.GetFloat(sfxKey);

        master.value = masterVol;
        music.value = musicVol;
        sfx.value = sfxVol;

        mixer.SetFloat("Master", masterVol);
        mixer.SetFloat("Music", musicVol);
        mixer.SetFloat("SFX", sfxVol);
    }

    public void OnMasterValueChanged(float value)
    {
        mixer.SetFloat("Master", value);
        PlayerPrefs.SetFloat(masterKey, value);
        PlayerPrefs.Save();
    }

    public void OnMusicValueChanged(float value)
    {
        mixer.SetFloat("Music", value);
        PlayerPrefs.SetFloat(musicKey, value);
        PlayerPrefs.Save();
    }

    public void OnSFXValueChanged(float value)
    {
        mixer.SetFloat("SFX", value);
        PlayerPrefs.SetFloat(sfxKey, value);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            this.gameObject.SetActive(false);
    }
}
