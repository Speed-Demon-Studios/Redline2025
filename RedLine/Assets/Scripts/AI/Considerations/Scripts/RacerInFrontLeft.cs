using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "RacersInFrontLeft", menuName = "Considerations/RacersInFrontLeft")]
public class RacerInFrontLeft : Consideration
{
    public override float Score(NPCController npc, ShipsControls controls)
    {
        if(npc.GetVisionCones().frontLeft.objects.Count > 0) // if there are ships in the front left sensor
        {
            foreach(GameObject racer in npc.GetVisionCones().frontLeft.objects) // go through all the ships in the sensor
            {
                Vector3 directionToRacer = (npc.transform.position - racer.transform.position).normalized; // direction to the ship in sensor
                Vector3 fowardFacingDirection = (npc.transform.position - controls.facingPoint.position).normalized; // front foward facing direction

                float angle = Vector3.SignedAngle(directionToRacer, fowardFacingDirection, npc.transform.up); // angle between direction to the ship in sensor and the foward facing direction
                float radAngle = angle * Mathf.Deg2Rad; // turn the angle to a radian

                float tempScore = curve.Evaluate(radAngle); // score the rad based on the curve

                if(tempScore > score) // if the temp score is larger than the current score
                {
                    score = tempScore; // set the score to the tempScore
                }
            }

            return score; // return the score
        }
        else
        {
            score = 0;
            return score;
        }


    }
}
