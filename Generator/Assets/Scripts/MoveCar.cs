using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;
    private bool m_brakeInput;

    private Vector3 centerOfMass;
    private WheelCollider[] wheelColliders;

    [SerializeField]
    float maxSpeed = 200f;
    [SerializeField]
    float breakSpeed = 400f;
    [SerializeField]
    float maxSteerAngle = 30f;

    void Start()
    {
        centerOfMass = GetComponent<Rigidbody>().centerOfMass;
        GetWheels();
        RotateWheelsCollider();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        Steer();
        Motor();
    }

    private void LateUpdate()
    {
        UpdateWheelPoses();
    }
    private void UpdateWheelPoses()
    {
        Vector3 _pos;
        Quaternion _quat;
        foreach (var wheel in wheelColliders)
        {
            wheel.GetWorldPose(out _pos, out _quat);

            wheel.transform.GetChild(0).position = new Vector3(_pos.x + CenterX(wheel) * wheel.center.x / 4, _pos.y, _pos.z);
            wheel.transform.GetChild(0).rotation = wheel.transform.localPosition.x > centerOfMass.x ? _quat : _quat * Quaternion.Euler(0, 180, 0);
        }
    }

    private int CenterX(WheelCollider wheel)
    {
        bool rightpos = wheel.transform.localPosition.x > centerOfMass.x;
            return rightpos ? 1 : -1;
    }

    private void Motor()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = m_verticalInput * maxSpeed;
            if (m_brakeInput)
                wheel.brakeTorque = breakSpeed;
            else
                wheel.brakeTorque = 0;
        }

    }
    private void Steer()
    {
        foreach (var wheel in wheelColliders)
        {
            if (wheel.transform.localPosition.z < centerOfMass.z)
                wheel.steerAngle = maxSteerAngle * m_horizontalInput;
        }
    }

    private void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = -Input.GetAxis("Vertical");
        m_brakeInput = Input.GetButton("Jump");
    }

    private void GetWheels()
    {
        List<WheelCollider> WheelColliderList = new List<WheelCollider>();

        WheelCollider[] childTransforms = gameObject.GetComponentsInChildren<WheelCollider>() as WheelCollider[];
        foreach (var child in childTransforms)
            if (child.gameObject.name == "Wheel_Collider")
                WheelColliderList.Add(child);

        wheelColliders = WheelColliderList.ToArray();
    }

    private void RotateWheelsCollider()
    {
        foreach (var wheel in wheelColliders)
            if (wheel.transform.localPosition.x > centerOfMass.x)
                wheel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            else
                wheel.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
}
