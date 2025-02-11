using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPositionDetails : MonoBehaviour
{
    public GameObject HeldRacer;
    public bool SpotFilled = false;
    private void Awake()
    {
        HeldRacer = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (HeldRacer != null)
        {
            SpotFilled = true;
        }
        else
        {
            SpotFilled = false;
        }
    }
}
