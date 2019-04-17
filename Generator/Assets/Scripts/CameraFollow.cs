using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float smoothSpeed = 0.125f;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float sensitivity = 2F;

    private float yaw = 0.0f;
    private float pitch = 0.0f;


    void FixedUpdate()
    {
        Vector3 MousePos = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            yaw += sensitivity * Input.GetAxis("Mouse X");
            pitch -= sensitivity * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
