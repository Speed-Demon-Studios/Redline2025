using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAIFollowCam : MonoBehaviour
{
    public GameObject AIParent;
    private List<GameObject> m_aiships = new();

    public float yAngle;
    private Vector3 average;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < AIParent.transform.childCount; i++)
        {
            m_aiships.Add(AIParent.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempAverage = Vector3.zero;
        for (int i = 0; i < AIParent.transform.childCount; i++)
        {
            tempAverage += m_aiships[i].transform.position;
        }

        average = tempAverage / m_aiships.Count;

        transform.position = average;

        //transform.Rotate(new Vector3(0, yAngle, 0));
    }
}
