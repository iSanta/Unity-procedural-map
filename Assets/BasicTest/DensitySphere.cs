using UnityEngine;
using static structures.Structs;

public class DensitySphere : MonoBehaviour
{
    [SerializeField] private MeshGenerator meshGenerator;
    [SerializeField] private float radio;
    [SerializeField] private Vector3 position;


    public dotStruct[,,] poinstMatrix;
    private int cubesPerAxis;
    private void Start() {
        densityGenerator();
    }

    private void densityGenerator(){

        int numChuncks = meshGenerator.numChuncks;
        cubesPerAxis = meshGenerator.pointsPerFace;

        poinstMatrix = new dotStruct[cubesPerAxis,cubesPerAxis,cubesPerAxis];

        int index = 0;
        for (int x = 0; x < numChuncks; x++)
        {
            for (int y = 0; y < numChuncks; y++)
            {
                Vector3 initPos = new Vector3(x * cubesPerAxis, 0, y * cubesPerAxis);



                for (int i = 0; i < meshGenerator.pointsPerFace; i++)
                {
                    for (int j = 0; j < meshGenerator.pointsPerFace; j++)
                    {
                        for (int k = 0; k < meshGenerator.pointsPerFace; k++)
                        {
                            poinstMatrix[i,j,k] = new dotStruct(i,j,k, new float[8]);
                            for (int corner = 0; corner < 8; corner++)
                            {
                                poinstMatrix[i,j,k].Corners[corner].w = calcCornerValues(poinstMatrix[i,j,k].Corners[corner].x +initPos.x,poinstMatrix[i,j,k].Corners[corner].y + initPos.y,poinstMatrix[i,j,k].Corners[corner].z +initPos.z);
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

    private float calcCornerValues(float _x, float _y, float _z){
        Vector3 newVec = new Vector3(_x, _y, _z) - position;
        float magnitude = Vector3.Magnitude(newVec);
        if (magnitude < radio)
        {
            return 1f;
        }else{
            return 0f;
        }
    }
}
