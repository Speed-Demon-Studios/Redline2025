using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHaptics : MonoBehaviour
{
    [Header("References")]
    [Space]
    [Header("Public Variables")]
    [Header("Public floats")]
    public float rumbleDurationAtTarget = 0.0f;

    // !!********************************************************!!
    // !! Only PRIVATE variables and references past this point. !!
    // !!********************************************************!!

    private PlayerInputScript m_pInputScript;   // Reference to PlayerInputScript.cs
    private Gamepad m_playerGamepad;            // Gamepad/Controller Input Device

    // Float Variables
    // ------------------------------------------------------
    private float m_boost1Rumble = 0.012f;      // High-Frequency Motor Speed For Boost LVL1
    private float m_boost2Rumble = 0.15f;       // High-Frequency Motor Speed For Boost LVL2
    private float m_boost3Rumble = 0.45f;       // High-Frequency Motor Speed For Boost LVL3
    private float currentRumble = 0.0f;         // Current High-Frequency Motor Speed For Rumble Scaling
    private float targetRumble = 0.0f;          // Target High-Frequency Motor Speed For Rumble Scaling
    // ------------------------------------------------------

    // Bool Variables
    private bool m_rumbling = false;
    private bool m_rumbleReady = false;
    private bool m_stopRumble = false;
    private bool m_chargeUpRumble = false;
    // ------------------------------------------------------


    private void Awake()
    {
        m_pInputScript = this.gameObject.GetComponent<PlayerInputScript>();
    }

    public void ConfigureRumble(Gamepad controller = null, float rumbleLevel = 0.0f)
    {
        if (controller != null)
        {
            controller.SetMotorSpeeds(rumbleLevel, rumbleLevel);
        }
    }

    public void RumbleTiming(Gamepad controller = null, int rumbleType = 0, float duration = 0)
    {
        currentRumble = 0.0f;
        targetRumble = 0.0f;
        rumbleDurationAtTarget = duration;

        switch (rumbleType)
        {
            case 0: // DO NOTHING
                {
                    break;
                }
            case 1: // Boost LVL1
                {
                    targetRumble = m_boost1Rumble;
                    break;
                }
            case 2: // Boost LVL2
                {
                    targetRumble = m_boost2Rumble;
                    break;
                }
            case 3: // Boost LVL3
                {
                    targetRumble = m_boost3Rumble;
                    break;
                }
        }

        if (rumbleType > 0)
        {
            m_rumbleReady = true;
            m_chargeUpRumble = true;
        }
        return;
    }

    private IEnumerator WaitRumbleDuration()
    {
        yield return new WaitForSecondsRealtime(rumbleDurationAtTarget);
        m_stopRumble = true;
        StopCoroutine(WaitRumbleDuration());
    }

    void Update() // Update Function, Called every frame. ---------------------------------------------------------------------------------------------------------------------------------||
    { //                                                                                                                                                                                   ||
        if (m_pInputScript != null && m_pInputScript.GetPlayerGamepad() != null) // If the PlayerInputScript.cs reference has been set, and the controller information has been assigned   ||
        {     //                                                                                                                                                                           ||
            if (m_playerGamepad == null) // If the reference to the players gamepad/controller input device has NOT been set                                                               ||
            { //                                                                                                                                                                           ||
                m_playerGamepad = m_pInputScript.GetPlayerGamepad();        // Set the reference to the players gamepad/controller input device.                                           ||
            } // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------||
            if (m_rumbleReady == true)          // <-\                                                                                                                                     ||
            {                                   //    }---{ If the 'RumbleTiming()' function has been called, & has finished running }                                                     ||
                if (m_chargeUpRumble == true)   // <-/                                                                                                                                     ||
                { //                                                                                                                                                                       ||
                    ConfigureRumble(m_playerGamepad, targetRumble); // Activate the controller rumble.                                                                                     ||
                                                                    //                                                                                                                     ||
                    m_chargeUpRumble = false;                               // Rumble is no longer "charging up" (AKA: Controller is now rumbling).                                        ||
                    m_rumbling = true;                                      // Controller is currently rumbling.                                                                           ||
                                                                            //                                                                                                             ||
                    StartCoroutine(WaitRumbleDuration());                   // Start the Coroutine 'WaitRumbleDuration()'.                                                                 ||
                }                                                           // ------------------------------------------------------------------------------------------------------------||
                else if (m_chargeUpRumble == false && m_stopRumble == true) // If the controller is finished charging up & the 'WaitRumbleDuration()' Coroutine has finished               ||
                {                                                           //                                                                                                             ||
                    targetRumble = 0.0f;                                    // Set the current target high-frequency motor speed to zero (0).                                              ||
                                                                            //                                                                                                             ||
                    if (currentRumble > targetRumble) // While the current high-frequency motor speed is GREATER than the target motor speed                                               ||
                    {                                                       //                                                                                                             ||
                        currentRumble -= 1.1f * Time.deltaTime;             // Decrease the current high-frequency motor speed over time.                                                  ||
                        ConfigureRumble(m_playerGamepad, currentRumble);    // Apply the new current high-frequency motor speed to the controllers rumble.                                 ||
                    }                                                       // ------------------------------------------------------------------------------------------------------------||
                    if (currentRumble <= targetRumble) // If the current high-frequency motor speed has reached the target motor speed                                                     ||
                    {                                                       //                                                                                                             ||
                        currentRumble = targetRumble;                       // Set the current high-frequency motor speed to the target motor speed as a precaution.                       ||
                        m_rumbling = false;                                 // The controller is no longer rumbling.                                                                       ||
                        m_stopRumble = false;                               // The controller is no longer stopping rumbling.                                                              ||
                                                                            //                                                                                                             ||
                        ConfigureRumble(m_playerGamepad, currentRumble);    // Apply the high-frequency motor speed changes to the controller.                                             ||
                        m_playerGamepad.PauseHaptics();                     // Fully stop the rumble haptics for the controller.                                                           ||
                    } // ------------------------------------------------------------------------------------------------------------------------------------------------------------------||
                } // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------||
            } // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------||
        } // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------||
        else // Otherwise, if either the reference to PlayerInputScript has NOT been set, AND/OR the reference to the players gamepad/controller input device hasnt been set               ||
        { //                                                                                                                                                                               ||
            m_rumbleReady = false;                                          // The controller is NOT ready to rumble. DO NOTHING.                                                          ||
        } // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------||
    }
}
