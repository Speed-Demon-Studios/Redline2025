using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nodes : MonoBehaviour
{
    public List<Nodes> nextNode;

    public Vector3 ReturnNodePos() { return transform.position; }

    public float radius;

    private void Awake()
    {
        if (this.CompareTag("StartNode"))
        {
            if(GameManager.gManager != null)
                GameManager.gManager.startNode = this;
        }
    }

    public Vector3 RandomNavSphere(Vector3 origin)
    {
        Vector3 randDirection = Random.insideUnitSphere * radius;

        randDirection += origin;

        return randDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
