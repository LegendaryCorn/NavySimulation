using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using static UnityEditor.PlayerSettings;

//struct to hold individual ship data
public struct ShipData
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 movePosition;
    public float heading;
};

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GraphPlane : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;

    public List<Vector3> vertices;
    List<int> triangles;
    List<ShipData> allShipData; //list for individual ship data
    public ComputeShader potentialShader; //compute shader used for calculation
    public Vector2 size;
    public int resolution;
    public SimulatedEntity sEntity; //entity that the graph is calculating for
    bool independent;

    //CPU threading parameters
    private Thread graphThread;
    Mutex mutex = new Mutex();
    Vector3[] worldVertices;
    bool read = true;

    void Awake()
    {
        //Create and set mesh
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        //Change mesh format, this needs to be done for graphs with large resolutions
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //Set mesh size and resolutions
        size = GraphMgr.inst.size;
        resolution = GraphMgr.inst.resolution;

        //Creates and sets triangles and vertices of the mesh
        GeneratePlane(size, resolution);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        allShipData = new List<ShipData>();

        independent = GraphMgr.inst.nonFollowing;
    }

    // Start is called before the first frame update
    void Start()
    {
        size = GraphMgr.inst.size;
        resolution = GraphMgr.inst.resolution;
        if (sEntity == null)
        {
            if(SelectionMgr.inst.selectedEntity != null)
                sEntity = SelectionMgr.inst.selectedEntity;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (independent)
            sEntity = SelectionMgr.inst.selectedEntity;
        if (sEntity != null)
            UpdateHeights(sEntity.ent);

        UpdateMesh();
        /*
        if (read)
        {
            worldVertices = vertices.ToArray();
            transform.TransformPoints(worldVertices); //transforms local mesh coordinates to world cordinates
        }
        lock(worldVertices)
        {
            read = false;
            StartJob();
        }
        if (read)
        {
            transform.InverseTransformPoints(worldVertices); //transforms world coordinates back to local coordinates
            vertices = new List<Vector3>(worldVertices);
        }
        UpdateMesh();
        */

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

    void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }


    void UpdateHeights(Entity381 entity)
    {
        updateShipData();
        ComputeBuffer shipsBuffer = new ComputeBuffer(allShipData.Count, sizeof(float) * 10);
        shipsBuffer.SetData(allShipData);

        //sends world position of mesh vertices to the compute shader via compute buffer
        ComputeBuffer vertexBuffer = new ComputeBuffer(vertices.Count, sizeof(float) * 3);
        Vector3[] worldVert = vertices.ToArray();
        transform.TransformPoints(worldVert); //transforms local mesh coordinates to world cordinates
        vertexBuffer.SetData(worldVert);

        //sends potential parameters to compute shader via compute buffer
        List<PotentialParameters> potParms = new List<PotentialParameters>();
        PotentialParameters pf = entity.gameMgr.aiMgr.potentialParameters;
        potParms.Add(pf);
        ComputeBuffer potentialBuffer = new ComputeBuffer(1, sizeof(float) * 14);
        potentialBuffer.SetData(potParms);

        //sets variables in the compute shader
        potentialShader.SetInt("numShips", entity.gameMgr.entityMgr.entities.Count);
        potentialShader.SetInt("entity", entity.gameMgr.entityMgr.entities.IndexOf(entity));
        potentialShader.SetInt("entCommands", entity.ai.commands.Count);
        potentialShader.SetFloat("maxMag", GraphMgr.inst.maxMag);
        potentialShader.SetBool("calcWaypoint", GraphMgr.inst.calcWaypoint);
        potentialShader.SetBool("calcRepField", GraphMgr.inst.calcRepField);
        potentialShader.SetBool("calcAttField", GraphMgr.inst.calcAttField);
        potentialShader.SetBool("calcCrossPosField", GraphMgr.inst.calcCrossPosField);
        potentialShader.SetBool("calcCrossVelField", GraphMgr.inst.calcCrossVelField);
        potentialShader.SetBool("directional", GraphMgr.inst.directionalRepresentation);
        potentialShader.SetBuffer(0, "ships", shipsBuffer);
        potentialShader.SetBuffer(0, "positions", vertexBuffer);
        potentialShader.SetBuffer(0, "potential", potentialBuffer);

        //starts calculations in buffer
        potentialShader.Dispatch(0, (vertices.Count / 64) + 1, 1, 1);

        //retrieves calculations and sets new vertices
        vertexBuffer.GetData(worldVert);
        transform.InverseTransformPoints(worldVert); //transforms world coordinates back to local coordinates
        vertices = new List<Vector3>(worldVert);

        //disposes buffers after use
        shipsBuffer.Dispose();
        vertexBuffer.Dispose();
        potentialBuffer.Dispose();

        /*for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 vertexPos = transform.TransformPoint(vertex);
            vertexPos.y = 0;
            vertex.y = (CalculatePotential(vertexPos, sEntity) / GraphMgr.inst.maxMag) * 400;
            vertices[i] = vertex;
        }*/
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

        return 0;

    }
    public void updateShipData()
    {
        allShipData = new List<ShipData>();
        foreach (Entity381 entity in sEntity.ent.gameMgr.entityMgr.entities)
        {
            ShipData shipData = new ShipData();
            shipData.position = entity.position;
            shipData.velocity = entity.velocity;
            shipData.heading = entity.heading;
            if (entity.ai.commands.Count != 0)
                shipData.movePosition = entity.ai.moves[0].movePosition;
            else
                shipData.movePosition = Vector3.zero;
            allShipData.Add(shipData);
        }
    }


    void CPUUpdateHeights()
    {
        if (mutex.WaitOne(1000))
        {
            try
            {
                for (int i = 0; i < worldVertices.Length; i++)
                {
                    List<Vector3> potentials = new List<Vector3>();
                    PotentialParameters pf = sEntity.ent.gameMgr.aiMgr.potentialParameters;

                    potentials.Add(Vector3.zero);
                    for (int j = 0; j < 5; j++)
                    {
                        potentials.Add(Vector3.zero); // i % 2 == 0 for positive potentials, i % 2 == 1 for negative potentials
                    }

                    Vector3 attractiveDist = sEntity.ent.ai.moves[0].movePosition - worldVertices[i];
                    Vector3 attractivePotential = attractiveDist.normalized *
                        pf.waypointCoefficient * Mathf.Pow(attractiveDist.magnitude, pf.waypointExponent);
                    potentials[0] += attractivePotential;

                    foreach (Entity381 ent in sEntity.ent.gameMgr.entityMgr.entities)
                    {
                        if (ent == sEntity.ent) continue;

                        Vector3 posDiff = ent.position - worldVertices[i];
                        Vector3 posDiffCross = Vector3.Cross(posDiff, Vector3.down);
                        Vector3 relVel = ent.velocity - sEntity.ent.velocity;
                        Vector3 relVelCross = Vector3.Cross(relVel, Vector3.up);

                        Vector3 starboard = Vector3.Normalize(Vector3.Cross(sEntity.ent.velocity, Vector3.down));

                        float relBearing = Mathf.Atan2((ent.position - sEntity.ent.position).x, (ent.position - sEntity.ent.position).z) * Mathf.Rad2Deg - sEntity.ent.heading;
                        float targetAngle = Mathf.Atan2((sEntity.ent.position - ent.position).x, (sEntity.ent.position - ent.position).z) * Mathf.Rad2Deg - ent.heading;

                        float bAngle = Mathf.Sin((relBearing + pf.bearingAngle) * Mathf.Deg2Rad);
                        float tAngle = Mathf.Sin((targetAngle + pf.taAngle) * Mathf.Deg2Rad);

                        Vector3 repField = Mathf.Pow(posDiff.magnitude, pf.repulsiveExponent) * pf.repulsiveCoefficient * -posDiff.normalized;
                        Vector3 attField = Mathf.Pow(posDiff.magnitude, pf.attractiveExponent) * pf.attractiveCoefficient * posDiff.normalized;
                        Vector3 crossPosField = Mathf.Pow(0.5f * (bAngle + 1), pf.bearingAngleExp) * Mathf.Pow(posDiff.magnitude, pf.bearingExponent) * pf.bearingCoefficient * starboard;
                        Vector3 crossVelField = Mathf.Pow(0.5f * (tAngle + 1), pf.taAngleExp) * Mathf.Pow(posDiff.magnitude, pf.taExponent) * pf.taCoefficient * starboard;


                        potentials[1] += repField;
                        potentials[2] += attField;
                        potentials[3] += crossPosField;
                        potentials[4] += crossVelField;
                    }


                    float magnitude = -potentials[0].magnitude + potentials[1].magnitude - potentials[2].magnitude - potentials[3].magnitude - potentials[4].magnitude;
                    worldVertices[i].y = (Mathf.Clamp(magnitude, -GraphMgr.inst.maxMag, GraphMgr.inst.maxMag) / GraphMgr.inst.maxMag) * 400;
                }
            }
            finally 
            {
                read = true;
                mutex.ReleaseMutex(); 
            }
        }
        
    }
    void StartJob()
    {
        graphThread = new Thread(CPUUpdateHeights);
        graphThread.Start();
    }


}
