using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuCameraHolder : MonoBehaviour
{
    [SerializeField, Range(0.5f, 10f)] private float fMouseSens;
    [SerializeField, Range(0f, 10f)] private float fImpulse;
    [SerializeField] private AudioMixer mixer;

    private Vector2 vImpulse = Vector2.zero;

    private void Rotate(Vector2 delta)
    {
        Vector3 rotationEulers = new Vector3(delta.y, delta.x, 0);
        transform.Rotate(rotationEulers);

        if (fImpulse > 0)
            vImpulse = Vector2.Lerp(vImpulse, Vector2.zero, fImpulse * Time.deltaTime); 
    }

    private void Start()
    {
        float masterVol = PlayerPrefs.GetFloat("masterVolume");
        float musicVol = PlayerPrefs.GetFloat("musicVolume");
        float sfxVol = PlayerPrefs.GetFloat("sfxVolume");

        mixer.SetFloat("Master", masterVol);
        mixer.SetFloat("Music", musicVol);
        mixer.SetFloat("SFX", sfxVol);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Rotate camera
            
            Vector2 mouseDeltaPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * fMouseSens * Time.deltaTime * 100;
            Rotate(mouseDeltaPosition);

            vImpulse = mouseDeltaPosition * fMouseSens * Time.deltaTime * 100;
        }

        else if (vImpulse.magnitude > 0)
            Rotate(vImpulse);
    }
}
