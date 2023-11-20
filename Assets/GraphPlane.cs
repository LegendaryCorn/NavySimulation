using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
//using UnityEngine.Experimental.PlayerLoop;
using Color = UnityEngine.Color;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GraphPlane : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;

    List<Vector3> vertices;
    List<int> triangles;

    public Vector2 size;
    public int resolution;
    public SimulatedEntity sEntity;

    void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        size = GraphMgr.inst.size;
        resolution = GraphMgr.inst.resolution;
    }

    // Update is called once per frame
    void Update()
    {
        resolution = Mathf.Clamp(resolution, 0, 50);
        GeneratePlane(size, resolution);
        UpdateHeights();
        AssignMesh();
    }

    void SetSizeAndResolutiion()
    {
        size = GraphMgr.inst.size;
        resolution = GraphMgr.inst.resolution;
    }

    void GeneratePlane(Vector2 size, int resolution)
    {
        vertices = new List<Vector3>();
        float xPerStep = size.x / resolution;
        float yPerStep = size.y / resolution;
        for (int y = 0; y < resolution + 1; y++)
        {
            for (int x = 0; x < resolution + 1; x++)
            {
                vertices.Add(new Vector3(x * xPerStep, 0, y * yPerStep));
            }
        }

        triangles = new List<int>();
        for (int row = 0; row < resolution; row++)
        {
            for (int col = 0; col < resolution; col++)
            {
                int i = (row * resolution) + row + col;

                triangles.Add(i);
                triangles.Add(i + resolution + 1);
                triangles.Add(i + resolution + 2);

                triangles.Add(i);
                triangles.Add(i + resolution + 2);
                triangles.Add(i + 1);

            }
        }
    }

    void AssignMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = ChangeColors();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    void UpdateHeights()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 vertexPos = transform.TransformPoint(vertex);
            vertexPos.y = 0;
            vertex.y = (CalculatePotential(vertexPos, sEntity) / GraphMgr.inst.maxMag) * 400;
            vertices[i] = vertex;
        }
    }

    float CalculatePotential(Vector3 position, SimulatedEntity entity)
    {
        float magnitude;
        Entity381 ent381 = entity.ent;

        if (ent381.ai.commands.Count == 0)
            return 0;

        List<Vector3> potentials = ((Move)ent381.ai.commands[0]).ComputePotentials(position);
        Vector3 attTotal = Vector3.zero;
        Vector3 repTotal = Vector3.zero; 

        int i = 0;
        foreach(Vector3 pot in potentials)
        {
            if (i % 2 == 0)
                attTotal += pot;
            else
                repTotal += pot;
            i++;
        }

        magnitude = Utils.Clamp(repTotal.magnitude - attTotal.magnitude, -GraphMgr.inst.maxMag, GraphMgr.inst.maxMag);

        return magnitude;

    }

    Color[] ChangeColors()
    {
        Color[] colors = new Color[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            colors[i] = Color.Lerp(Color.green, Color.red, (vertices[i].y + 400f) / 800f);
        }

        return colors;
    }
}
