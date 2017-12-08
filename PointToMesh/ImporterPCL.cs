using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PointToMesh
{
    public class ImporterPCL
    {
        private static readonly float RADIUS = 0.3f;
        private delegate void MeshCreation(MeshData data, ref System.Text.StringBuilder builder, ref Bitmap outTex);
        private static MeshCreation creationMethod = new MeshCreation(TriangleCreator);
        private static Dictionary<UnityEngine.Color, Vector2> vcToTextureMap = new Dictionary<UnityEngine.Color, Vector2>();
        private static int vcMapEmptyPixelIndex =0;

        public static string Export( MeshData data, string path)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(ExportHeader(data));
            sb.Append(ExportVertices(data, path));

            //Clear texture stuff
            vcMapEmptyPixelIndex = 0;
            vcToTextureMap.Clear();

            return sb.ToString();

        }

        private static string ExportVertices(MeshData data, string path)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int halfTexSize = (int)Math.Ceiling(data.UniqueColors * 0.5f);
            Bitmap outText = new Bitmap(halfTexSize,halfTexSize);
            //Texture2D outText = new Texture2D(halfTexSize, halfTexSize, TextureFormat.ARGB32, false);
            //Texture2D outText = null;
            creationMethod(data, ref sb, ref outText);
            outText.Save(path.Replace(".ply",".png"));

            //NOTE: Stringbuilder can run out of memory due to it's internal variables, may need to put checks in place for this in the future and make a stringbuilder list
            return sb.ToString();
        }

        private static void TriangleCreator(MeshData data, ref System.Text.StringBuilder sb, ref Bitmap outText)
        {
            

            for (int i = 0; i < data.vertices.Length; i++)
            {
                Vector3 center = data.vertices[i];
                Vector3 normal = data.normals[i];
                if (normal == Vector3.zero)
                    normal = Vector3.right;
                UnityEngine.Color rgb = data.colors[i];
                Vector3 tangent = Vector3.Normalize(Vector3.Cross(Vector3.up, normal));
                Vector3 up = Vector3.Normalize(Vector3.Cross(tangent, normal));

                Vector3 p1 = center + tangent * -RADIUS / 1.5f;
                Vector3 p2 = center + up * RADIUS;
                Vector3 p3 = center + tangent * RADIUS / 1.5f;

                Vector2 uv = FindUV(rgb, ref outText);

                sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p1.x, p1.y, p1.z, normal.x, normal.y, normal.z, uv.x, uv.y, rgb.r, rgb.g, rgb.b);
                sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p2.x, p2.y, p2.z, normal.x, normal.y, normal.z, uv.x, uv.y, rgb.r, rgb.g, rgb.b);
                sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p3.x, p3.y, p3.z, normal.x, normal.y, normal.z, uv.x, uv.y, rgb.r, rgb.g, rgb.b);

            }

            for (int i = 0; i < data.vertices.Length; i++)
            {
                sb.AppendLine("3 " + (i * 3) + " " + (i * 3 + 1) + " " + (i * 3 + 2));
            }
        }

        private static Vector2 FindUV(UnityEngine.Color rgb, ref Bitmap outTexture)
        {
            if (vcToTextureMap.ContainsKey(rgb))
            {
                return vcToTextureMap[rgb];
            }
            else
            {
                //Issues:
                // pixel coordinates in 0-width range, also start at top left
                // uv coordinates in 0-1 range, start at bottom left

                //Get uv coords in pixel range (0-width/height)
                Vector2 uvCoords = new Vector2(vcMapEmptyPixelIndex % outTexture.Size.Width, vcMapEmptyPixelIndex / outTexture.Size.Width);
                outTexture.SetPixel((int)uvCoords.x, (int)uvCoords.y, System.Drawing.Color.FromArgb((int)(rgb.r), (int)(rgb.g), (int)(rgb.b)));

                //Convert to 0-1 for uv coordinates
                uvCoords.x /= outTexture.Size.Width;
                uvCoords.y = 1- uvCoords.y /outTexture.Size.Height; //flip y to convert to pixel coordinate system

                vcToTextureMap.Add(rgb, uvCoords);
                vcMapEmptyPixelIndex++;
                return uvCoords;
            }
        }

        /* private static void PyramidCreator(MeshData data, ref System.Text.StringBuilder sb)
{
    for (int i = 0; i < data.vertices.Length; i++)
    {
        Vector3 center = data.vertices[i];
        Vector3 normal = data.normals[i];
        if (normal == Vector3.zero)
            normal = Vector3.right;
        Color rgb = data.colors[i];
        Vector3 tangent = Vector3.Normalize(Vector3.Cross(Vector3.up, normal));
        Vector3 up = Vector3.Normalize(Vector3.Cross(tangent, normal));

        Vector3 p1 = center + tangent * -RADIUS / 1.5f;
        Vector3 p2 = center + up * RADIUS;
        Vector3 p3 = center + tangent * RADIUS / 1.5f;
        sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p1.x, p1.y, p1.z, normal.x, normal.y, normal.z, 0.0, 0.0, rgb.r, rgb.g, rgb.b);
        sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p2.x, p2.y, p2.z, normal.x, normal.y, normal.z, 0.0, 0.0, rgb.r, rgb.g, rgb.b);
        sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p3.x, p3.y, p3.z, normal.x, normal.y, normal.z, 0.0, 0.0, rgb.r, rgb.g, rgb.b);
        sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}" + System.Environment.NewLine, p3.x, p3.y, p3.z, normal.x, normal.y, normal.z, 0.0, 0.0, rgb.r, rgb.g, rgb.b);

    }

    for (int i = 0; i < data.vertices.Length; i++)
    {
        sb.AppendLine("3 " + (i * 3) + " " + (i * 3 + 1) + " " + (i * 3 + 2));
    }
}*/


        private static string ExportHeader(MeshData data)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("ply");
            sb.AppendLine("format ascii 1.0");
            sb.AppendLine("comment Created by Glynn Taylor's Point Cloud to Mesh Cloud application");
            sb.AppendLine("element vertex " + data.vertices.Length * 3);
            sb.AppendLine("property float x");
            sb.AppendLine("property float y");
            sb.AppendLine("property float z");
            sb.AppendLine("property float nx");
            sb.AppendLine("property float ny");
            sb.AppendLine("property float nz");
            sb.AppendLine("property float s");
            sb.AppendLine("property float t");
            sb.AppendLine("property uchar red");
            sb.AppendLine("property uchar green");
            sb.AppendLine("property uchar blue");
            sb.AppendLine("element face " + data.vertices.Length);
            sb.AppendLine("property list uchar uint vertex_indices");
            sb.AppendLine("end_header");
            return sb.ToString();
        }

        public static MeshData Load(string path)
        {
            MeshData data = new MeshData();
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string inputLine = "";
                    bool header = true;
                    int propertyOffset = 0;

                    DataEntry normalCount = new DataEntry(), rgbCount = new DataEntry(), uvCount = new DataEntry();
                    int vertexIndex = 0;

                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        inputLine = inputLine.Trim();
                        if (header)
                        {
                            header = ProcessHeader(data, inputLine, ref propertyOffset, ref normalCount, ref rgbCount, ref uvCount);
                        }
                        else
                        {
                            if (inputLine.Split(' ').Length >= 3 + normalCount.Length + rgbCount.Length + uvCount.Length)
                            {
                                ProcessVertex(data, inputLine, vertexIndex, normalCount, rgbCount, uvCount);
                                vertexIndex++;
                            }
                        }
                    }
                    data.CalculateDistinct();
                    reader.Close();
                }
            }
            return data;
        }

        private static void ProcessVertex(MeshData data, string inputLine, int vertexIndex, DataEntry normalCount, DataEntry rgbCount, DataEntry uvCount)
        {
            List<float> row = inputLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToList();
            Vector3 xyz = new Vector3(row[0], row[1], row[2]);
            Vector3 normal = default(Vector3);
            Vector2 uv = default(Vector2);
            UnityEngine.Color rgb = default(UnityEngine.Color);
            //normal
            if (normalCount.Length > 0)
            {
                normal = new Vector3(row[normalCount.offset], row[normalCount.offset + 1], row[normalCount.offset + 2]);
            }
            //uv
            if (uvCount.Length > 0)
            {
                uv = new Vector2(row[uvCount.offset], row[uvCount.offset + 1]);
            }
            //rgb
            if (rgbCount.Length > 0)
            {
                rgb = new UnityEngine.Color(row[rgbCount.offset], row[rgbCount.offset + 1], row[rgbCount.offset + 2], rgbCount.Length > 3 ? row[rgbCount.offset + 3] : 1);
            }
            data.setVertex(vertexIndex, xyz, normal, uv, rgb);
        }

        private static bool ProcessHeader(MeshData data, string inputLine, ref int propertyOffset, ref DataEntry normalCount, ref DataEntry rgbCount, ref DataEntry uvCount)
        {
            if (inputLine.Contains("end_header"))
            {
                return false;
            }
            else
            {

                if (inputLine.Contains("element vertex"))
                {
                    data.SetLength(Int32.Parse(inputLine.Split(' ')[2]));
                }
                else if (inputLine.Equals("property uchar red"))
                {
                    if (rgbCount.Length == 0)
                        rgbCount.Length = 3;
                    rgbCount.offset = propertyOffset;
                }
                else if (inputLine.Equals("property uchar alpha"))
                {
                    rgbCount.Length = 4;
                }
                else if (inputLine.Equals("property float nx"))
                {
                    normalCount.Length = 3;
                    normalCount.offset = propertyOffset;
                }
                else if (inputLine.Equals("property float s"))
                {
                    uvCount.Length = 2;
                    uvCount.offset = propertyOffset;
                }
                if (inputLine.Contains("property"))
                {
                    propertyOffset++;
                }
            }
            return true;
        }
    }
}
