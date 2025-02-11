using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RedlineColliderSpawner : MonoBehaviour
{
    List<GameObject> m_lineColliders = new();
    private List<GameObject> m_shipsInColliders = new();
    public List<GameObject> GetColliders() { return m_shipsInColliders; }
    public List<GameObject> m_allShipsInColliders;
    private int childIndex;

    public GameObject colliderPrefab;
    public Transform spawnPoint;
    public GameObject colliderParent;

    /// <summary>
    /// Spawns 35 colliders for the redline
    /// </summary>
    public void CallSpawnCollider()
    {
        for (int i = 0; i < 35; i++)
        {
            SpawnCollider();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if the race has not finished then move the positions of the colliders
        if(!GameManager.gManager.raceFinished)
            ChangePositions();
    }

    /// <summary>
    /// clears the list. is only called when the game resets back to menu
    /// </summary>
    public void ClearList()
    {
        m_lineColliders = new List<GameObject> ();
    }

    /// <summary>
    /// changest the position of the back collider so that is at the front
    /// </summary>
    private void ChangePositions()
    {
        // checks for null references so that there are no errors
        if (spawnPoint != null && m_lineColliders[1] != null)
        {
            if (childIndex < m_lineColliders.Count)
            {
                // changes the position of 1 collider which is the childIndex
                m_lineColliders[childIndex].gameObject.transform.position = spawnPoint.transform.position;
                childIndex += 1;
                // if that childIndex gets to the end of the list length then go back to the first one
                if (childIndex > m_lineColliders.Count - 1)
                    childIndex = 0;
            }
        }
    }

    /// <summary>
    /// Spawns a collider at the spawnpoint and then makes the parent null
    /// </summary>
    public void SpawnCollider()
    {
        GameObject a = Instantiate(colliderPrefab, spawnPoint.position, Quaternion.Euler(Vector3.zero), colliderParent.transform.parent.transform);
        a.transform.parent = null;

        m_lineColliders.Add(a);
    }
}
