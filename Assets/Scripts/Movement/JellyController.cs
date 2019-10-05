using UnityEngine;

public class JellyController : MonoBehaviour, IController<Rigidbody>
{
    public Transform copyTransformPosition;
    SkinnedMeshRenderer smr;

    void Start()
    {
        smr = gameObject.GetComponent<SkinnedMeshRenderer>();
    }

    void LateUpdate()
    {
        Control(copyTransformPosition.gameObject.GetComponent<Rigidbody>());
    }

    public void Control(Rigidbody rbody)
    {
        transform.position = copyTransformPosition.position;
        transform.right = rbody.velocity;
        smr.SetBlendShapeWeight(1, rbody.velocity.magnitude * 10f); //xz jiggle

        smr.SetBlendShapeWeight(0, 100f * Mathf.Abs(Mathf.Sin(Time.time)));
    }
}
