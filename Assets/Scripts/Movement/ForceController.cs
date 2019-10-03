using UnityEngine;

public class ForceController : MonoBehaviour, IController
{
    // manual motion of ball // add forces other than gravity
    Vector3 fakeForceDir;
    Vector3 gravityDirection = Vector3.down;
    Vector3 platformNormalDir;

    Rigidbody rbody;

    GameObject vectorHandler;

    public float power = 10;

    void Awake()
    {
        vectorHandler = GameObject.Find("VectorHandler");
        rbody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Control();
    }

    public void Control()
    {
        Vector3 veloDir = rbody.velocity;
        platformNormalDir = vectorHandler.transform.up;
        fakeForceDir = Vector3.ProjectOnPlane(gravityDirection, platformNormalDir);

        if (rbody.velocity.magnitude < 4)
        {
            PhysicsFormula(1.5f);
        }
        else
        {
            PhysicsFormula(1f);
        }
    }

    void PhysicsFormula(float factor)
    {
        rbody.AddForce(fakeForceDir * power * factor);
        rbody.velocity /= 1.1f;
    }
}
