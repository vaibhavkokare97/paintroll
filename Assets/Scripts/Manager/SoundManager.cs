using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Sound;
    public AudioClip celebrationClip;
    public Rigidbody rbody;
    bool oneTime = true;

    void Start()
    {

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