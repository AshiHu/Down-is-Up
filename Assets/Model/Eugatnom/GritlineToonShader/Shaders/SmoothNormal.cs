using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class SmoothNormals : MonoBehaviour
{
    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = Instantiate(meshFilter.mesh); // copie pour ne pas modifier l'original

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector3[] smoothNormals = new Vector3[vertices.Length];

        Dictionary<Vector3, Vector3> normalMap = new Dictionary<Vector3, Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (!normalMap.ContainsKey(vertices[i]))
                normalMap[vertices[i]] = normals[i];
            else
                normalMap[vertices[i]] += normals[i];
        }

        for (int i = 0; i < vertices.Length; i++)
            smoothNormals[i] = normalMap[vertices[i]].normalized;

        // Encode dans les vertex colors (R=X, G=Y, B=Z)
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(
                smoothNormals[i].x * 0.5f + 0.5f,
                smoothNormals[i].y * 0.5f + 0.5f,
                smoothNormals[i].z * 0.5f + 0.5f
            );
        }

        mesh.colors = colors;
        meshFilter.mesh = mesh;
    }
}