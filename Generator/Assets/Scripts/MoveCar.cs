using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    private float speed = 0.5f;
    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 velocity = (transform.forward * vertical) * speed;
        rigidbody.MovePosition(transform.position + velocity);

        rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0f, horizontal * vertical, 0f));
    }
}
