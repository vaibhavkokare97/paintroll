using UnityEngine;

public class ParticleController : MonoBehaviour, IController
{
    public Transform copyTransformPosition;

    void LateUpdate()
    {
        Control();
    }

    public void Control()
    {
        transform.position = copyTransformPosition.position;
    }
}
