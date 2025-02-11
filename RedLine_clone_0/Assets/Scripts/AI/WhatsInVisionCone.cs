using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WhatsInVisionCone : MonoBehaviour
{
    private List<VisionCone> m_visionCons = new();
    public VisionCone frontLeft;
    public VisionCone frontRight;
    public VisionCone backLeft;
    public VisionCone backRight;

    // Start is called before the first frame update
    void Awake()
    {
        m_visionCons.Add(frontLeft);
        m_visionCons.Add(frontRight);
        m_visionCons.Add(backLeft);
        m_visionCons.Add(backRight);
    }

    // Update is called once per frame
    void Update()
    {
       //objects.Clear();
       //foreach(VisionCone cone in m_visionCons)
       //{
       //     foreach(GameObject obj in cone.objects)
       //     {
       //         objects.Add(obj);
       //     }
       //} 
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //foreach (var obj in objects)
        //{
        //    Gizmos.DrawSphere(obj.transform.position, 2f);
        //}
    }
}
