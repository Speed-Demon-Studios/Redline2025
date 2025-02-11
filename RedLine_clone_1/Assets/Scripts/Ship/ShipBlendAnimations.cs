using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public enum ShipType
{
    Splitwing,
    Cutlass,
    Fulcrum
}
public class ShipBlendAnimations : MonoBehaviour
{
    private ShipsControls m_controls;
    private Animator m_controller;
    private ShipType shipType;
    private List<VisualEffect> eSteeringEffects;

    //cutlass wings float
    float m_currentWingPos;
    float m_targetWingPos;
    public float wingSpeed;

    //Engine
    float m_currentEnginePos;
    float m_targetEnginePos;
    public float engineSpeed;

    //Flaps
    float m_currentFlapPos;
    float m_targetFlapPos;
    public float flapSpeed;

    // Start is called before the first frame update
    public void Inistialize()
    {
        m_controls = GetComponent<ShipsControls>();
        FindEveryChild(m_controls.shipModel.transform);
        shipType = GetComponent<ShipsControls>().VariantObject.shipType;
        eSteeringEffects = GetComponent<ShipsControls>().shipModel.transform.GetComponentInChildren<ShipTypeInfo>().eSteering;
    }

    /// <summary>
    /// Finds every child in the parent and if the parent has children then go through all of them
    /// </summary>
    /// <param name="parent"></param>
    public void FindEveryChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ShipTypeInfo outPut;
            if (child.TryGetComponent<ShipTypeInfo>(out outPut))
            {
                m_controller = outPut.animator;
                return;
            }
            else if(child.childCount > 0)
                FindEveryChild(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.enabled && m_controller != null) // check for null refreneces so there are no errors
        {
            m_targetEnginePos = m_controls.GetAccelerationMultiplier;
            m_currentEnginePos = Mathf.Lerp(m_currentEnginePos, m_targetEnginePos, engineSpeed);
            m_controller.SetFloat("Engine", m_currentEnginePos); // blend engine to speed

            m_targetFlapPos = m_controls.StrafeAnimAngle;
            m_currentFlapPos = Mathf.Lerp(m_currentFlapPos, m_targetFlapPos, flapSpeed);
            m_controller.SetFloat("Flaps", m_currentFlapPos); // blend wing to the direction of turning

            if (shipType == ShipType.Cutlass)
            {
                if (m_controls.RB.velocity.magnitude > 0.5f && m_controls.GetBrakeMultiplier == 0)
                {
                    m_targetWingPos = m_controls.GetAccelerationMultiplier;
                    m_currentWingPos = Mathf.Lerp(m_currentWingPos, m_targetWingPos, wingSpeed);
                    m_controller.SetFloat("WingRight", m_currentWingPos);
                    m_controller.SetFloat("WingLeft", m_currentWingPos);
                }
                else if (m_controls.GetBrakeMultiplier != 0) // if the ship is braking         
                {
                    m_targetWingPos = -m_controls.GetBrakeMultiplier;
                    m_currentWingPos = Mathf.Lerp(m_currentWingPos, m_targetWingPos, wingSpeed);
                    m_controller.SetFloat("WingRight", m_currentWingPos);
                    m_controller.SetFloat("WingLeft", m_currentWingPos);
                }
                else // if the ship is at a stop and not doing anything  
                {                          
                    m_targetWingPos = 0;
                    m_currentWingPos = Mathf.Lerp(m_currentWingPos, m_targetWingPos, wingSpeed);
                    m_controller.SetFloat("WingRight", m_currentWingPos);                              
                    m_controller.SetFloat("WingLeft", m_currentWingPos);                               
                }
            }
            else if(shipType == ShipType.Splitwing)
            {
                m_controller.SetFloat("WingRight", m_currentFlapPos);
            }

            if(m_controls.GetTurnMultiplier > 0)
            {
                eSteeringEffects[0].SetFloat("Particle Alpha", m_controls.TurnFireAngle);
                eSteeringEffects[1].SetFloat("Particle Alpha", 0);
            }
            else
            {
                eSteeringEffects[1].SetFloat("Particle Alpha", -m_controls.TurnFireAngle);
                eSteeringEffects[0].SetFloat("Particle Alpha", 0);
            }
        }
        
    }
}
