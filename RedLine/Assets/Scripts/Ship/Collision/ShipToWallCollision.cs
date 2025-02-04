using EAudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class ShipToWallCollision : MonoBehaviour
{
    [SerializeField] private ShipsControls sControlScript;
    [SerializeField] private float knockbackPercentage = 1.3f;
    private ShipVariant shipVariant;
    private float changedTopSpeed;
    private float changedAcceleration;
    private float defaultTopSpeed;
    private float defaultAcceleration;
    private bool intoWall = false;
    private bool detailsSet = false;
    private bool crashDelayFinished = true;
    private bool crashDelayStarted = false;
    private bool stillColliding = false;

    public void ResetDetails()
    {
        defaultTopSpeed = 0f;
        defaultAcceleration = 0f;
        changedTopSpeed = 0f;
        changedAcceleration = 0f;
        detailsSet = false;
    }

    public void UpdateDetails()
    {
        detailsSet = true;
        shipVariant = sControlScript.VariantObject;
        defaultTopSpeed = sControlScript.GetDefaultMaxSpeed;

        changedTopSpeed = (defaultTopSpeed * 0.53f); // The speed that ships will be capped at while colliding with walls.
    }

    private IEnumerator CrashDelayTimer()
    {
        crashDelayStarted = true;
        crashDelayFinished = false;

        yield return new WaitForSeconds(0.85f);

        crashDelayFinished = true;
        crashDelayStarted = false;
        StopCoroutine(CrashDelayTimer());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.ToLower() == "walls")
        {
            PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
            if (sControlScript == null)
            {
                sControlScript = this.GetComponent<ShipsControls>();
            }
            
            intoWall = true;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.tag.ToLower() == "walls" && intoWall == true)
        {
            stillColliding = true;
            sControlScript.SetCurrentMaxSpeed(changedTopSpeed);
            PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.tag.ToLower() == "walls" && intoWall == true)
        {
            PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
            intoWall = false;

            //stillColliding = false;
        }
    }

    private void Update()
    {
        if (GameManager.gManager != null && detailsSet == false && GameManager.gManager.raceStarted)
        {
            UpdateDetails();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (intoWall == false && detailsSet == true && GameManager.gManager.raceStarted)
        {
            if (sControlScript.GetCurrentMaxSpeed < defaultTopSpeed)
            {
                sControlScript.SetCurrentMaxSpeed((sControlScript.GetCurrentMaxSpeed + (defaultTopSpeed / 0.33f) * Time.deltaTime));
            }
            else if (shipVariant.DefaultMaxAcceleration > defaultTopSpeed)
            {
                sControlScript.SetCurrentMaxSpeed(defaultTopSpeed);
            }
        }
    }
}
