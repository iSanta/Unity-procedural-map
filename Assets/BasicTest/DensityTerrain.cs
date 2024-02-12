using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static structures.Structs;

public class DensityTerrain : MonoBehaviour
{

    [SerializeField] private MeshGenerator meshGenerator;
    [SerializeField] private float radio;
    [SerializeField] private Vector3 position;
    [SerializeField] private Texture2D texture;
    [SerializeField] private float textureScale;
    [SerializeField] private Material meshMaterial;

    private float offsetx;
    private float offsety;


    public dotStruct[,,] poinstMatrix;
    private int cubesPerAxis;
    private float[,] noiseTexture;
    private int numChuncks;
    private void Start() {
        numChuncks  = meshGenerator.numChuncks;
        cubesPerAxis = meshGenerator.pointsPerFace;
        noiseGenerator();
        densityGenerator();
    }



    /// <summary>
    /// this function generate the noise texture for the terrain, fill the noiseTexture array with the data necesary to create the mesh,
    /// you can visualize the texture in the inspector, just check the "texture" serialized variable
    /// </summary>
    private void noiseGenerator(){
        offsetx = Random.Range(0f, 10000f);
        offsety = Random.Range(0f, 10000f);
        int numSamples = (cubesPerAxis*numChuncks)+2;
        noiseTexture = new float[numSamples,numSamples];
        Texture2D textureSample = new Texture2D(numSamples,numSamples);
        
        for (int i = 0; i < numSamples; i++)
        {
            for (int j = 0; j < numSamples; j++)
            {
                float xCoord = (float) i / numSamples * textureScale + offsetx;
                float yCoord = (float) j / numSamples * textureScale + offsety;

                float sample = Mathf.PerlinNoise(xCoord,yCoord);
                noiseTexture[i,j] = sample;
                Color color = new Color(sample,sample,sample);
                textureSample.SetPixel(i,j,color);
            }
        }
        textureSample.Apply();
        texture = textureSample;       
    }


    /// <summary>
    /// this function assign all the corner values chunk by chunk, at the end call the functions to generate the mesh
    /// </summary>
    private void densityGenerator(){
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

    /// <summary>
    /// based on the noise texture, this function calcule the correct value for each corner
    /// </summary>
    /// <param name="_x">x component of the corner position in the world</param>
    /// <param name="_y">y component of the corner position in the world</param>
    /// <param name="_z">z component of the corner position in the world</param>
    /// <returns></returns>
    private float calcCornerValues(float _x, float _y, float _z){

        float sampleValue = noiseTexture[(int)_x+1,(int)_z+1];
        float value = (_y+1)/(cubesPerAxis+1);

        if (value < sampleValue)
        {
            return 1f;
        }else{
            return 0f;
        }
    }
}
