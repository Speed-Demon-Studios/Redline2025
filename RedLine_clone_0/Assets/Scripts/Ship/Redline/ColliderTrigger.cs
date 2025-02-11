using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    public RedlineColliderSpawner spawner;
    public GameObject ship;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.ToLower() == "racer")
        {
            Debug.Log(other.gameObject);
            ShipsControls test;
            if (other.gameObject.TryGetComponent<ShipsControls>(out test) && other.gameObject != ship)
                spawner.m_allShipsInColliders.Add(test.gameObject);
            else if(other.gameObject != ship)
            {
                spawner.m_allShipsInColliders.Add(other.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.ToLower() == "racer")
        {
            Debug.Log(other.gameObject);
            ShipsControls test;
            if (other.gameObject.TryGetComponent<ShipsControls>(out test) && other.gameObject != spawner.gameObject)
                spawner.m_allShipsInColliders.Remove(test.gameObject);
            else if(other.gameObject != ship)
            {
                spawner.m_allShipsInColliders.Remove(other.transform.parent.gameObject);
            }
        }
    }
}
