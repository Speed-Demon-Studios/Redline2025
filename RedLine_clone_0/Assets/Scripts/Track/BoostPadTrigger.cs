using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPadTrigger : MonoBehaviour
{
    public float force;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.ToLower() == "racer")
        {
            Debug.Log("Boost pad triggered");
            ShipsControls test;
            if (other.gameObject.TryGetComponent<ShipsControls>(out test))
                test.BoostPadBoost(force);
        }
    }
}
