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
    public bool entSpecific;
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
            if (!entSpecific)
                vertex.y = (CalculatePotential(vertexPos) / GraphMgr.inst.maxMag) * 400f;
            else
                vertex.y = (CalculatePotential(vertexPos, sEntity) / GraphMgr.inst.maxMag) * 400;
            vertices[i] = vertex;
        }
    }

    float CalculatePotential(Vector3 position)
    {

        Vector3 repulsivePotential;
        float magnitude;
        repulsivePotential = Vector3.zero;

        foreach (Entity381 ent in EntityMgr.inst.entities)
        {
            foreach (Vector3 fp in ent.fieldPos)
            {
                Vector3 diff = fp - position;
                float dist = diff.magnitude;
                Vector3 direc = diff.normalized;

                if (!ent.underway)
                {
                    if (dist < 1000)
                    {
                        repulsivePotential += direc * ent.mass / 40 * ent.length / 20 *
                           ent.repulsiveCoefficient / ent.numFields * Mathf.Pow(dist, ent.repulsiveExponent);
                    }
                }
                else
                {
                    if (dist < 1000)
                    {
                        if (fp != ent.front)
                        {
                            repulsivePotential += direc * ent.mass / 40 * ent.length / 20 *
                                ent.repulsiveCoefficient / ent.numFields * Mathf.Pow(dist, ent.repulsiveExponent);
                        }
                        else
                        {
                            repulsivePotential += direc * ent.mass / 40 * ent.length / 20 *
                                ent.repulsiveCoefficient / 2 * Mathf.Pow(dist, ent.repulsiveExponent);
                        }
                    }
                }
            }
        }

        magnitude = Utils.Clamp(repulsivePotential.magnitude, 0, GraphMgr.inst.maxMag);

        return magnitude;
    }

    float CalculatePotential(Vector3 position, Entity381 entity)
    {

        Vector3 repulsivePotential;
        Vector3 attractivePotential = Vector3.zero;
        float magnitude;
        float potentialMag;

        Potential p;
        repulsivePotential = Vector3.zero;
        foreach (Entity381 ent in EntityMgr.inst.entities)
        {
            if (ent == entity) continue;
            p = DistanceMgr.inst.GetPotential(entity, ent);
            foreach (Vector3 fp in ent.fieldPos)
            {
                float coeff = SituationCases(p);

                Vector3 diff = fp - position;
                float dist = diff.magnitude;
                Vector3 direc = diff.normalized;

                if (!ent.underway)
                {
                    if (dist < 1000)
                    {
                        repulsivePotential += direc * ent.mass / 40 * ent.length / 20 *
                           ent.repulsiveCoefficient / ent.numFields * Mathf.Pow(dist, ent.repulsiveExponent);
                    }
                }
                else
                {
                    if (dist < 1000)
                    {
                        if (fp != ent.front)
                        {
                            repulsivePotential += coeff * /*(0.3f * Mathf.Cos(p.targetAngle * Mathf.Deg2Rad) + 1) **/ direc * ent.mass / 40 * ent.length / 20 *
                                ent.repulsiveCoefficient / ent.numFields * Mathf.Pow(dist, ent.repulsiveExponent);
                        }
                        else
                        {
                            repulsivePotential += coeff * /*(0.3f * Mathf.Cos(p.targetAngle * Mathf.Deg2Rad) + 1) **/ direc * ent.mass / 40 * ent.length / 20 *
                                ent.repulsiveCoefficient / 2 * Mathf.Pow(dist, ent.repulsiveExponent);
                        }
                    }
                }
            }

        }

        if (entity.transform.GetComponent<UnitAI>().commands.Count != 0)
        {
            attractivePotential = entity.transform.GetComponent<UnitAI>().commands[0].movePosition - position;
            Vector3 tmp = attractivePotential.normalized;
            attractivePotential = tmp *
                entity.attractionCoefficient * Mathf.Pow(attractivePotential.magnitude, entity.attractiveExponent);
        }

        potentialMag = repulsivePotential.magnitude - attractivePotential.magnitude;

        magnitude = Utils.Clamp(potentialMag, -GraphMgr.inst.maxMag, GraphMgr.inst.maxMag);

        return magnitude;

    }

    float SituationCases(Potential p)
    {
        float coeff = 1;

        if (Utils.isBetween(90, 180, p.relativeBearingDegrees) && Utils.isBetween(270, 0, p.targetAngle))
            coeff = 0;
        if (Utils.isBetween(210, 310, p.relativeBearingDegrees) && Utils.isBetween(300, 40, p.targetAngle))
            coeff = 0;
        if (p.cpaInfo.time == 0)
            coeff = 0;

        return coeff;
    }

    Color[] ChangeColors()
    {
        Color[] colors = new Color[vertices.Count];
        if (entSpecific)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                colors[i] = Color.Lerp(Color.green, Color.red, (vertices[i].y + 400f) / 800f);
            }
        }
        else
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                colors[i] = Color.Lerp(Color.green, Color.red, vertices[i].y / 400f);
            }
        }


        return colors;
    }
}
