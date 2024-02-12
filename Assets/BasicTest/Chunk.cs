using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static structures.Structs;


public class Chunk : MonoBehaviour
{
    private int pointsPerFace;
    public dotStruct[,,] poinstMatrix;

    /// <summary>
    /// Gizmos to see the size of the cube matrix
    /// </summary>
    void OnDrawGizmos(){
        float size = structures.Structs.size;
        Vector3 pos = new Vector3((pointsPerFace*size/2)-(size/2),(pointsPerFace*size/2)-(size/2),(pointsPerFace*size/2)-(size/2));
        Vector3 initialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        pos = pos + initialPos;
        Gizmos.DrawWireCube(pos, Vector3.one * size * pointsPerFace );
        //transform.position = Vector3.zero;
    }


    /// <summary>
    /// set the number of cubes in each face of the matrix
    /// </summary>
    /// <param name="_points">number of cubes in each face </param>
    public void SetPointsPerFace(int _points){
        pointsPerFace = _points;
    }

    /// <summary>
    /// copy the chuck data matrix of this especific chunk, this is used when the user modify the terrain
    /// </summary>
    /// <param name="_data">new data for this chunk</param>
    public void SetChunkData(dotStruct[,,] _data){
        poinstMatrix = new dotStruct[pointsPerFace,pointsPerFace,pointsPerFace];
        for (int i = 0; i < pointsPerFace; i++)
        {
            for (int j = 0; j < pointsPerFace; j++)
            {
                for (int k = 0; k < pointsPerFace; k++)
                {
                    poinstMatrix[i, j, k] = _data[i, j, k];
                }
                
            }
        }
        
        
    }

    /// <summary>
    /// get chunk data
    /// </summary>
    /// <returns></returns>
    public dotStruct[,,] getChunkData(){
        return poinstMatrix;
    }
}
