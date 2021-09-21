using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize;
    [Range(-10f, 10f)]
    public float xheight;
    public int zSize;
    [Range(-10f, 10f)]
    public float zHeight;
    [Range(1f, 100f)]
    public float smoothness = 1;
    [Range(1f, 100f)]
    public float NoiseMultiplier = 1f;

    private MeshCollider collider;

    private void Awake()
    {
        collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }
    private void OnValidate()
    {
        Initialize();
        CreateShape();
        UpdateMesh();
        Physics.BakeMesh(mesh.GetInstanceID(), true);
    }
    void Initialize()
    {
        mesh = new Mesh();
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((x * xheight) / smoothness , (z * zHeight) / smoothness) * NoiseMultiplier;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void FixedUpdate()
    {
        collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }
}
