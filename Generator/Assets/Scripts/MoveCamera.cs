using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    private Vector3 pos;

    void Start()
    {
        pos = player.transform.InverseTransformPoint(transform.position);
    }

    void LateUpdate()
    {
        transform.position = player.transform.TransformPoint(pos);
        transform.position = new Vector3(transform.position.x, player.transform.position.y + 4, transform.position.z); 
        transform.LookAt(player.transform.position);
    }
}
