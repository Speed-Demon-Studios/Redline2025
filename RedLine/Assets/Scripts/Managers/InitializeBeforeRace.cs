using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InitializeBeforeRace : MonoBehaviour
{
    public bool movementEnabled = false;
    public GameObject playerCamOBJECT;
    public ShipsControls sControls;

    [SerializeField] private PlayerInputScript pInput;
    [SerializeField] private AIMoveInputs aiInput;
    [SerializeField] private Rigidbody rb;


    public void Initialize()
    {
        if (!GetComponent<ShipsControls>().isTestShip)
        {
            foreach (GameObject playerOBJ in GameManager.gManager.players)
            {
                if (this.gameObject == playerOBJ)
                {
                    DontDestroy ddol;

                    this.gameObject.TryGetComponent<DontDestroy>(out ddol);

                    if (ddol == null)
                    {
                        this.gameObject.AddComponent<DontDestroy>();
                    }
                    break;
                }
            }

            sControls = this.GetComponent<ShipsControls>();

            if(!GameManager.gManager.allRacers.Contains(this.gameObject))
                GameManager.gManager.allRacers.Add(this.gameObject);

            if (playerCamOBJECT != null)
            {
                playerCamOBJECT.SetActive(true);
            }

        }
    }

    public void DisableShipControls()
    {
        sControls.enabled = false;
    }

    public void EnableRacerMovement()
    {
        sControls.enabled = true;
    }

    public void InitializeForRace(GameObject racerOBJ)
    {
        RacerDetails rDeets = racerOBJ.GetComponent<RacerDetails>();

        rDeets.finishedRacing = false;
        rDeets.crossedFinishLine = false;
        
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        sControls.ResetAcceleration();
        rb.isKinematic = true;
        DisableShipControls();
    }
}
