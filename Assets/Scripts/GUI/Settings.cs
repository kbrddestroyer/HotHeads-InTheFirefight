using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;

using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private static Settings instance;
    public static Settings Instance { get => instance; }

    [SerializeField] private Slider master;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;

    [SerializeField] private AudioMixer mixer;

    private const string masterKey = "masterVolume";
    private const string musicKey = "musicVolume";
    private const string sfxKey = "sfxVolume";

    private void Start()
    {
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

    public void OnMasterValueChanged()
    {
        mixer.SetFloat("Master", master.value);
        PlayerPrefs.SetFloat(masterKey, master.value);
        PlayerPrefs.Save();
    }

    public void OnMusicValueChanged()
    {
        mixer.SetFloat("Music", music.value);
        PlayerPrefs.SetFloat(musicKey, music.value);
        PlayerPrefs.Save();
    }

    public void OnSFXValueChanged()
    {
        mixer.SetFloat("SFX", sfx.value);
        PlayerPrefs.SetFloat(sfxKey, sfx.value);
        PlayerPrefs.Save();
    }
}
