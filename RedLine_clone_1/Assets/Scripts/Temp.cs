using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public Vector3 _transform;
    public float speed;
    public bool startMove = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(startMove)
            transform.position += _transform * speed * Time.deltaTime;
    }
}
