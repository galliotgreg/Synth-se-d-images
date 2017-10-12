using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace SynthDImageTP1___CSharp
{

    public struct Points
    {
        public string X;
        public string Y;
        public string Z;
        public string Type;
    }

    public struct Face
    {
        public string A;
        public string B;
        public string C;
        public string D;

        public string Type;
    }

    public class Model
    {
        string fileName = "";
        public ArrayList arrMesh = new ArrayList();

        public Model(string fileName)
        {
            this.fileName = fileName;

            string readfile = "";
            StreamReader reader = new StreamReader(fileName);
            readfile = reader.ReadLine();

            while (readfile != null)
            {
                string[] splitvalue = readfile.Split(' ');

                if (readfile == "")
                {
                    readfile = reader.ReadLine();
                    continue;
                }

                if (readfile.Contains("# object"))
                {
                    string[] splitvalue2 = readfile.Split(' ');
                    Mesh m1 = new Mesh(splitvalue2[2]);
                    arrMesh.Add(m1);

                }

                else if (splitvalue.Length > 6 || splitvalue.Length < 4 || readfile[0] == '#')
                {
                    readfile = reader.ReadLine();
                    continue;
                }

                Points vCoord = new Points();
                Face fCoord = new Face();

                if (splitvalue.Length == 4)
                {
                    vCoord.Type = splitvalue[0];
                    vCoord.X = splitvalue[1];
                    vCoord.Y = splitvalue[2];
                    vCoord.Z = splitvalue[3];
                }

                else if (splitvalue.Length == 5)
                {
                    vCoord.Type = splitvalue[0];
                    vCoord.X = splitvalue[2];
                    vCoord.Y = splitvalue[3];
                    vCoord.Z = splitvalue[4];
                }

                if (splitvalue[0] == "v" || splitvalue[0] == "vn" || splitvalue[0] == "vt")
                {
                    ((Mesh)arrMesh[arrMesh.Count - 1]).addPoints(splitvalue[0], vCoord);
                }

                else if (splitvalue[0] == "f")
                {
                    if (splitvalue.Length == 5)
                    {
                        fCoord.Type = splitvalue[0];
                        fCoord.A = splitvalue[1];
                        fCoord.B = splitvalue[2];
                        fCoord.C = splitvalue[3];
                    }
                    else if (splitvalue.Length == 6)
                    {

                        fCoord.Type = splitvalue[0];
                        fCoord.A = splitvalue[1];
                        fCoord.B = splitvalue[2];
                        fCoord.C = splitvalue[3];
                        fCoord.D = splitvalue[4];
                    }
                    ((Mesh)arrMesh[arrMesh.Count - 1]).AddFace(fCoord);
                }
                readfile = reader.ReadLine();
            }
        }
    }

    class Mesh
    {
        ArrayList v = new ArrayList();
        ArrayList vn = new ArrayList();
        ArrayList f = new ArrayList();
        ArrayList vt = new ArrayList();

        public int getCount(string type)
        {
            switch (type)
            {
                case "v": return v.Count;
                case "vn": return vn.Count;
                case "vt": return vt.Count;
                case "f": return f.Count;
                default: return 0;
            }
        }

        public string meshName = "";

        public Mesh(string meshName)
        {
            this.meshName = meshName;
        }

        public void addPoints(string type, Points Val)
        {
            switch (type)
            {
                case "v": v.Add(Val); break;
                case "vn": vn.Add(Val); break;
                case "vt": vt.Add(Val); break;
            }
        }

        public Face getFace(int index)
        {
            return (Face)f[index];
        }

        public void setFace(int index, Face bigF)
        {
            f[index] = bigF;
        }

        // Added by Hanna
        public void AddFace(Face shi)
        {
            f.Add(shi);
        }

        public vec3 this[int index, string type]
        {
            get
            {
                switch (type)
                {
                    case "v": return new vec3(float.Parse(((Points)v[index]).X), float.Parse(((Points)v[index]).Y), float.Parse(((Points)v[index]).Z));
                    case "vn": return new vec3(float.Parse(((Points)vn[index]).X), float.Parse(((Points)vn[index]).Y), float.Parse(((Points)vn[index]).Z));
                    case "vt": return new vec3(float.Parse(((Points)vt[index]).X), float.Parse(((Points)vt[index]).Y), float.Parse(((Points)vt[index]).Z));
                    default: return new vec3();
                }
            }
            set
            {
                switch (type)
                {
                    case "v": v[index] = value; break;
                    case "vn": vn[index] = value; break;
                    case "vt": vt[index] = value; break;
                }
            }
        }
    }
}
