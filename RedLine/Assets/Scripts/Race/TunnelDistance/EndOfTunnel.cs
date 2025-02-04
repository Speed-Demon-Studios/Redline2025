using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTunnel : MonoBehaviour
{
    public Transform point;

    private void Awake()
    {
        foreach (GameObject racerOBJ in GameManager.gManager.players)
        {
            RacerDetails rDeets = racerOBJ.GetComponent<RacerDetails>();

            rDeets.endOfTunnelOBJ = this.gameObject;
        }
    }
}
