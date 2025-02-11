using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetNormal : MonoBehaviour
{
    public Transform point;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.ToLower() == "racercolliders")
        {
            //if (other.transform.parent.gameObject.tag.ToLower() == "racer")
            //{
            //    if (other.transform.parent.parent.gameObject != null)
            //    {
            other.GetComponent<ShipsControls>().SetRotationToTrack(point);
            //    }
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<ShipsControls>() != null)
        {
            other.GetComponent<ShipsControls>().SetRotationToTrack(point);
        }
        //if (other.CompareTag("RacerColliders"))
        //    other.transform.parent.gameObject.GetComponentInParent<ShipsControls>().SetRotationToTrack(point);
    }
}
