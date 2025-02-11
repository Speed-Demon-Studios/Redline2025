using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireInfo : MonoBehaviour
{
    [SerializeField] private List<VisualEffect> m_fireEffect = new();
    
    public List<VisualEffect> GetFireList() { return m_fireEffect; }

    public void TurnFireOff()
    {
        foreach(VisualEffect vE in m_fireEffect)
        {
            vE.Stop();
        }
    }

    public void TurnFireOn()
    {
        foreach (VisualEffect vE in m_fireEffect)
        {
            vE.Play();
        }
    }

}
