using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static structures.Structs;

public class MeshGenerator : MarchTables
{
 [SerializeField] private GameObject redererObj;
    public int pointsPerFace = 30;
    public int numChuncks = 4;
    private float size = structures.Structs.size;

    [SerializeField] private float surfaceLevel = 0.5f;
    [SerializeField] private bool ShowGrid = false;
    [SerializeField] private Material DotMaterial;
    [SerializeField] private Material triangleMaterial;
    [SerializeField] private GameObject ChunkPrefab;




    private Vector3[] vertices;
    private int[] meshTriangles;
    private Mesh[] newMesh;
    public GameObject[] chunks;
    

   


    public dotStruct[,,] poinstMatrix;

    private GameObject[] allDots;



    /// <summary>
    /// asign sizes of arrays based on pointsPerFace and create chunk prefabs, at the end set the position of each chink only in x and z axis 
    /// </summary>
    private void Awake() {
        //poinstMatrix = new dotStruct[pointsPerFace, pointsPerFace, pointsPerFace];
        allDots = new GameObject[pointsPerFace*pointsPerFace*pointsPerFace];
        newMesh = new Mesh[numChuncks*numChuncks];
        chunks = new GameObject[numChuncks*numChuncks];
        GameObject chunkInstance;

        for (int i = 0; i < numChuncks*numChuncks; i++)
        {
                newMesh[i] = new Mesh();
                chunkInstance = Instantiate(ChunkPrefab) as GameObject;
				chunkInstance.name = i + "- Chunk";
                chunks[i] = chunkInstance;
                chunks[i].GetComponent<Chunk>().SetPointsPerFace(pointsPerFace);
                Destroy(chunks[i].GetComponent<MeshCollider>());
                chunks[i].transform.parent = redererObj.transform;
        }
        int index = 0;
        for (int i = 0; i < numChuncks; i++)
        {
            for (int j = 0; j < numChuncks; j++)
            {
                chunks[index].transform.position = new Vector3(i * pointsPerFace, 0, j * pointsPerFace);
                index++;
            }
        }
        
    }


    










    /// <summary>
    /// each cube have corners, depending of the value of that corner (w component) the algoritm generate triangles, there are like 256 difrent posible configuration for each cube
    /// the number of triangles and positions depends on the corners values, the int cubeIndex variable store the index of the configuration for each cube, this index is for the triangulation matrix on MarchTables.cs
    /// the triangulation matrix is fill of int arrays who describe each posible configuration (256 in total)
    /// those values are just the edges of a cube, for example if a cube have the configuration 2 ({ 0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }) this means that this cube only have one triangule, and the corners of that triangle are the edges 0 , 1 and 9 of our cube
    /// each 3 values on the int arrays are difent triangles, all the -1 values are just empty space
    /// 
    /// at the end of this function vertices and meshTriangles variables get fill with all the necesary information correctly ordered to create a mesh later
    /// </summary>
    /// <param name="_iteration">the lenght of the matrix, i put it like a paramether because some times i just want to see some cubes and not all the matrix</param>

    public void createTriangles(){
        if(ShowGrid){
            makePointsVisibles(pointsPerFace*pointsPerFace*pointsPerFace);
        }
        int numTris = 0;
        int index = 0;
        List<Triangle> AllTriangles = new List<Triangle>();
        foreach (var item in poinstMatrix)
        {            
            int cubeIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (item.Corners[i].w < surfaceLevel)
                {
                    cubeIndex |= 1<<i;
                    //Debug.Log($"({item.Corners[i].x}, {item.Corners[i].y}, {item.Corners[i].z})");
                }
            }
            //Debug.Log($"cubeIndex: {cubeIndex}" );
            int n = 16;
            int numVetices = 0;
            int[] chosen = new int[n] ;
            
            
            
            for (int i = 0; i < n; i++)
            {
                chosen[i] = triangulation[cubeIndex, i];
                if (triangulation[cubeIndex, i] != -1)
                {
                    numVetices++;
                }
            }   

            
            for (int i = 0; chosen[i] != -1; i+=3)
            {
                //Debug.Log(chosen[i]);
                numTris++;
                int a0 = cornerIndexAFromEdge[chosen[i]];
                int b0 = cornerIndexBFromEdge[chosen[i]];

                int a1 = cornerIndexAFromEdge[chosen[i+1]];
                int b1 = cornerIndexBFromEdge[chosen[i+1]];

                int a2 = cornerIndexAFromEdge[chosen[i+2]];
                int b2 = cornerIndexBFromEdge[chosen[i+2]];

                Triangle tri;
                tri = new Triangle(interpolateVerts(item.Corners[a0], item.Corners[b0]), interpolateVerts(item.Corners[a1], item.Corners[b1]), interpolateVerts(item.Corners[a2], item.Corners[b2]));
                AllTriangles.Add(tri);
            }


            index++;
        }
        int trianglesCounter = 0;
        
        vertices = new Vector3[numTris * 3];
        meshTriangles = new int[numTris * 3];
        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = AllTriangles[i].IndivTriangle[j];
                
