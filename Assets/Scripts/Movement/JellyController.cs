using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JellyController : MonoBehaviour, IController<Rigidbody>
{
    float timer = 0;
    public Transform copyTransformPosition;
    SkinnedMeshRenderer smr;
    Rigidbody rbody;

    void Start()
    {
        smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        rbody = copyTransformPosition.gameObject.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        Control(rbody);
        timer += Time.deltaTime;
    }
    
    public void Control(Rigidbody rbody)
    {
        transform.position = copyTransformPosition.position;
        transform.right = rbody.velocity;
        smr.SetBlendShapeWeight(1, rbody.velocity.magnitude * 10f); //xz jiggle
        smr.SetBlendShapeWeight(0, 100f * Mathf.Abs(Mathf.Sin(Time.time)));
    }

    float n;
    public void OnChangeSlider()
    {
        if (timer - n > 0.1f) //problem
        {
            Taptic.Selection();
            n = timer;
        }
    }
}
