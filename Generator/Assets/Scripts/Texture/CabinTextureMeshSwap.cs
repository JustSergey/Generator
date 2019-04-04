using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinTextureMeshSwap : MonoBehaviour
{
    GameObject Mesh;
    GameObject Mesh_low;
    // Start is called before the first frame update
    void Start()
    {
        Mesh = transform.GetChild(0).gameObject;
        Mesh_low = transform.GetChild(1).gameObject;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
