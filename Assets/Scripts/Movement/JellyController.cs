using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JellyController : MonoBehaviour, IController<Rigidbody>
{
    //float timer = 0;
    public Transform copyTransformPosition;
    SkinnedMeshRenderer smr;
    Rigidbody rbody;
    public Toggle vibrationToggle;

    void Start()
    {
        smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        rbody = copyTransformPosition.gameObject.GetComponent<Rigidbody>();
        if (PlayerPrefs.HasKey("vibration"))
        {
            vibrationToggle.isOn = PlayerPrefs.GetInt("vibration") == 1 ? true : false;
        }
    }

    void LateUpdate()
    {
        Control(rbody);
        //timer += Time.deltaTime;
    }
    
    public void Control(Rigidbody rbody)
    {
        transform.position = copyTransformPosition.position;
        transform.right = rbody.velocity;
        smr.SetBlendShapeWeight(1, rbody.velocity.magnitude * 10f); //xz jiggle
        smr.SetBlendShapeWeight(0, 100f * Mathf.Abs(Mathf.Sin(Time.time)));
    }

    public void OnChangeSlider()
    {
        if (vibrationToggle.isOn) { Taptic.Selection();}
    }

    public void OnVibrationToggle()
    {
        PlayerPrefs.SetInt("vibration", vibrationToggle.isOn ? 1 : 0);
    }
}
