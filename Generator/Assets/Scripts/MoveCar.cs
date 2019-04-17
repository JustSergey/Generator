using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    [SerializeField]
    private MachinePhysics motor;
    private MachineInput control = new MachineInput();

    void FixedUpdate()
    {
        control.GetInput();
        motor.Steer(control);
    }

    private void LateUpdate()
    {
        motor.AnimateMesh();
    }

    public void InitWheels()
    {
        motor = new MachinePhysics(gameObject,GetComponent<Rigidbody>().centerOfMass);
        motor.UpdateWheels();
        motor.RotateWheelsColliders();
    }
}

[System.Serializable]
public class MachinePhysics
{
    public float maxSpeed = 3000;
    public float breakSpeed = 5500;
    public float maxSteerAngle = 30;
    private GameObject gameObject;

    private Vector3 centerOfMass;
    private WheelCollider[] wheelColliders;

    public MachinePhysics(GameObject _gameObject,Vector3 _centerOfMass)
    {
        gameObject = _gameObject;
        centerOfMass = _centerOfMass;
    }

    public void UpdateWheels()
    {
        List<WheelCollider> WheelColliderList = new List<WheelCollider>();

        WheelCollider[] childTransforms = gameObject.GetComponentsInChildren<WheelCollider>() as WheelCollider[];
        foreach (var child in childTransforms)
            WheelColliderList.Add(child);

        wheelColliders = WheelColliderList.ToArray();
    }

    public void RotateWheelsColliders()
    {
        foreach (var wheel in wheelColliders)
            if (!(wheel is null))
            wheel.transform.localRotation = IsRightWheel(wheel) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

    public void AnimateMesh()
    {
        Vector3 _pos;
        Quaternion _quat;
        foreach (var wheel in wheelColliders)
            if (!(wheel is null))
            {
                wheel.GetWorldPose(out _pos, out _quat);
                wheel.transform.GetChild(0).position = _pos;
                wheel.transform.GetChild(0).rotation = IsRightWheel(wheel) ? _quat : _quat * Quaternion.Euler(0, 180, 0);
            }
    }

    private bool IsRightWheel(WheelCollider wheel)
    {
        return wheel.transform.localPosition.x > centerOfMass.x;
    }
    private bool IsFrontWheel(WheelCollider wheel)
    {
        return wheel.transform.localPosition.z < centerOfMass.z;
    }

    public void Steer(MachineInput input)
    {
        foreach (var wheel in wheelColliders)
            if (!(wheel is null))
            {
                Rotate(wheel, input);
                Move(wheel, input);
            }
    }

    private void Move(WheelCollider wheel, MachineInput input)
    {
        wheel.motorTorque = input.Vertical * maxSpeed;
        wheel.brakeTorque = input.Brake ? breakSpeed : 0;
    }

    private void Rotate(WheelCollider wheel, MachineInput input)
    {
        if (IsFrontWheel(wheel))
            wheel.steerAngle = maxSteerAngle * input.Horizontal;
    }
}

public struct MachineInput
{
    private float horizontal;
    private float vertical;
    private bool brake;

    public float Horizontal => horizontal;
    public float Vertical => vertical;
    public bool Brake => brake;

    public void GetInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");
        brake = Input.GetButton("Jump");
    }
}