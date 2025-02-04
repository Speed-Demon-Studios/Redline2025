using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestIndicatorOne : MonoBehaviour
{
    public Camera main;
    public Transform targetIndex;
    public RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        image.transform.position = main.WorldToScreenPoint(targetIndex.transform.position);
    }
}
