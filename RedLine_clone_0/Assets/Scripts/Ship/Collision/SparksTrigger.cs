using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class SparksTrigger : MonoBehaviour
{
    [SerializeField] private SparksParticlesController m_spc;

    public VisualEffect[] sparks;
    public bool isColliding = false;
    public bool waiting = false;

    public void SetSPC(SparksParticlesController spcScript)
    {
        m_spc = spcScript;
    }

    public SparksParticlesController GetSPC()
    {
        return m_spc;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.ToLower() == "walls")
        {
            isColliding = true;
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag.ToLower() == "walls")
        {
            isColliding = false;
        }
    }


}
