using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorkscrewReset : MonoBehaviour
{
    public Transform point;
    public int resetIndex;

    private void Awake()
    {
        foreach (GameObject racerOBJ in GameManager.gManager.players)
        {
            RacerDetails rDeets = racerOBJ.GetComponent<RacerDetails>();

            if (resetIndex == 1)
            {
                rDeets.corkscrewReset1 = this.gameObject;
            }
            else if (resetIndex == 2)
            {
                rDeets.corkscrewReset2 = this.gameObject;
            }
            //rDeets.resetNormalOBJs.Add(this.gameObject);
        }
    }
}
