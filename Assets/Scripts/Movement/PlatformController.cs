using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Camera cam;
    Vector3 initialPos, finalPos;
    public float magnitude;
    public float limitAngle = 10f;
    bool mouseUp = true;

    Quaternion rotationMax;
    Quaternion rotationMin;

    private void Start()
    {
        rotationMin = Quaternion.Euler(new Vector3(-limitAngle, 0f, -limitAngle));
        rotationMax = Quaternion.Euler(new Vector3(limitAngle, 0f, limitAngle));
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            mouseUp = true;
        }

        if (Input.GetMouseButton(0))
        {
            initialPos = Input.mousePosition;
            if(mouseUp)
            {
                finalPos = Input.mousePosition;
                mouseUp = false;
            }
            Vector3 delta = finalPos - initialPos;
            MovePlatform(delta);
            finalPos = Input.mousePosition;
        }
    }

    void MovePlatform(Vector3 delta)
    {
        transform.Rotate(0, 0, magnitude * delta.x, Space.Self);
        transform.Rotate(-delta.y * magnitude, 0, 0, Space.World);

        if (transform.rotation.x > rotationMax.x || transform.rotation.x < rotationMin.x
            || transform.rotation.z > rotationMax.z || transform.rotation.z < rotationMin.z)
        {
            transform.rotation = Quaternion.Euler
                (
                ClampAngle(EulerError(transform.eulerAngles.x), -limitAngle, limitAngle),
                0,
                ClampAngle(EulerError(transform.eulerAngles.z), -limitAngle, limitAngle)
                );
        }
    }

    public float EulerError(float angle)
    {
        float f;
        if (angle > 300)
        {
            f = angle - 360f;
        }
        else
        {
            f = angle;
        }
        return f;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
