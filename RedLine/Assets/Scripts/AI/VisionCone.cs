using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisionCone : MonoBehaviour
{
    public float distance;
    public float angle;
    public float height;
    public Color meshColor;
    public LayerMask things;
    public LayerMask obsicles;
    public List<GameObject> objects;
    public GameObject parent;

    Collider[] colliders = new Collider[50];
    int count;
    private Mesh _coneMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scan();
    }

    public void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, things, QueryTriggerInteraction.Collide);

        objects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (obj != parent)
            {
                if (IsInSight(obj))
                {
                    objects.Add(obj);
                }
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if(direction.y < 0 || direction.y > height)
        {
            return false;
        }

        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if(deltaAngle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if(Physics.Linecast(origin, dest, obsicles))
        {
            return false;
        }

        return true;
    }

    Mesh CreateMesh()
    {
        Mesh mesh = new();

        int segments = 15;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 topCenter = bottomCenter + Vector3.up * height;

        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        float cureentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for(int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, cureentAngle, 0) * Vector3.forward * distance;
            topLeft = bottomLeft + Vector3.up * height;

            bottomRight = Quaternion.Euler(0, cureentAngle + deltaAngle, 0) * Vector3.forward * distance;
            topRight = bottomRight + Vector3.up * height;
            // bottom tri
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            // top tri
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // front tris
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            cureentAngle += deltaAngle;
        }

        // left tris
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right tris
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;



        for(int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    private void OnValidate()
    {
        _coneMesh = CreateMesh();
    }

    private void OnDrawGizmos()
    {       
        if (_coneMesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(_coneMesh, transform.position, transform.rotation);
        }
        //
        ////Gizmos.DrawWireSphere(transform.position, distance);
        //
        //Gizmos.color = Color.red;
        //for (int i = 0; i < count; i++)
        //{
        //    Gizmos.DrawSphere(colliders[i].transform.position, 2f);
        //}

        Gizmos.color = Color.green;
        foreach(var obj in objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 2f);
        }
    }
}
