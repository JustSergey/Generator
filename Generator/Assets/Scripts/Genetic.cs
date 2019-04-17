using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Generate))]
public class Genetic : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void FixedUpdate()
    {
        if (transform.position.y < 2f)
            GetComponent<Generate>().ReSpawn(new Vector3(transform.position.x, 10, transform.position.x), true);
    }
}
