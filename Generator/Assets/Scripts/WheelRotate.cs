using System;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelRotate : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;

    [SerializeField]
    float maxSteerAngle = 30;
    [SerializeField]
    float motorForce = 1000;

    [SerializeField]
    float brakeForce = 4;


    WheelCollider collider;
    GameObject Mesh;
    Transform CenterOfMass;

    void Start()
    {
        collider = GetComponent<WheelCollider>();
        CenterOfMass = GameObject.Find("CenterOfMass").transform;
        RotateMesh();
    }

    private void RotateMesh()
    {
        Mesh = transform.GetChild(0).gameObject;

        if (collider.transform.localPosition.x > 0)
            Mesh.transform.localRotation = Quaternion.Euler(0, -90, 0);
        else
            Mesh.transform.localRotation = Quaternion.Euler(0, 90, 0);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        inhibit();
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

    private void inhibit()
    {
        if (Input.GetKey(KeyCode.Space))
            collider.brakeTorque = motorForce * brakeForce;
        else
            collider.brakeTorque = 0;
    }

    private void Steer()
    {
        if (transform.localPosition.z < CenterOfMass.localPosition.z)
            collider.steerAngle = maxSteerAngle * m_horizontalInput;
    }

    private void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = -Input.GetAxis("Vertical");
    }
}
