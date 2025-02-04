using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishlineTrigger : MonoBehaviour
{
    [SerializeField] private RaceManager rM;

   // private void OnTriggerEnter(Collider other)
   // {
   //     if (other.tag.ToLower() == "racer")
   //     {
   //         RacerDetails rDeets = other.gameObject.GetComponent<RacerDetails>();
   //         Debug.Log("Crossed Line!");
   //         rM.LapComplete(rDeets);
   //     }
   // }
}
