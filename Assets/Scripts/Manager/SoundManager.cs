using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource Sound;
    public AudioClip celebrationClip;
    public Rigidbody rbody;
    bool oneTime = true;
    public Toggle soundToggle;

    void Start()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            Sound.mute = PlayerPrefs.GetInt("sound") == 0 ? true : false;
            soundToggle.isOn = !Sound.mute;
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
#if JELLY
            Sound.volume = rbody.velocity.magnitude / 14f;
#endif 

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
}