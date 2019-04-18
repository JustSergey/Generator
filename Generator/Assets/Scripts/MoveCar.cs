using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Generate))]
public class MoveCar : MonoBehaviour
{
    [SerializeField]
    private MachinePhysics motor;
    private MachineInput control = new MachineInput();

    public bool AutoRun { get; set; }

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
        motor = new MachinePhysics(gameObject, GetComponent<Rigidbody>().centerOfMass, AutoRun);
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
    private List<WheelCollider> wheelColliders;

    private bool autoRun;

    public MachinePhysics(GameObject _gameObject,Vector3 _centerOfMass, bool _autoRun)
    {
        gameObject = _gameObject;
        centerOfMass = _centerOfMass;
        autoRun = _autoRun;
    }
    
    public void UpdateWheels()
    {
        wheelColliders = new List<WheelCollider>();
    
        WheelCollider[] childTransforms = gameObject.GetComponentsInChildren<WheelCollider>() as WheelCollider[];
        foreach (var child in childTransforms)
            wheelColliders.Add(child);
    }
    
    public void RotateWheelsColliders()
    {
        foreach (var wheel in wheelColliders)
            if (!IsDestroyed(wheel))
            wheel.transform.localRotation = IsRightWheel(wheel) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }
    
    public void AnimateMesh()
    {
        Vector3 _pos;
        Quaternion _quat;
        foreach (var wheel in wheelColliders)
            if (!IsDestroyed(wheel))
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
            if (!IsDestroyed(wheel))
            {
                Rotate(wheel, input);
                Move(wheel, input);
            }
    }

    private static bool IsDestroyed(WheelCollider wheel)
    {
        return (wheel.Equals(null));
    }

    private void Move(WheelCollider wheel, MachineInput input)
    {
        wheel.motorTorque = autoRun ? -maxSpeed : input.Vertical * maxSpeed;
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
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool Brake { get; private set; }

    public void GetInput()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = -Input.GetAxis("Vertical");
        Brake = Input.GetButton("Jump");
    }
}