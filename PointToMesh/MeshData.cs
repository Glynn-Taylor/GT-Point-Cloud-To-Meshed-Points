using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PointToMesh
{
    public class MeshData
    {
        //VERTEX DATA (position, normal, color, uv)
        public Vector3[] vertices;
        public Vector3[] normals;
        public Color[] colors;
        public Vector2[] uvs;
        //FACE DATA
        public int[] indices;
        public int UniqueColors = 0;

        public string Debug(int limit)
        {
            if(vertices == null)
            {
                return "";
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < Math.Min(vertices.Length, limit); i++)
            {
                sb.Append("Pos: "+vertices[i].ToString());
                sb.Append(" Normal: "+normals[i].ToString());
                sb.Append(" RGB: "+colors[i].ToString());
                sb.Append(" UV: "+uvs[i].ToString());
                sb.Append(System.Environment.NewLine);
            }
            return sb.ToString();
        }

        internal void SetLength(int v)
        {
            vertices = new Vector3[v];
            normals = new Vector3[v];
            colors = new Color[v];
            uvs = new Vector2[v];
        }

        internal void setVertex(int vertexIndex, Vector3 xyz, Vector3 normal, Vector2 uv, Color rgb)
        {
           if(vertexIndex < vertices.Length)
            {
                vertices[vertexIndex] = xyz;
                normals[vertexIndex] = normal == null? new Vector3(0,0,0):normal;
                colors[vertexIndex] = rgb == null ? new Color(0, 0, 0,1) : rgb;
                uvs[vertexIndex] = uv == null ? new Vector2(0, 0) : uv;
            }
        }

        internal void CalculateDistinct()
        {
            UniqueColors = colors.Distinct().Count();
        }
    }

    /*public class Vector3
    {
        public float x, y, z;

        public Vector3(float v1, float v2, float v3)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
        }

        public String ToString()
        {
            return String.Concat("(",x,"," ,y,",", z,")");
        }
    }
    public class Vector2
    {
        public float x, y;
        public Vector2(float v1, float v2)
        {
            this.x = v1;
            this.y = v2;
        }
        public String ToString()
        {
            return String.Concat("(", x, ",", y,")");
        }
    }
    public class Color
    {
        public float r,g,b,a;

        public Color(float v1, float v2, float v3, float a)
        {
            this.r = v1;
            this.g = v2;
            this.b = v3;
            this.a = a;
        }
        public String ToString()
        {
            return String.Concat("(", r, ",", g,",",b,")");
        }
    }*/

    public class DataEntry
    {
        public int Length = 0;
        public int offset = 0;
    }
}
