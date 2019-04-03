using System;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelRotate : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public float maxSteerAngle = 30;
    public float motorForce = 500;


    WheelCollider collider;

    public float streengthCoefficient = 100000f;

    void Start()
    {
        collider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    private void UpdateWheelPoses()
    {
        Vector3 _pos;
        Quaternion _quat;

        collider.GetWorldPose(out _pos, out _quat);

        transform.position = _pos;
        transform.rotation = _quat; 
    }

    private void Accelerate()
    {
        collider.motorTorque = m_verticalInput * motorForce;
    }

    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        collider.steerAngle = m_steeringAngle;
    }

    private void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = -Input.GetAxis("Vertical");
    }
}
