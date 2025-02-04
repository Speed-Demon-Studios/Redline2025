using EAudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class ShipsControls : MonoBehaviour
{
    [Header("Refrences")]
    [HideInInspector] public int shipSelected = 0;
    public ShipVariant VariantObject;
    public Transform rotation;
    public Transform facingPoint;
    public Transform collisionParent;
    public GameObject shipModel;
    public Transform rayCastPoint;
    private Rigidbody m_rb;
    private List<FireInfo> m_fire = new();
    public GameObject cameraRotationPoint;
    private int m_fireIndex;
    private int m_shipManiIndex;
    public bool isTestShip;

    [Space]
    [Header("Speed Variables")]
    private float m_accelerateMultiplier;
    private float m_brakeMultiplier;
    private float m_acceleration;
    private float m_currentMaxSpeed;
    private float m_defaultMaxSpeed;
    private bool m_hasDoneDifficultyChange;

    [Space]
    [Header("Turning Varibles")]
    [SerializeField] private float m_cameraTurnAngle;
    [SerializeField] private AnimationCurve m_modelRotationCurve;
    [SerializeField] private float m_strafeStrength;

    private float m_targetAngle;
    private float m_currentAngle;
    private float m_shipAngle;
    private float m_strafeMultiplier;
    private float m_turningAngle;
    private float m_turnFireAngle;
    private float m_strafeAnimAngle;

    [Space]
    [Header("TrackStick")]
    [SerializeField] private float m_shipAdjustSpeed;
    private Vector3 m_targetPos;
    private Vector3 m_currentPos;

    [Space]
    [Header("Boost Variables")]
    [SerializeField] private float m_accelerationForce;
    private bool m_wantingToBoost;
    [SerializeField] private float m_howFastYouGetBoost;
    [SerializeField] private float m_howFastYouLooseBoost;
    [SerializeField] private float m_maxBoostSpeedChange;
    [SerializeField] private List<float> m_boostingTimes = new();

    private float m_currentBoost;
    private bool m_isBoosting;
    private bool m_isInRedline;
    private bool m_isBoostingOnBoostPad;
    private float m_maxSpeedDuringBoost;
    [SerializeField, Range(0,3)] private int m_boostLevel;
    bool chargeSound0Played = false;
    bool chargeSound1Played = false;
    bool chargeSound2Played = false;

    public TextMeshProUGUI test;
    /////////////////////////////////////////////////////////////////
    ///                                                           ///
    ///      All of the getters and setters in this script        ///
    ///                                                           ///
    /////////////////////////////////////////////////////////////////

    public bool WantingToBoost => m_wantingToBoost;
    public Rigidbody RB => m_rb;
    public List<FireInfo> FireList => m_fire;
    public float TurnFireAngle => m_turnFireAngle;
    public float StrafeAnimAngle => m_strafeAnimAngle;
    public float GetDefaultMaxSpeed => m_defaultMaxSpeed;
    public float GetCurrentMaxSpeed => m_currentMaxSpeed;
    public float GetBrakeMultiplier => m_brakeMultiplier;
    public float GetAccelerationMultiplier => m_accelerateMultiplier;
    public float GetTurnMultiplier => m_turningAngle + m_strafeMultiplier; 
    public float ReturnBoost => m_currentBoost;
    public int ReturnBoostLevel => m_boostLevel;
    public bool ReturnIsBoosting => m_isBoosting;
    public bool ReturnIsInRedline => m_isInRedline;

    public void DelayRedlineFalse() { StopCoroutine(RedlineFalse()); StartCoroutine(RedlineFalse()); }

    public void ChangeDoneDifficulty(bool change) { m_hasDoneDifficultyChange = change; }
    public void SetCurrentMaxSpeed(float speed) { m_currentMaxSpeed = speed; }
    public void SwitchRedlineBool(bool switchTo) { m_isInRedline = switchTo; }
    public void MaxSpeedCatchupChange(float multiplier) { m_currentMaxSpeed = m_defaultMaxSpeed * multiplier; }
    public void SetMaterialIndex(int index) { m_shipManiIndex = index; }


    private IEnumerator RedlineFalse()
    {
        yield return new WaitForSeconds(0.2f);

        m_isInRedline = false;
    }
    
    public void ResetRedline()
    {
        m_isInRedline = false;
        m_wantingToBoost = false;
        m_isBoostingOnBoostPad = false;
        m_currentBoost = 0.0f;
        m_boostLevel = 0;
    }

    public void ResetAngles(float angle1, float angle2, float angle3)
    {
        m_currentAngle = angle1;
        m_targetAngle = angle2;
        m_shipAngle = angle3;
    }

    public void ResetPositions(Vector3 position)
    {
        m_currentPos = position;
        m_targetPos = position;
    }

    public void ResetAcceleration()
    {
        m_wantingToBoost = false;
        m_boostLevel = 0;
        m_acceleration = 0;
    }

    /// <summary>
    /// Spawn the models onto the ship for the ship that the player chose
    /// </summary>
    /// <param name="ship"> Which player ship controls is it </param>
    public void AttachModels()
    {
        if (shipModel.transform.childCount > 0)
            DestroyImmediate(shipModel.transform.GetChild(0).gameObject);

        Instantiate(VariantObject.model, shipModel.transform);
        Instantiate(VariantObject.collision, collisionParent);
    }

    public void DeInitialize()
    {
        ResetPositions(new Vector3(0, 0, 0));
        ResetAcceleration();
        ResetAngles(0, 0, 0);
    }

    public void Initialize(bool isAIShip = false)
    {
        m_rb = GetComponent<Rigidbody>();

        if (!isAIShip)
        {
            AttachModels();
            if(shipModel != null)
            {
                FindChildWithTag(shipModel.transform);
            }
            if (VariantObject != null && !m_hasDoneDifficultyChange)
            {
                m_defaultMaxSpeed = VariantObject.DefaultMaxSpeed;
            }
            foreach(FireInfo fire in m_fire)
            {
                fire.TurnFireOff();
            }
            gameObject.GetComponent<ShipBlendAnimations>().Inistialize();
            shipModel.transform.GetComponentInChildren<ShipTypeInfo>().SwitchMaterials(m_shipManiIndex);
        }

        DifficultySpeedChange();
    }

    private void DifficultySpeedChange()
    {
        m_defaultMaxSpeed = VariantObject.DefaultMaxSpeed * GameManager.gManager.difficultyChange;
        m_currentMaxSpeed = m_defaultMaxSpeed;
        m_maxSpeedDuringBoost = m_defaultMaxSpeed + m_maxBoostSpeedChange;
        m_hasDoneDifficultyChange = true;
    }

    private void FindChildWithTag(Transform childParent)
    {
        foreach (Transform child in childParent)
        {
            if (child.GetComponent<FireInfo>() != null)
            {
                m_fire.Add(child.GetComponent<FireInfo>());
            }

            if (child.childCount > 0)
            {
                FindChildWithTag(child);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isTestShip || this.enabled && GameManager.gManager.raceStarted)
        {
            if(m_fire.Count > 0)
                SwitchFire();
            CheckBoost();
            Strafe();
            Turn();
            Brake();
            Accelerate();
            DownForce();
            RotateShip();
            AddToBoost();
        }
    }

    /// <summary>
    /// switches the fire
    /// </summary>
    private void SwitchFire()
    {
        if (!isTestShip)
        {
            PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
            if (m_fire.Count > 0)
            {
                m_fireIndex = m_boostLevel;
                switch (m_fireIndex)
                {
                    case 0:
                        foreach(FireInfo fire in m_fire)
                        {
                            fire.TurnFireOff();
                        }
                        break;
                    case 1:
                        if (PAC != null)
                        {
                            PAC.SetBoostPitch(0, 1.0f);
                            PAC.SetBoostPitch(1, 1.55f);
                            if (chargeSound0Played == false)
                            {
                                chargeSound0Played = true;
                                PAC.PlayBoostAudio(0);
                            }
                        }
                        m_fire[0].TurnFireOn();
                        m_fire[1].TurnFireOff();
                        m_fire[2].TurnFireOff();
                        break;
                    case 2:
                        if (PAC != null)
                        {
                            PAC.SetBoostPitch(0, 1.3f);
                            PAC.SetBoostPitch(1, 1.3f);
                            if (chargeSound1Played == false)
                            {
                                chargeSound1Played = true;
                                PAC.PlayBoostAudio(0);
                            }
                        }
                        m_fire[1].TurnFireOn();
                        m_fire[2].TurnFireOff();
                        m_fire[0].TurnFireOff();
                        break;
                    case 3:
                        if (PAC != null)
                        {
                            PAC.SetBoostPitch(0, 1.5f);
                            PAC.SetBoostPitch(1, 0.75f);
                            if (chargeSound2Played == false)
                            {
                                chargeSound2Played = true;
                                PAC.PlayBoostAudio(0);
                            }
                        }
                        m_fire[2].TurnFireOn();
                        m_fire[0].TurnFireOff();
                        m_fire[1].TurnFireOff();
                        break;
                }
            }
        }

    }

    /// <summary>
    /// this will check if you are currently boosting and if not then slowly take away from the current boost
    /// </summary>
    private void CheckBoost()
    {
        if (!m_isInRedline && m_currentBoost > 0)
        {
            if(m_currentBoost > m_boostLevel)
            {
                m_currentBoost -= m_howFastYouLooseBoost * Time.deltaTime;
                if(m_currentBoost < m_boostLevel)
                {
                    m_currentBoost = m_boostLevel;
                }
            }

        }
        if (m_fire.Count > 0)
            SwitchFire();
    }

    /// <summary>
    /// Adding to the current boost when in a red line and changing the level of boost the character is at
    /// </summary>
    public void AddToBoost()
    {
        if (m_isInRedline)
        {
            float multiplier = 1f / (m_boostLevel + 1);
            m_currentBoost += m_howFastYouGetBoost * multiplier * Time.deltaTime;
            if (m_currentBoost > 3)
            {
                m_currentBoost = 3;
            }
            switch (m_currentBoost)
            {
                case < 1:
                    m_boostLevel = 0;
                    m_fireIndex = 0;
                    break;
                case < 2:
                    m_boostLevel = 1;
                    m_fireIndex = 1;
                    break;
                case < 3:
                    m_boostLevel = 2;
                    m_fireIndex = 2;
                    break;
                case < 4:
                    m_boostLevel = 3;
                    m_fireIndex = 3;
                    break;
            }
            CheckBoost();
        }
    }

    /// <summary>
    /// Rotates the ship after the calculations
    /// </summary>
    private void RotateShip()
    {
        float tempTarget;
        if (m_targetAngle < 0)
            tempTarget = -m_targetAngle;
        else
            tempTarget = m_targetAngle;

        float tempCurrent;
        if (m_currentAngle < 0)
            tempCurrent = -m_currentAngle;
        else
            tempCurrent = m_currentAngle;

        float difference;
        if (tempCurrent > tempTarget)
            difference = tempCurrent - tempTarget;
        else
            difference = tempTarget - tempCurrent;
        // this curve will let the modle rotate fast at the start and slow down once it gets to it max turn
        float lerpSpeed = m_modelRotationCurve.Evaluate(difference);

        // this is similar to the ship turn lerp but its for the ship model to swing from side to side depending on which direction you are turning
        m_shipAngle = Mathf.Lerp(Mathf.Clamp(m_shipAngle, -35, 35), (m_currentAngle * 2f) * Mathf.Rad2Deg, lerpSpeed);

        // first it will look at facing position which in the empty object infront of the ship
        transform.LookAt(facingPoint, transform.up);

        // Rotate the ship to the normal of the track
        transform.rotation = Quaternion.FromToRotation(transform.up, m_currentPos) * transform.rotation;
    }

    /// <summary>
    /// Downforce keeps the car perpendicular to the track as well as add downforce so it stays on the track
    /// </summary>
    private void DownForce()
    {
        // raycasting to find the track and its normal.
        // raycasts from a 
        RaycastHit hit1;
        if (Physics.Raycast(transform.position, -rayCastPoint.up, out hit1))
        {
            Debug.DrawRay(transform.position, transform.position - facingPoint.position);
        
            if (hit1.transform.tag == "Road")
            {
                m_targetPos = hit1.normal;
        
                m_currentPos.x = Mathf.Lerp(m_currentPos.x, m_targetPos.x, m_shipAdjustSpeed);
                m_currentPos.y = Mathf.Lerp(m_currentPos.y, m_targetPos.y, m_shipAdjustSpeed);
                m_currentPos.z = Mathf.Lerp(m_currentPos.z, m_targetPos.z, m_shipAdjustSpeed);
        
                if (hit1.distance < 1f)
                    m_rb.AddForce(transform.up * 4000, ForceMode.Force);
            }
        
        }

        if (hit1.distance > 1.5f)
            m_rb.AddForce(-transform.up * VariantObject.DownForce, ForceMode.Force);
    }

    /// <summary>
    /// this is only for the reset box thats used outside the tunnel incase the player comes out upside down
    /// </summary>
    /// <param name="pointOfCast"></param>
    public void SetRotationToTrack(Transform pointOfCast)
    {
        RaycastHit hit;
        if (Physics.Raycast(pointOfCast.position, -pointOfCast.up, out hit))
        {
            if (hit.transform.tag == "Road")
            {
                m_targetPos = hit.normal;
            }
        }

        Debug.DrawLine(pointOfCast.position, hit.point);

        m_currentPos.z = Mathf.Lerp(m_currentPos.z, m_targetPos.z, 0.01f);
    }

    /// <summary>
    /// This is only for the boost pad. its called from when you hit the boost pad
    /// </summary>
    /// <param name="force"> How strong the boost is </param>
    public void BoostPadBoost(float force)
    {
        PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
        if (PAC != null)
        {
            PAC.SetBoostPitch(1, 1.0f);
            PAC.PlayBoostAudio(3);
            PAC.PlayBoostAudio(0);
            PAC.PlayBoostAudio(1);
            PAC.PlayBoostAudio(2);
        }
        m_isBoostingOnBoostPad = true;
        m_rb.AddForce(transform.forward * force, ForceMode.VelocityChange);
        if (m_fire.Count > 0)
            SwitchFire();
        StartCoroutine(StopBoosting());
    }

    IEnumerator StopBoosting()
    {
        yield return new WaitForSeconds(2f);

        m_isBoostingOnBoostPad = false;
    }

    /// <summary>
    /// When the player hits boost you get the first jolt of boost which feels fast
    /// </summary>
    private void ShipBoost()
    {
        if (m_boostLevel > 0 && !GameManager.gManager.raceFinished)
        {
            m_isBoosting = true;
            chargeSound0Played = false;
            chargeSound1Played = false;
            chargeSound2Played = false;
            ControllerHaptics cRumble = this.gameObject.GetComponent<ControllerHaptics>();
            PlayerInputScript pInput = this.gameObject.GetComponent<PlayerInputScript>();
            if (cRumble != null && pInput != null)
            {
                switch (m_boostLevel)
                {
                    case 1:
                        {
                            cRumble.RumbleTiming(pInput.GetPlayerGamepad(), 1, 0.3f);
                            break;
                        }
                    case 2:
                        {
                            cRumble.RumbleTiming(pInput.GetPlayerGamepad(), 2, 0.5f);
                            break;
                        }
                    case 3:
                        {
                            cRumble.RumbleTiming(pInput.GetPlayerGamepad(), 3, 0.8f);
                            break;
                        }
                }
            }
            StartCoroutine(ShipBoostAcceleration());
        }
    }
     /// <summary>
     /// after the first jolt the ship will maintain the the speed for a bit depending on the level of boost
     /// </summary>
     /// <returns></returns>
    IEnumerator ShipBoostAcceleration()
    {
        PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
        PAC.PlayBoostAudio(0);
        PAC.PlayBoostAudio(1);
        PAC.PlayBoostAudio(2);

        float t = 0;
        float targetTime = m_boostingTimes[m_boostLevel - 1];

        while (t < targetTime)
        {
            if (test != null)
                test.text = t.ToString();

            t += Time.fixedDeltaTime;

            m_rb.AddForce(transform.forward * m_accelerationForce, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForEndOfFrame();

        m_fire[0].TurnFireOff();
        m_fire[1].TurnFireOff();
        m_fire[2].TurnFireOff();
        m_wantingToBoost = false;
        m_currentBoost = 0f;
        m_boostLevel = 0;

        yield return new WaitForSeconds(1f);
        m_isBoosting = false;
        StopCoroutine(ShipBoostAcceleration());
    }

    /// <summary>
    /// This activates the brake when the multiplier is more than zero
    /// </summary>
    private void Brake()
    {
        float multiplier = VariantObject.breakCurve.Evaluate(m_rb.velocity.magnitude / m_currentMaxSpeed);

        m_acceleration -= m_brakeMultiplier * VariantObject.BreakMultiplier * multiplier * Time.deltaTime;

        PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();
        if (PAC != null)
        {
            // ||------------------------//Ship Breaking Pitch Modulation Equation\\------------------------||
            // || [R = Result] [A = m_breakMultiplier] [B = VariantObject.BreakMultiplier] [C = multiplier] ||
            // ||-------------------------------------------------------------------------------------------||
            // ||                                                                                           ||
            // ||                                 R = ((A x B x C) x 0.64)                                  ||
            // ||                                                                                           ||
            // ||-------------------------------------------------------------------------------------------||

            PAC.UpdateEngineModulations(shipSelected, 2, ((m_brakeMultiplier * VariantObject.BreakMultiplier * multiplier) * 0.8f));
            PAC.UpdateWindVolume(0, (((m_brakeMultiplier * VariantObject.BreakMultiplier * multiplier) * 0.35f) * GameManager.gManager.difficultyChange), ((1.0f) * GameManager.gManager.difficultyChange), false, true, 0.01f);
        }
    }

    /// <summary>
    /// Accelerate is very simple. It basicly makes the car go foward when you press the accelerator and brake when you press the brake
    /// </summary>
    private void Accelerate()
    {
        float speedMultiplier = VariantObject.SpeedCurve.Evaluate(m_rb.velocity.magnitude / m_currentMaxSpeed);
        float accelerationMultiplier = VariantObject.accelerationCurve.Evaluate(m_acceleration / VariantObject.DefaultMaxAcceleration);

        PlayerAudioController PAC = this.GetComponent<PlayerAudioController>();

        if (m_accelerateMultiplier == 0 && m_brakeMultiplier == 0 && !m_isBoosting)
        {
            m_acceleration -= (VariantObject.AccelerationMultiplier * 0.4f) * Time.deltaTime;

            if (PAC != null)
            {
                // Audio Pitch & Volume Modulation
                PAC.UpdateEngineModulations(shipSelected, 1, 0.5f);
                PAC.UpdateWindVolume(0, ((0.38f) * GameManager.gManager.difficultyChange), ((1.8f) * GameManager.gManager.difficultyChange), false, true, 0.01f);
            }
        }
        else
        {
            m_acceleration += VariantObject.AccelerationMultiplier * m_accelerateMultiplier * accelerationMultiplier * Time.deltaTime;

            if (PAC != null)
            {
                // Audio Pitch & Volume Modulation
                PAC.UpdateEngineModulations(shipSelected, 0);
                PAC.UpdateWindVolume(0, ((1.3f) * GameManager.gManager.difficultyChange), ((1.9f) * GameManager.gManager.difficultyChange), true, false);
            }
        }

        if(!m_isBoosting)
            m_acceleration = Mathf.Clamp(m_acceleration, 0, VariantObject.DefaultMaxAcceleration);

        if (float.IsNaN(m_acceleration))
            m_acceleration = 0;
        if (float.IsNaN(speedMultiplier))
            speedMultiplier = 0;

        m_rb.velocity += m_acceleration * speedMultiplier * transform.forward;
    }

    /// <summary>
    /// Turn sounds simple but it doesnt turn the ship. it rotates an position in front of the ship then that becomes the foward direction
    /// </summary>
    private void Turn()
    {
        // this script has a targetAngle which is where the empty position wants to be but to make it smooth it lerps the current pos to the target pos
        m_currentAngle = Mathf.Lerp(m_currentAngle, m_targetAngle, 0.06f);

        // this multiplier changes the turn angle based on how fast you are going. The faster you go the less you turn
        float multiplier = 0.5f;
        if (!m_isBoosting && !m_isBoostingOnBoostPad)
            multiplier = VariantObject.TurnSpeedCurve.Evaluate(m_rb.velocity.magnitude / m_currentMaxSpeed);

        if (float.IsNaN(multiplier))
            multiplier = 0;

        // this rotation is for the turning of the ship which only happens on the ships local y axis
        rotation.localRotation = Quaternion.Euler(new Vector3(0, m_currentAngle * (VariantObject.TurnSpeed * multiplier), 0));

        if (cameraRotationPoint != null)
        {
            // rotates a point that lets the camera rotate but a lot less then the ship
            cameraRotationPoint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -m_shipAngle / m_cameraTurnAngle));
        }
        // this uses the shipAngle lerp to rotate both on the y axis and the z axis
        shipModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -m_shipAngle * 0.8f));
    }

    private void Strafe()
    {
        m_rb.AddForce(transform.right * m_strafeMultiplier * m_strafeStrength, ForceMode.VelocityChange);
    }

    /// <summary>
    /// both of these functions set the multipler when an input is sent
    /// </summary>
    /// <param name="multiplier"></param>
    public void SetSpeedMultiplier(float multiplier) { m_accelerateMultiplier = multiplier; }
    public void SetBrakeMultiplier(float multiplier) { m_brakeMultiplier = multiplier; }
    public void SetTurnMultipliers(float multiplier) { m_turningAngle = multiplier; m_turnFireAngle = multiplier * 2; AddAnglesTogether(m_strafeMultiplier, m_turningAngle); }
    private void AddAnglesTogether(float angle1, float angle2) { m_targetAngle = angle1 + angle2; }
    public void SetStrafeMultiplier(float multiplier) { m_strafeMultiplier = multiplier; m_strafeAnimAngle = multiplier * 2; AddAnglesTogether(m_strafeMultiplier, m_turningAngle); }
    public void WantToBoost() { ShipBoost(); }
}
