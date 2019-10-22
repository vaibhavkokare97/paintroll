using UnityEngine;

public class ParticleController : MonoBehaviour, IController
{
    public Transform copyTransformPosition;
    public Vector3 Offset;
    public bool towards = false;
    public float lerpVal;

    void LateUpdate()
    {
        Control();
    }

    public void Control()
    {
        transform.position = copyTransformPosition.position + Offset;
        if (towards && copyTransformPosition.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 2f)
        {
            transform.forward = Vector3.Lerp(transform.forward, Quaternion.AngleAxis(30, transform.right) * copyTransformPosition.gameObject.GetComponent<Rigidbody>().velocity.normalized, Time.deltaTime * lerpVal);
            //transform.up = Quaternion.AngleAxis(-30, Vector3.right) * (copyTransformPosition.position - this.gameObject.transform.position);
        }
    }
}
