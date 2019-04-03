using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelRotate : MonoBehaviour
{
    WheelCollider collider;

    public float streengthCoefficient = 10000f;

    void Start()
    {
        collider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.y < 0)
            collider.motorTorque = streengthCoefficient * Time.deltaTime * Input.GetAxis("Vertical");



        transform.Rotate(0, 0, collider.rpm / 60 * 360 * Time.deltaTime);
    }
}
