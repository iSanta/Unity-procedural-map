using UnityEngine;
using static structures.Structs;


public class ModifyMesh : MonoBehaviour
{
    [SerializeField] private float ModifyRadius;
    [SerializeField] private MeshGenerator meshGenerator;

    private Vector3 rayPos;
    private int rayType;
    private int pointsPerFace;



    private void Start() {
        pointsPerFace = meshGenerator.pointsPerFace;
    }

    /// <summary>
    /// This recibe position and tipe of ray from the camera and update all value field on each corner of the marching cube algoritm
    /// </summary>
    /// <param name="_position">position vector where the ray impact with the mesh</param>
    /// <param name="_type">type of ray 1 = add  |  2 = remove</param>
    public void rayTerrainModify(Vector3 _position, int _type){
        Debug.Log("Ray");
        rayPos = _position;
        rayType = _type;
        
        int numChuncks = meshGenerator.numChuncks;
        int index = 0;
        for (int x = 0; x < numChuncks; x++)
        {
            for (int y = 0; y < numChuncks; y++)
            {
                Vector3 initPos = new Vector3(x * pointsPerFace, 0, y * pointsPerFace);
                dotStruct[,,] poinstMatrix = meshGenerator.chunks[index].GetComponent<Chunk>().getChunkData();
                //Debug.Log($"x: {poinstMatrix[1,10,1].Corners[0].x}, y: {poinstMatrix[1,10,1].Corners[0].y}, z: {poinstMatrix[1,10,1].Corners[0].z} value: {poinstMatrix[1,10,1].Corners[0].w} ");
                Debug.Log(meshGenerator.chunks[index].name);
                for (int i = 0; i < pointsPerFace; i++)
                {
                    for (int j = 0; j < pointsPerFace; j++)
                    {
                        for (int k = 0; k < pointsPerFace; k++)
                        {
                            for (int corner = 0; corner < 8; corner++)
                            {
                                poinstMatrix[i,j,k].Corners[corner].w = calcDistance(poinstMatrix[i,j,k].Corners[corner].x +initPos.x,poinstMatrix[i,j,k].Corners[corner].y + initPos.y,poinstMatrix[i,j,k].Corners[corner].z +initPos.z, poinstMatrix[i,j,k].Corners[corner].w);
                            }
                        }
                    }
                }

                meshGenerator.poinstMatrix = poinstMatrix;
                meshGenerator.createTriangles();
                meshGenerator.createMesh(index);
                meshGenerator.chunks[index].GetComponent<Chunk>().SetChunkData(poinstMatrix);
                index++;
            }
        } 
    }

    /// <summary>
    /// this calcule distance between each corner and the ray hit point if the distance is less than the radio of an imaginare sphere then change the value of the corner depending of the type of ray
    /// </summary>
    /// <param name="_corner">indiviudal corner of the marching cube algoritm</param>
    /// <returns></returns>
    private float calcDistance(float _x, float _y, float _z, float _w){
        Vector3 newVec = new Vector3(_x, _y, _z) - rayPos;
        float magnitude = Vector3.Magnitude(newVec);
        switch (rayType)
        {
            case 1:
                if (magnitude <= ModifyRadius) return 1;
                else return _w;

            case 2:
                if (magnitude <= ModifyRadius) return 0;
                else return _w;
            default:
                return _w;
        }
        
        
    }
}
