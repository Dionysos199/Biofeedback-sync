
 using UnityEngine;
using UnityEngine.UI;
using Uduino;
using UnityEngine.InputSystem;
using System;
//using System.Collections;

public class wobbelySphere : MonoBehaviour
{
    public int numLongitudes = 24;
    public int numLatitudes = 12;
    public float radius = 1f;
    public Vector3 center = Vector3.zero;
   

    private float scale;

    [SerializeField]
    private Material _material;

    public Slider scaleSlider;


    public float _data;
    public float inputMin = 950;
    public float inputMax = 980;
    float outputMin = 0;
    float outputMax = 5;

    void Awake()
    {
        UduinoManager.Instance.OnDataReceived += createVertices ; //Create the Delegate
    }
    Vector3[] verticesArray;

    SignalProcessor processor = new SignalProcessor(bufferSize: 50);
    void createVertices(string data, UduinoDevice device)
    {
        int reading = int.Parse(data);
        processor.AddValue(reading);

        // Process reading
        float scale = _data = processor.GetNormalized();

        verticesArray = new Vector3[(numLongitudes + 1) * (numLatitudes + 1)];

        for (int lat = 0; lat <= numLatitudes; lat++)
        {
            float theta = lat * Mathf.PI / numLatitudes;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);
            
            for (int lon = 0; lon <= numLongitudes; lon++)
            {
                float phi = lon * 2f * Mathf.PI / numLongitudes;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                Vector3 vertex = new Vector3(cosPhi * sinTheta, cosTheta, sinPhi * sinTheta);
                float xCoord = (float)lon / (float)numLongitudes;
                float yCoord = (float)lat / (float)numLatitudes;

                verticesArray[lat * (numLongitudes + 1) + lon] = center + 10 * radius * vertex;
                radius = Mathf.PerlinNoise(scale * sinTheta * (float)lat / numLatitudes, scale * cosTheta * (float)lat / numLatitudes);

            }
        }
        generateMesh();
    }

    public void generateMesh()
    {

        MeshFilter _meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        _meshFilter.mesh = mesh;
        MeshRenderer _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _material;



        mesh.vertices = verticesArray;

        int[] triangles = new int[numLongitudes * numLatitudes * 6];

        for (int lat = 0; lat < numLatitudes; lat++)
        {
            for (int lon = 0; lon < numLongitudes; lon++)
            {
                int currentVertex = lat * (numLongitudes + 1) + lon;
                int nextVertex = currentVertex + numLongitudes + 1;

                triangles[6 * (lat * numLongitudes + lon) + 0] = currentVertex;
                triangles[6 * (lat * numLongitudes + lon) + 1] = nextVertex + 1;
                triangles[6 * (lat * numLongitudes + lon) + 2] = currentVertex + 1;
                triangles[6 * (lat * numLongitudes + lon) + 3] = currentVertex;
                triangles[6 * (lat * numLongitudes + lon) + 4] = nextVertex;
                triangles[6 * (lat * numLongitudes + lon) + 5] = nextVertex + 1;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}