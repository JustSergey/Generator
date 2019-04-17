using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Generate))]
public class Genetic : MonoBehaviour
{
    [SerializeField]
    private float secondsToLife;
    private Vector3 position;
    private float time;

    void Start()
    {
        position = transform.position;
        time = 0f;
    }
    
    void FixedUpdate()
    {
        if (time >= secondsToLife)
        {
            GetComponent<Generate>().ReSpawn(position, true);
            time = 0f;
        }
        else
        {
            time += Time.fixedDeltaTime;
        }
    }
}
