using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private RacerDetails racer;
    public bool enteredCheckpoint = false;
    public bool finalCheckpoint = false;
    
    private void Awake()
    {
        racer = GetComponentInParent<RacerDetails>();
    }
}
