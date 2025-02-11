using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(ShipVariant))]
//public class ShipVariantInspector : Editor
//{
//    GameObject Collision;
//    public override void OnInspectorGUI()
//    {
//        EditorGUI.BeginChangeCheck();
//
//        ShipVariant variant = (ShipVariant)target;
//
//        EditorGUILayout.LabelField("Ship Name");
//        EditorGUILayout.TextField(variant.VariantName, GUILayout.Width(150), GUILayout.Height(25));
//
//        GUILayout.Space(15f);
//
//        EditorGUILayout.LabelField("Down Force");
//
//        GUILayout.BeginHorizontal();
//
//        float DownForce = EditorGUILayout.FloatField(variant.DownForce, GUILayout.Width(80), GUILayout.Height(25));
//
//        GUILayout.EndHorizontal();
//
//        GUILayout.Space(15f);
//
//        GUILayout.BeginHorizontal();
//        EditorGUILayout.LabelField("Model", GUILayout.Width(50));
//
//        GUILayout.Space(145f);
//
//        EditorGUILayout.LabelField("Collison", GUILayout.Width(50));
//        GUILayout.EndHorizontal();
//
//        GUILayout.BeginHorizontal();
//
//        GameObject model = (GameObject)EditorGUILayout.ObjectField(variant.model, typeof(GameObject), true, GUILayout.Width(150), GUILayout.Height(25));
//
//        GUILayout.Space(45f);
//        Collision = (GameObject)EditorGUILayout.ObjectField(variant.collision, typeof(GameObject), true, GUILayout.Width(150), GUILayout.Height(25));
//
//        GUILayout.EndHorizontal();
//
//        GUILayout.Space(15f);
//
//        EditorGUILayout.LabelField("Speed Stats");
//
//        GUILayout.BeginHorizontal();
//        EditorGUILayout.LabelField("Default Max Acceleration", GUILayout.Width(160));
//        EditorGUILayout.LabelField("Max Acceleration", GUILayout.Width(180));
//        GUILayout.EndHorizontal();
//
//        GUILayout.BeginHorizontal();
//        float DeafaultMaxAcceleration = EditorGUILayout.FloatField(variant.DefaultMaxAcceleration, GUILayout.Width(80), GUILayout.Height(25));
//        GUILayout.Space(80f);
//        float MaxAcceleration = EditorGUILayout.FloatField(variant.MaxAcceleration, GUILayout.Width(80), GUILayout.Height(25));
//        GUILayout.EndHorizontal();
//        EditorGUILayout.Space();
//
//        GUILayout.BeginHorizontal();
//        EditorGUILayout.LabelField("Default Max Speed", GUILayout.Width(125));
//        EditorGUILayout.LabelField("Acceleration Multiplier", GUILayout.Width(145));
//        EditorGUILayout.LabelField("Break Multiplier", GUILayout.Width(130));
//        GUILayout.EndHorizontal();
//
//        GUILayout.BeginHorizontal();
//        float DefaultMaxSpeed = EditorGUILayout.FloatField(variant.DefaultMaxSpeed, GUILayout.Width(100), GUILayout.Height(25));
//        GUILayout.Space(35f);
//        float AccelerationMultiplier = EditorGUILayout.FloatField(variant.AccelerationMultiplier, GUILayout.Width(100), GUILayout.Height(25));
//        GUILayout.Space(35f);
//        float breakMultiplier = EditorGUILayout.FloatField(variant.BreakMultiplier, GUILayout.Width(100), GUILayout.Height(25));
//        GUILayout.EndHorizontal();
//
//        EditorGUILayout.LabelField("Speed Curve", GUILayout.Width(125));
//
//        AnimationCurve speedCurve = EditorGUILayout.CurveField(variant.SpeedCurve, GUILayout.Height(80));
//
//        GUILayout.Space(5f);
//
//        EditorGUILayout.LabelField("Turn Speed", GUILayout.Width(125));
//
//        float turnSpeed = EditorGUILayout.FloatField(variant.TurnSpeed, GUILayout.Width(100), GUILayout.Height(25));
//
//        EditorGUILayout.LabelField("Turn Speed Curve", GUILayout.Width(125));
//
//        AnimationCurve TurnSpeedCurve = EditorGUILayout.CurveField(variant.TurnSpeedCurve, GUILayout.Height(80));
//
//        GUILayout.Space(10f);
//
//        EditorGUILayout.LabelField("AI Stats");
//
//        GUILayout.BeginHorizontal();
//        EditorGUILayout.LabelField("Distance", GUILayout.Width(125));
//        EditorGUILayout.LabelField("Turn Multiplier", GUILayout.Width(125));
//        GUILayout.EndHorizontal();
//
//        GUILayout.BeginHorizontal();
//        float Distance = EditorGUILayout.FloatField(variant.distance, GUILayout.Width(100), GUILayout.Height(25));
//        GUILayout.Space(35f);
//        float turnMultiplier = EditorGUILayout.FloatField(variant.turnMultiplier, GUILayout.Width(100), GUILayout.Height(25));
//        GUILayout.EndHorizontal();
//
//        EditorGUILayout.LabelField("Needed Speed Curve", GUILayout.Width(125));
//        AnimationCurve neededSpeedCurve = EditorGUILayout.CurveField(variant.NeededSpeedCurve, GUILayout.Height(80));
//
//        if (EditorGUI.EndChangeCheck())
//        {
//            Undo.RecordObject(variant, "Changed Script");
//
//            variant.DownForce = DownForce;
//            variant.model = model;
//            variant.distance = Distance;
//            variant.turnMultiplier = turnMultiplier;
//            variant.SpeedCurve = speedCurve;
//            variant.TurnSpeedCurve = TurnSpeedCurve;
//            variant.AccelerationMultiplier = AccelerationMultiplier;
//            variant.BreakMultiplier = breakMultiplier;
//            variant.DefaultMaxAcceleration = DeafaultMaxAcceleration;
//            variant.TurnSpeed = turnSpeed;
//            variant.NeededSpeedCurve = neededSpeedCurve;
//            variant.DefaultMaxSpeed = DefaultMaxSpeed;
//            variant.MaxAcceleration = MaxAcceleration;
//            variant.collision = Collision;
//        }
//    }
//}
