using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource Sound;
    public AudioClip celebrationClip;
    bool oneTime = true;
    public Toggle soundToggle;

    void Start()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            Sound.mute = PlayerPrefs.GetInt("sound") == 0 ? true : false;
            soundToggle.isOn = !Sound.mute;
        }

        if (PlayerPrefs.HasKey("vibration"))
        {
            vibrationToggle.isOn = PlayerPrefs.GetInt("vibration") == 1 ? true : false;
        }
    }

    public void ToggleSound()
    {
        Sound.mute = (soundToggle.isOn) ? false : true;
        PlayerPrefs.SetInt("sound", Sound.mute ? 0 : 1);
    }

    void LateUpdate()
    {
        SoundControl();
    }

    public void SoundControl()
    {
        if (oneTime)
        {
            if (NewLevel.GameOver)
            {
                oneTime = false;
                Sound.volume = 1f;
                Sound.clip = celebrationClip;
                Sound.loop = false;
                Sound.Play();
            }
        }
    }

    public Toggle vibrationToggle;

    public void OnChangeSlider()
    {
        if (vibrationToggle.isOn) { Taptic.Selection(); }
    }

    public void OnVibrationToggle()
    {
        PlayerPrefs.SetInt("vibration", vibrationToggle.isOn ? 1 : 0);
    }
}