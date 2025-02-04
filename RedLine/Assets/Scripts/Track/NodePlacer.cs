using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamteck.Splines;

public class NodePlacer : MonoBehaviour
{
    public Nodes prefabToSpawn;
    public SplineFollower follower;
    public Nodes prevNode;

    public GameObject nodeParent;
    private int maxIndex;
    private int currentIndex;

    public void SpawnNode()
    {
        //if(prefabToSpawn == null)
        //{
        //    EditorUtility.DisplayDialog("Error", "No prefab to spawn", "OK");
        //    return;
        //}
        //
        //Nodes spawnNode = PrefabUtility.InstantiatePrefab(prefabToSpawn) as Nodes;
        //
        //spawnNode.gameObject.transform.localPosition = this.gameObject.transform.localPosition;
        //spawnNode.gameObject.transform.localRotation = this.gameObject.transform.localRotation;
        //
        //if(prevNode != null)
        //{
        //    prevNode.nextNode.Add(spawnNode);
        //}
        //
        //prevNode = spawnNode;
    
    }

    public void OrderNodes()
    {
       currentIndex = 0;
       GameObject startNode = nodeParent.transform.GetChild(currentIndex).gameObject;
       maxIndex = nodeParent.transform.childCount;
       currentIndex += 1;
       
       //while (currentIndex != maxIndex)
       //{
       //    //GameObject tempNode = startNode.GetComponent<Nodes>().nextNode.gameObject;
       //    tempNode.transform.SetSiblingIndex(currentIndex);
       //
       //    startNode = tempNode;
       //    currentIndex += 1;
       //}
    }
}
