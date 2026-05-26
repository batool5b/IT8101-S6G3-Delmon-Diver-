using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SimpleWaterWaves : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveHeight = 0.25f;   //how high the waves go
    public float waveSpeed = 1.5f;     //how fast waves move
    public float waveScale = 0.4f;     //distance between waves

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        originalVertices = mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            float wave =
                Mathf.Sin(Time.time * waveSpeed + vertex.x * waveScale) +
                Mathf.Cos(Time.time * waveSpeed + vertex.z * waveScale);

            vertex.y = wave * waveHeight;

            modifiedVertices[i] = vertex;
        }

        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();
    }
}


