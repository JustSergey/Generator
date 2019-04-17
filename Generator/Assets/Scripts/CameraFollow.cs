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
    private float sensitivity = 1F;

    private float MyAngle = 0F;

    void FixedUpdate()
    {
        Vector3 MousePos = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            MyAngle = sensitivity * (MousePos.x - (Screen.width / 2)) / Screen.width;
            transform.RotateAround(target.transform.position, transform.up, MyAngle);

            var rightTurn = Quaternion.Euler(0, 90, 0); // Создаём новый поворот направо
            transform.rotation = transform.rotation * rightTurn; // Крутим

            //MyAngle = sensitivity * ((MousePos.y - (Screen.height / 2)) / Screen.height);
            //transform.RotateAround(target.transform.position, transform.right, -MyAngle);
        }

        //Vector3 desiredPosition = target.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //transform.position = smoothedPosition;



        //transform.LookAt(target);
    }
}
