using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public List<GameObject> tracks = new();
    public List<GameObject> ships = new();
    private int m_shipIndex;
    private int m_trackIndex;
    private GameObject m_currentTrack;

    private bool m_hasFaded;
    public Animator anim;
    private float m_trackPos = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_currentTrack = tracks[m_trackIndex];
        ships[m_shipIndex].SetActive(true);
        m_trackPos = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_trackPos += 0.1f * Time.deltaTime;
        if(m_trackPos > 0.97f && !m_hasFaded)
        {
            anim.SetTrigger("0");
            m_hasFaded = true;
        }
        if(m_trackPos > 1)
        {
            m_currentTrack.gameObject.SetActive(false);
            m_trackIndex += 1;
            if(m_trackIndex > tracks.Count - 1)
            {
                ships[m_shipIndex].SetActive(false);
                m_shipIndex += 1;
                if(m_shipIndex > ships.Count - 1)
                {
                    m_shipIndex = 0;
                }
                ships[m_shipIndex].SetActive(true);
                m_trackIndex = 0;
            }
            m_trackPos = 0;
            m_currentTrack = tracks[m_trackIndex];
            m_currentTrack.gameObject.SetActive(true);
            m_hasFaded = false;
        }

        m_currentTrack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = m_trackPos;
    }
}
