using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelRotate : MonoBehaviour
{
    WheelCollider collider;

    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    float DAMax = 40f;
    float WheelAngleMax = 10f;
    float MAxSpeed = 100f;
    float Speed = 40f;

    void Start()
    {
        collider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");

        transform.Rotate(0, 0, collider.rpm / 60 * 360 * Time.deltaTime);
    }
}
