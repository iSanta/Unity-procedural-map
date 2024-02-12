using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace structures
{
    public class Structs : MonoBehaviour
    {
        public static float size = 0.5f;
        private static float distance = 1f * size;
        
        /// <summary>
        /// this vector array is to store the 8 diferent corner positions in a cube based on a pivot in the center of the cube
        /// </summary>
        public static Vector3[] CP ={
            new Vector3(distance,-distance,distance),
            new Vector3(distance,-distance,-distance),
            new Vector3(-distance,-distance,-distance),
            new Vector3(-distance,-distance,distance),
            new Vector3(distance,distance,distance),
            new Vector3(distance,distance,-distance),
            new Vector3(-distance,distance,-distance),
            new Vector3(-distance,distance,distance)
        };

        /// <summary>
        /// this vector array is to store the 12 difrent edge positions in a cube based on a pivot in the center of the cube
        /// </summary>
        public static Vector3[] EP ={
            new Vector3(distance,-distance,0),
            new Vector3(0,-distance,-distance),
            new Vector3(-distance,-distance,0),
            new Vector3(0,-distance,distance),
            new Vector3(distance,distance,0),
            new Vector3(0,distance,-distance),
            new Vector3(-distance,distance,0),
            new Vector3(0,distance,distance),
            new Vector3(distance,0,distance),
            new Vector3(distance,0,-distance),
            new Vector3(-distance,0,-distance),
            new Vector3(-distance,0,distance)
        };


        /// <summary>
        /// this struct is for triangles, each triangle have 3 corners 
        /// </summary>
        public struct Triangle {
            public Vector3[] IndivTriangle;
            public Triangle(Vector3 _a, Vector3 _b,Vector3 _c) : this(){
                IndivTriangle = new Vector3[3];
                IndivTriangle[0] = _a;
                IndivTriangle[1] = _b;
                IndivTriangle[2] = _c;
            }
        };
        

        /// <summary>
        /// this strict have all necesary information for a single cube in a marching cube algoritm
        /// x,y,z variables describe the position of the cube 
        /// Corners array of type Vector4 describe the 8 difrent corners positions of a cube, the w component is for the value 0 to 1, this indicate if the corner is inside of the mesh or not
        /// a cube with 8 corners with w component at 1 means that cube is complete inside of the mesh, 
        /// a cube with 8 corners with w component at 0 means that cube is complete outside of the mesh,
        /// the rest of the cases means that cube is in the surface so it will create triangles of the mesh later
        /// 
        /// in the constructor store the position of the cube and using the arrays of CP (corner points) and EP (edge points) calcule the positions of corners and edges for this specific cube
        /// 
        /// for the w component of the cornesrs, for this case (a sphere) just calcule the distance between each corner to the center of an imaginary sphere, 
        /// if the distance is less than the radius of that sphere means that especific corner is inside of the sphere
        /// 
        /// if you need a diferent shape, for example a terrain, you will need to create a way to asign w corner component a value that you need
        /// </summary>
        public struct dotStruct
        {
            public float x;
            public float y;
            public float z;
            public Vector4[] Corners;

            public Vector3[] Edges;

            public dotStruct(float _x, float _y, float _z, float[] values) : this() {
                x = _x;
                y = _y;
                z = _z;
                Corners = new Vector4[8];
                Edges = new Vector3[12];



                Corners[0] = new Vector4(_x + CP[0].x ,_y + CP[0].y,_z + CP[0].z, values[0]);
                Corners[1] = new Vector4(_x + CP[1].x ,_y + CP[1].y,_z + CP[1].z, values[1]);
                Corners[2] = new Vector4(_x + CP[2].x ,_y + CP[2].y,_z + CP[2].z, values[2]);
                Corners[3] = new Vector4(_x + CP[3].x ,_y + CP[3].y,_z + CP[3].z, values[3]);
                Corners[4] = new Vector4(_x + CP[4].x ,_y + CP[4].y,_z + CP[4].z, values[4]);
                Corners[5] = new Vector4(_x + CP[5].x ,_y + CP[5].y,_z + CP[5].z, values[5]);
                Corners[6] = new Vector4(_x + CP[6].x ,_y + CP[6].y,_z + CP[6].z, values[6]);
                Corners[7] = new Vector4(_x + CP[7].x ,_y + CP[7].y,_z + CP[7].z, values[7]);

                Edges[0] = new Vector3(_x + EP[0].x ,_y + EP[0].y,_z + EP[0].z);
                Edges[1] = new Vector3(_x + EP[1].x ,_y + EP[1].y,_z + EP[1].z);
                Edges[2] = new Vector3(_x + EP[2].x ,_y + EP[2].y,_z + EP[2].z);
                Edges[3] = new Vector3(_x + EP[3].x ,_y + EP[3].y,_z + EP[3].z);
                Edges[4] = new Vector3(_x + EP[4].x ,_y + EP[4].y,_z + EP[4].z);
                Edges[5] = new Vector3(_x + EP[5].x ,_y + EP[5].y,_z + EP[5].z);
                Edges[6] = new Vector3(_x + EP[6].x ,_y + EP[6].y,_z + EP[6].z);
                Edges[7] = new Vector3(_x + EP[7].x ,_y + EP[7].y,_z + EP[7].z);
                Edges[8] = new Vector3(_x + EP[8].x ,_y + EP[8].y,_z + EP[8].z);
                Edges[9] = new Vector3(_x + EP[9].x ,_y + EP[9].y,_z + EP[9].z);
                Edges[10] = new Vector3(_x + EP[10].x ,_y + EP[10].y,_z + EP[10].z);
                Edges[11] = new Vector3(_x + EP[11].x ,_y + EP[11].y,_z + EP[11].z);
            }
            
        }
    }

}
