using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyConstrainer : MonoBehaviour
{

    Rigidbody rb;
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;
    Vector3 initialLocalTransform;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialLocalTransform = transform.localPosition;
    }

    private void Update()
    {
        if (rb != null)
        {
            Vector3 lockedPosition = transform.localPosition;
            if (lockX)
                lockedPosition.x = initialLocalTransform.x;
            if (lockY)
                lockedPosition.y = initialLocalTransform.y;
            if (lockZ)
                lockedPosition.z = initialLocalTransform.z;
            transform.localPosition = lockedPosition;

        }
    }
}