                trianglesCounter++;
            }
            //Debug.Log(vertices[i*3]);
            
        }

        //createMesh();
    }
    Vector3 interpolateVerts(Vector4 v1, Vector4 v2) {
        float t = (surfaceLevel - v1.w) / (v2.w - v1.w);
        return v1 + t * (v2-v1);
    }


    /// <summary>
    /// this function create the mesh, clear previus information, fill the mesh with the new vertices and triangles information and asign it to a game object
    /// </summary>
    public void createMesh(int _chunk){

            newMesh[_chunk].Clear();
            Destroy(chunks[_chunk].GetComponent<MeshCollider>());
            newMesh[_chunk].vertices = vertices;
            
            newMesh[_chunk].triangles = meshTriangles;
            newMesh[_chunk].RecalculateNormals ();


            

            newMesh[_chunk].colors = MehsColor();


            chunks[_chunk].GetComponent<MeshFilter>().mesh = newMesh[_chunk];
            chunks[_chunk].AddComponent<MeshCollider>();
        
    }


    /// <summary>
    /// This function set the colors of the mesh based on how hight in y axis each vertex is, using the array of colors in some cases it make an interpolation between colors for make the transition more soft
    /// </summary>
    /// <returns></returns>
    private Color[] MehsColor(){
        Color[] terrainColors = {
            new Color(0.549f,0.435f,0.254f,1),
            new Color(0.309f,0.568f,0.254f,1),
            new Color(0.819f,0.780f,0.584f,1),
            new Color(0.172f,0.552f,0.717f,1),
            new Color(0.145f,0.192f,0.6f,1),
        };

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++){
            if ((vertices[i].y/pointsPerFace)<=1f && (vertices[i].y/pointsPerFace) >0.8f)
                {
                    colors[i]= terrainColors[0];
                }
                else if((vertices[i].y/pointsPerFace)<=0.8f && (vertices[i].y/pointsPerFace) >0.7f)
                {
                    float power = ((vertices[i].y/pointsPerFace)-0.7f)*10;
                    colors[i] =  (terrainColors[0]*power) + (terrainColors[1]*(1-power));
                }
                else if((vertices[i].y/pointsPerFace)<=0.7f && (vertices[i].y/pointsPerFace) >0.45f)
                {
                    colors[i]= terrainColors[1];
                }
                else if((vertices[i].y/pointsPerFace)<=0.45f && (vertices[i].y/pointsPerFace) >0.35f)
                {
                    float power = ((vertices[i].y/pointsPerFace)-0.35f)*10;
                    colors[i] =  (terrainColors[1]*power) + (terrainColors[2]*(1-power));
                }
                else if((vertices[i].y/pointsPerFace)<=0.35f && (vertices[i].y/pointsPerFace) >0.3f)
                {
                    colors[i]= terrainColors[2];
                }
                else if((vertices[i].y/pointsPerFace)<=0.3f && (vertices[i].y/pointsPerFace) >0.2f)
                {
                    colors[i]= terrainColors[3];  
                }
                else if((vertices[i].y/pointsPerFace)<=0.2f && (vertices[i].y/pointsPerFace) >0.1f)
                {
                    float power = ((vertices[i].y/pointsPerFace)-0.1f)*10;
                    colors[i] =  (terrainColors[3]*power) + (terrainColors[4]*(1-power));
                }
                else if((vertices[i].y/pointsPerFace)<=0.1f && (vertices[i].y/pointsPerFace) >0.0f)
                {
                    colors[i]= terrainColors[4];
                }
        }


        return colors;
    }

 



    /// <summary>
    /// WARNING: this function execute if you mark ShowGrid in the inspecto, if you do that, make sure that the pointsPerFace variable not have a hight value, i usualy use this with a value like 10
    /// otherwise this will blow up your pc
    /// 
    /// this function is to visualy see the cubes and corners in the algoritm
    /// </summary>
    /// <param name="_iteration"></param>
    private void makePointsVisibles(int _iteration){
        Vector3 sizeDot = new Vector3(size,size,size);
        Vector3 sizeCorner = new Vector3(0.1f,0.1f,0.1f);

        int indexPoints = 0;
        foreach (var item in poinstMatrix)
        {
            if (indexPoints >= _iteration) break;
            
            allDots[indexPoints] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            allDots[indexPoints].transform.localScale = sizeDot;
            allDots[indexPoints].transform.position = new Vector3(item.x, item.y, item.z);
            allDots[indexPoints].GetComponent<MeshRenderer>().material = DotMaterial;

            float opacityCorner = 0.8f;
            GameObject[] cornersOfCube = new GameObject[8];
            for (int i = 0; i < 8; i++)
            {
                //if (item.Corners[i].w < surfaceLevel)
                //{
                    cornersOfCube[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornersOfCube[i].transform.parent = allDots[indexPoints].transform;
                    cornersOfCube[i].transform.localScale = sizeCorner;
                    cornersOfCube[i].transform.position = item.Corners[i];
                    Material newMaterial = new Material(DotMaterial);
                    newMaterial.color = new Color(item.Corners[i].w,item.Corners[i].w,item.Corners[i].w, opacityCorner);
                    cornersOfCube[i].GetComponent<MeshRenderer>().material = newMaterial;  
                //}
                   
            }


            indexPoints++;
        }
    }


    /// <summary>
    /// this is just an example of how to create meshes on unity using vertices positions and triangles
    /// triangles is just an array of int who describe the order of each vertice
    /// in this example we have 2 triangles, the firts one with the corners 0,1,2, and the second with the corners 3,4,5 this values are the index of the verticeesArr
    /// </summary>
    private void createSimpleTriangle(){
        Mesh newMesh = new Mesh();

        Vector3[] verticesArr = { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(0,1,0.5f), new Vector3(0,0,0), new Vector3(0,1,0.5f), new Vector3(0,1,1f),};
        newMesh.vertices  = verticesArr;

        int[] trianglesArr = {0,1,2,3,4,5};
        newMesh.triangles = trianglesArr;

        redererObj.GetComponent<MeshFilter>().mesh = newMesh;
    }


}
