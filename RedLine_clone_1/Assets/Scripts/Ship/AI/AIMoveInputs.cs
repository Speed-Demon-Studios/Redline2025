using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AIMoveInputs : MonoBehaviour
{
    [SerializeField] private ShipVariant Variant;
    private float m_speed;
    private ShipsControls m_controls;
    private Vector3 m_randomPos;
    private float m_firstDistanceToNode;
    private float m_currentDistanceToNode;
    private float m_targetTurnAngle;
    private float m_currentTurnAngle;

    public void SetVariant(ShipVariant variant) { Variant = variant; }

    private Nodes m_prevNode;
    public Nodes desiredNode;
    private Nodes m_nextNode;
    public GameObject nodeParent;
    public TMP_Text test;
    public TMP_Text test2;

    // Start is called before the first frame update
    void Start()
    {
        m_controls = GetComponent<ShipsControls>(); // reference to the shipsControls script 
        m_prevNode = desiredNode;

        if(desiredNode.nextNode.Count > 1) // if the nextNode list count is greater than 1
        {
            int nodeChoice = Random.Range(0, desiredNode.GetComponent<Nodes>().nextNode.Count); // choose a random index from 0 to count
            m_nextNode = desiredNode.nextNode[nodeChoice]; // set nextNode to the nextNode list at index of the random choice
        }
        else // if it only has 1 in the list
        {
            m_nextNode = desiredNode.nextNode[0]; // set next node to that 1
        }

        m_randomPos = desiredNode.RandomNavSphere(desiredNode.transform.position); // randomPos is a random transform inside a radius of the desiredNode

        m_firstDistanceToNode = Vector3.Distance(this.transform.position, m_randomPos); // the distance from the ship to the randomPos

        m_speed = Random.Range(0.6f, 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Accelerate();
        Turning();
    }


    private void Turning()
    {
        // checks if it is at the current node by check the distance
        // if it is at the current node then it will change the current node to the next node
        // if not then it will continue to go to the current node
        if (Vector3.Distance(this.gameObject.transform.position, desiredNode.transform.position) < Variant.distance || Vector3.Distance(this.gameObject.transform.position, m_nextNode.transform.position) < Variant.distance ||
            Vector3.Distance(this.gameObject.transform.position, m_nextNode.transform.position) < Vector3.Distance(this.gameObject.transform.position, m_prevNode.transform.position))                                          
        {
            m_prevNode = desiredNode;
            desiredNode = m_nextNode;
            
            if(desiredNode.nextNode.Count > 1)  
            {
                int nodeChoice = Random.Range(0, desiredNode.nextNode.Count);                  
                m_nextNode = desiredNode.nextNode[nodeChoice];            
                m_randomPos = desiredNode.RandomNavSphere(desiredNode.transform.position);     
            }                          
            else { m_nextNode = desiredNode.nextNode[0]; m_randomPos = desiredNode.RandomNavSphere(desiredNode.transform.position); }                       

            m_firstDistanceToNode = Vector3.Distance(this.transform.position, m_randomPos);
        }            
        m_currentDistanceToNode = Vector3.Distance(this.transform.position, m_randomPos); // get the distance between the ship and the randomPos     
        // this chunk is finding both current nodes up and next nodes up and getting the difference between them
        // this will be used later
        Transform pointA = this.transform; // reference to the transform of the ship
        Transform pointB = m_nextNode.gameObject.transform; // reference to the transform of the nectNode
        Vector3 up = (pointB.up + pointA.up); // add them together to make an up reference

        Vector3 nodeDirection = (transform.position - m_randomPos).normalized; // direction from the forward facing to the randomPos
        Vector3 directionFoward = (transform.position - m_controls.facingPoint.position).normalized; // forward facing direction
        Vector3 nodeDirectionNext = (transform.position - m_nextNode.transform.position).normalized; // direction from forward facing to nextNode

        // finding the angle between the ship and the next node and changing it to radians
        float secondAngle = Vector3.SignedAngle(nodeDirectionNext, directionFoward, up);
        float secondAngleRad = secondAngle * Mathf.Deg2Rad;
        // make the targetTurnAngle equal to the angleRad
        m_targetTurnAngle = secondAngleRad;//target turn angle is used to help with the lerp
        float percentage = CalculatePercentage(); // first we calculate the percentage of how close the ship is to the current node
                                                  // from the prev node. so if its half way between the prev and current it would be 50%
        float turnAnglePer = m_targetTurnAngle * percentage / 100; // now take that percentage and get that percengate of the turn angle

        m_currentTurnAngle = m_targetTurnAngle - turnAnglePer; // take that turn angle away from the tagetAngle to get the currentAngle

        Debug.DrawLine(this.transform.position, desiredNode.transform.position);

        m_controls.SetStrafeMultiplier(-(m_currentTurnAngle * Variant.turnMultiplier));
        m_controls.SetTurnMultipliers(-(m_currentTurnAngle * Variant.turnMultiplier));
    }

    /// <summary>
    /// Calcutation of the percentage for the turn angles
    /// </summary>
    /// <returns></returns>
    private float CalculatePercentage()
    {
        float maxTakeMin = m_firstDistanceToNode - Variant.distance;
        float customPieNumber = 100 / maxTakeMin;
        float differnceOfMaxNX = m_firstDistanceToNode - m_currentDistanceToNode;
        float extraPercentage = maxTakeMin - differnceOfMaxNX;
        float percentage = extraPercentage * customPieNumber;
        return percentage;
    }

    /// <summary>
    /// send through the speed
    /// </summary>
    private void Accelerate()
    {
        m_controls.SetSpeedMultiplier(m_speed);
    }
}
