using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipVariant", menuName = "Ship Variants/Ship Variant", order = 1)]
public class ShipVariant : ScriptableObject
{
    public string VariantName;
    public float DownForce;
    [Space]
    [Header("Ship Model")]
    public GameObject model;
    public GameObject collision;
    public ShipType shipType;
    [Space]
    [Header("Speed Variables")]
    public float DefaultMaxAcceleration;
    public float MaxAcceleration;
    public float DefaultMaxSpeed;
    public float AccelerationMultiplier;
    public float BreakMultiplier;
    public AnimationCurve breakCurve;
    public AnimationCurve accelerationCurve;
    public AnimationCurve SpeedCurve;
    [Space]
    [Header("Turning Variables")]
    public float TurnSpeed;
    public AnimationCurve TurnSpeedCurve;
    [Space]
    [Header("Ai Movement Variables")]
    public AnimationCurve NeededSpeedCurve;
    public float distance;
    public float turnMultiplier;
}
