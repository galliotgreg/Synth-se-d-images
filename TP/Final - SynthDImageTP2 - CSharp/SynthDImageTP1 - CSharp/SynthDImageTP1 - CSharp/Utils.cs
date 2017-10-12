using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SynthDImageTP1___CSharp
{
    class Utils
    {
        public static float Clamp(float val, float min, float max)
        {
            return (val < min ? min : (val > max ? max : val));
        }

        public static int Clamp(int val, int min, int max)
        {
            return (val < min ? min : (val > max ? max : val));
        }

        public static void Save_Img(int width, int height, vec3[] colors)
        {
            StreamWriter W = new StreamWriter("./TP01_" + DateTime.Now.ToString().Replace(":", "-").Replace("/", "-").Replace("\\", "-") + ".ppm");
            
            W.WriteLine("P3 ");
            W.WriteLine(width.ToString() + " " + height.ToString() + " ");
            W.WriteLine("255 ");
            for (int i = 0; i < colors.Length; i++)
            {
                W.WriteLine(Clamp(colors[i].x * 255, 0, 255).ToString() + " " + Clamp(colors[i].y * 255, 0, 255).ToString() + " " + Clamp(colors[i].z * 255, 0, 255).ToString() + " ");
            }
            W.Close();
        }
    }
}
