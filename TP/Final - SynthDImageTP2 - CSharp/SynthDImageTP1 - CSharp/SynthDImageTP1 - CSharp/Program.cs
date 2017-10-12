using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace SynthDImageTP1___CSharp
{
    enum MATERIEL_TYPE { TRANSPARENTE, DIFFUSE, SPECULAIRE };

    struct intersection_data
    {
        public vec3 Position;
        public vec3 Normale;
        public float T;
    }

    class Worker
    {
        [ThreadStatic]
        public vec3[] colors;
        [ThreadStatic]
        public Scene sc1;
        [ThreadStatic]
        public int k;
        [ThreadStatic]
        public int thread_number;

        public void DoWork()
        {
            try
            {
                int compteur_local = 0;
                for (int i = k * (sc1.Width / thread_number); i < (k + 1) * (sc1.Width / thread_number); i++)
                {
                    for (int j = 0; j < sc1.Height; j++)
                    {
                        vec3 direction = new vec3(j - sc1.Width / 2, i - sc1.Height / 2, -sc1.Width / (float)(2 * Math.Tan((sc1.AngleFOV * Math.PI / 180) / 2)));
                        Ray r = new Ray(sc1.Origine, direction);
                        vec3 color = sc1.GetColor(r, 0, 1);

                        lock (sc1.syncLock)
                        {
                            colors[(sc1.Height - i - 1) * sc1.Width + j] = color;
                            sc1.compteurCouleurs++;
                            compteur_local++;

                            if (sc1.compteurCouleurs == sc1.Height * sc1.Width)
                            {
                                Console.WriteLine("fini en " + (DateTime.Now - sc1.time).TotalSeconds);
                                Utils.Save_Img(sc1.Width, sc1.Height, colors);
                            }
                        }
                    }
                }
                //Console.WriteLine("counter=" + sc1.compteurCouleurs + " counter_local=" + compteur_local);
            }
            catch (Exception e)
            {
                Console.WriteLine("e=" + e.StackTrace);
            }
        }
    }

    class PrimitiveBasic
    {
        public virtual intersection_data Intersection(Ray r)
        {
            return new intersection_data();
        }

        public virtual Materiel Materiel
        {
            get
            {
                return new Materiel(MATERIEL_TYPE.DIFFUSE, 1, new vec3());
            }
            set
            {
            }
        }
    }

    class Sphere : PrimitiveBasic
    {
        #region variables
        vec3 _centre;
        float _rayon;
        Materiel _mat;
        #endregion

        #region constructors
        public Sphere(vec3 centre, float rayon, Materiel mat)
        {
            _centre = centre;
            _rayon = rayon;
            _mat = mat;
        }
        #endregion

        #region getters and setters
        public vec3 Centre
        {
            get
            {
                return _centre;
            }
            set
            {
                _centre = value;
            }
        }

        public float Rayon
        {
            get
            {
                return _rayon;
            }
            set
            {
                _rayon = value;
            }
        }

        public override Materiel Materiel
        {
            get
            {
                return _mat;
            }
            set
            {
                _mat = value;
            }
        }
        #endregion

        #region public functions
        public override intersection_data Intersection(Ray r)
        {
            // point intersecrion
            intersection_data id = new intersection_data();

            // vecteur direteur unité du rayon
            vec3 u = vec3.Normalized(r.Direction);

            // sachant que l'eqution est sous la forme: a*t^2 + b*t + c
            float a = 1;
            float b = 2 * vec3.Scalaire(u, r.Origine - Centre);
            float c = vec3.NormeCarre(r.Origine - Centre) - Rayon * Rayon;
            float delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                id.T = -1;
                id.Position = null;
                id.Normale = null;
                return id;
            }
            else
            {
                float t1 = (float)(-b - Math.Sqrt(delta)) / (2 * a);
                float t2 = (float)(-b + Math.Sqrt(delta)) / (2 * a);
                if (t1 < 0 && t2 < 0)
                {
                    id.T = -1;
                    id.Position = null;
                    id.Normale = null;
                    return id;
                }
                else if (t2 > 0 && t1 < 0)
                {
                    id.T = t2;
                    id.Position = r.PointPourT(id.T);
                    id.Normale = vec3.Normalized(id.Position - Centre);
                    return id;
                }
                else if (t1 < t2)
                {
                    id.T = t1;
                    id.Position = r.PointPourT(id.T);
                    id.Normale = vec3.Normalized(id.Position - Centre);
                    return id;
                }
                else
                {
                    id.T = -1;
                    id.Position = null;
                    id.Normale = null;
                    return id;
                }
            }
        }
        #endregion
    }

    class Plan : PrimitiveBasic
    {
        #region variables
        vec3 _point;
        vec3 _normale;
        Materiel _mat;
        #endregion

        #region constructors
        public Plan(vec3 point, vec3 normale, Materiel mat)
        {
            _point = point;
            _normale = normale;
            _mat = mat;
        }
        #endregion

        #region getters and setters
        public vec3 Point
        {
            get
            {
                return _point;
            }
            set
            {
                _point = value;
            }
        }

        public vec3 Normale
        {
            get
            {
                return _normale;
            }
            set
            {
                _normale = value;
            }
        }

        public override Materiel Materiel
        {
            get
            {
                return _mat;
            }
            set
            {
                _mat = value;
            }
        }
        #endregion

        #region public functions
        public override intersection_data Intersection(Ray r)
        {
            // point intersecrion
            intersection_data id = new intersection_data();

            // vecteur direteur unité du rayon
            vec3 u = vec3.Normalized(r.Direction);

            // sachant que l'eqution est sous la forme: a*t = b
            float a = vec3.Scalaire(Normale, r.Direction);
            float b = vec3.Scalaire(Normale, Point - r.Origine);
            if (a == 0)
            {
                id.T = -1;
                id.Position = null;
                id.Normale = null;
                return id;
            }
            else
            {
                id.T = b / a;
                id.Position = r.PointPourT(id.T);
                id.Normale = vec3.Normalized(Normale);
                return id;
            }
        }
        #endregion
    }

    class Triangle : Plan
    {
        #region variables
        vec3 _pointA;
        vec3 _pointB;
        vec3 _pointC;
        Materiel _mat;
        #endregion

        #region constructors
        public Triangle(vec3 pointA, vec3 pointB, vec3 pointC, Materiel mat)
            :base(pointB, vec3.Normalized(vec3.Vectoriel(pointB - pointA, pointC - pointA)), mat)
        {
            _pointA = pointA;
            _pointB = pointB;
            _pointC = pointC;
            _mat = mat;
        }
        #endregion

        #region getters and setters
        public vec3 PointA
        {
            get
            {
                return _pointA;
            }
            set
            {
                _pointA = value;
            }
        }

        public vec3 PointB
        {
            get
            {
                return _pointB;
            }
            set
            {
                _pointB = value;
            }
        }

        public vec3 PointC
        {
            get
            {
                return _pointC;
            }
            set
            {
                _pointC = value;
            }
        }
        #endregion

        #region public functions
        public override intersection_data Intersection(Ray r)
        {
            intersection_data id = base.Intersection(r);
            if (!PointInTriangle(id.Position, PointA, PointB, PointC, Normale))
            {
                id.T = -1;
                id.Position = null;
                id.Normale = null;
            }
            return id;
        }
        #endregion

        #region private functions
        vec3 vect(vec3 p1, vec3 p2, vec3 p3)
        {
            return vec3.Vectoriel(p1 - p3, p2 - p3);
        }

        bool PointInTriangle(vec3 pt, vec3 v1, vec3 v2, vec3 v3, vec3 normale)
        {
            if (pt == null)
            {
                return false;
            }

            vec3 v1v2 = vect(pt, v1, v2);
            vec3 v2v3 = vect(pt, v2, v3);
            vec3 v3v1 = vect(pt, v3, v1);

            return Math.Sign(vec3.Scalaire(normale, v1v2)) == Math.Sign(vec3.Scalaire(normale, v2v3))
                && Math.Sign(vec3.Scalaire(normale, v3v1)) == Math.Sign(vec3.Scalaire(normale, v2v3));
        }
        #endregion
    }

    class Quad : Plan
    {
        #region variables
        vec3 _pointA;
        vec3 _pointB;
        vec3 _pointC;
        vec3 _pointD;
        Materiel _mat;
        #endregion

        #region constructors
        public Quad(vec3 pointA, vec3 pointB, vec3 pointC, vec3 pointD, Materiel mat)
            : base(pointB, vec3.Normalized(vec3.Vectoriel(pointB - pointA, pointC - pointA)), mat)
        {
            _pointA = pointA;
            _pointB = pointB;
            _pointC = pointC;
            _pointD = pointD;
            _mat = mat;
        }
        #endregion

        #region getters and setters
        public vec3 PointA
        {
            get
            {
                return _pointA;
            }
            set
            {
                _pointA = value;
            }
        }

        public vec3 PointB
        {
            get
            {
                return _pointB;
            }
            set
            {
                _pointB = value;
            }
        }

        public vec3 PointC
        {
            get
            {
                return _pointC;
            }
            set
            {
                _pointC = value;
            }
        }

        public vec3 PointD
        {
            get
            {
                return _pointD;
            }
            set
            {
                _pointD = value;
            }
        }
        #endregion

        #region public functions
        public override intersection_data Intersection(Ray r)
        {
            intersection_data id = base.Intersection(r);
            if (!PointInQuad(id.Position, PointA, PointB, PointC, PointD, Normale))
            {
                id.T = -1;
                id.Position = null;
                id.Normale = null;
            }
            return id;
        }
        #endregion

        #region private functions
        vec3 vect(vec3 p1, vec3 p2, vec3 p3)
        {
            return vec3.Vectoriel(p1 - p3, p2 - p3);
        }
        
        bool PointInTriangle(vec3 pt, vec3 v1, vec3 v2, vec3 v3, vec3 normale)
        {
            if (pt == null)
            {
                return false;
            }

            vec3 v1v2 = vect(pt, v1, v2);
            vec3 v2v3 = vect(pt, v2, v3);
            vec3 v3v1 = vect(pt, v3, v1);

            return Math.Sign(vec3.Scalaire(normale, v1v2)) == Math.Sign(vec3.Scalaire(normale, v2v3))
                && Math.Sign(vec3.Scalaire(normale, v3v1)) == Math.Sign(vec3.Scalaire(normale, v2v3));
        }

        bool PointInQuad(vec3 pt, vec3 v1, vec3 v2, vec3 v3, vec3 v4, vec3 normale)
        {
            if (pt == null)
            {
                return false;
            }

            return PointInTriangle(pt, v1, v2, v4, normale)
                || PointInTriangle(pt, v2, v3, v4, normale);
        }
        #endregion
    }
    
    class Materiel
    {
        #region variables
        MATERIEL_TYPE _mat;
        float _indice;
        vec3 _couleur;
        #endregion

        #region constructors
        public Materiel(MATERIEL_TYPE mat, float indice, vec3 couleur)
        {
            _mat = mat;
            _indice = indice;
            _couleur = couleur;
        }
        #endregion

        #region getters and setters
        public float Indice
        {
            get
            {
                return _indice;
            }
            set
            {
                _indice = value;
            }
        }

        public MATERIEL_TYPE Mat
        {
            get
            {
                return _mat;
            }
            set
            {
                _mat = value;
            }
        }

        public vec3 Albedo
        {
            get
            {
                return _couleur;
            }
            set
            {
                _couleur = value;
            }
        }
        #endregion

        #region public functions
        
        #endregion
    }

    class Lumiere
    {
        #region variables
        vec3 _centre;
        float _intensité;
        #endregion
        
        #region constructors
        public Lumiere(vec3 centre, float intensite)
        {
            _centre = centre;
            _intensité = intensite;
        }
        #endregion

        #region getters and setters
        public vec3 Centre
        {
            get
            {
                return _centre;
            }
            set
            {
                _centre = value;
            }
        }

        public float Intensité
        {
            get
            {
                return _intensité;
            }
            set
            {
                _intensité = value;
            }
        }
        #endregion
    }
    
    class Scene
    {
        #region variables
        vec3 _origine;
        float _angleFOV; // en degré
        int _width;
        int _height;
        ArrayList _objets;
        ArrayList _lumieres;

        const int REFLETS_MAX = 10;
        const float REFRAC_AIR = 1;
        const float OFFSET = 0.0005f;

        public DateTime time;
        public int compteurCouleurs = 0;
        public readonly object syncLock = new object();
        #endregion

        #region constructors
        public Scene(vec3 origine, float angleFOV, int width, int height)
        {
            _origine = origine;
            _angleFOV = angleFOV;
            _width = width;
            _height = height;
            _objets = new ArrayList();
            _lumieres = new ArrayList();
        }
        #endregion

        #region getters and setters
        public vec3 Origine
        {
            get
            {
                return _origine;
            }
            set
            {
                _origine = value;
            }
        }
        public ArrayList Lumieres
        {
            get
            {
                return _lumieres;
            }
            set
            {
                _lumieres = value;
            }
        }
        
        public ArrayList Objets
        {
            get
            {
                return _objets;
            }
            set
            {
                _objets = value;
            }
        }

        public float AngleFOV
        {
            get
            {
                return _angleFOV;
            }
            set
            {
                _angleFOV = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        #endregion

        #region indexers
        public object this[int i]
        {
            get
            {
                return (i >= _objets.Count) ? null : _objets[i];
            }
        }
        #endregion

        #region public functions
        public void AddPrimitiveBasic(PrimitiveBasic s)
        {
            _objets.Add(s);
        }

        public void AddLumiere(Lumiere l)
        {
            _lumieres.Add(l);
        }

        public void DrawSphere(Sphere s)
        {
            vec3[] colors = new vec3[Width * Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    vec3 direction = new vec3(j - Width / 2, i - Height / 2, -Width / (float)(2 * Math.Tan((AngleFOV * Math.PI / 180) / 2)));
                    Ray r = new Ray(Origine, direction);
                    intersection_data id = s.Intersection(r);
                    if (id.T >= 0)
                    {
                        colors[i * Width + j] = s.Materiel.Albedo;
                    }
                    else
                    {
                        colors[i * Width + j] = new vec3(); ;
                    }
                }
            }
            Utils.Save_Img(Width, Height, colors);
        }

        public void DrawScene()
        {
            vec3[] colors = new vec3[Width * Height];
            bool threads = true;

            compteurCouleurs = 0;
            // Create the thread object. This does not start the thread.
            time = DateTime.Now;
            if (threads)
            {
                int thread_number = 8;
                for (int k = 0; k < thread_number; k++)
                {
                    // Create the thread object. This does not start the thread.
                    Worker workerObject = new Worker();
                    Thread workerThread = new Thread(workerObject.DoWork);
                    workerObject.k = k;
                    workerObject.thread_number = thread_number;
                    workerObject.sc1 = this;
                    workerObject.colors = colors;

                    // Start the worker thread.
                    workerThread.Start();
                    Thread.BeginThreadAffinity();
                }
            }
            else
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        vec3 direction = new vec3(j - Width / 2, i - Height / 2, -Width / (float)(2 * Math.Tan((AngleFOV * Math.PI / 180) / 2)));
                        Ray r = new Ray(Origine, direction);
                        vec3 color = GetColor(r, 0, 1);
                        colors[(Height - i - 1) * Width + j] = color;
                    }
                }
                Utils.Save_Img(Width, Height, colors);
                Console.WriteLine("fini en " + (DateTime.Now - time).TotalSeconds);
            }
        }

        public vec3 GetColor(Ray r, int compteur_reflets, int inverseur_normale)
        {
            SortedList t_list = new SortedList();
            foreach (PrimitiveBasic s in Objets)
            {
                intersection_data id = s.Intersection(r);
                if (id.T > 0)
                {
                    if (!t_list.ContainsKey(id.T))
                    {
                        t_list.Add(id.T, new object[] { s, id });
                    }
                }
            }
            if (t_list.Count > 0)
            {
                float t_elue = (float)t_list.GetKey(0);
                PrimitiveBasic elue = (PrimitiveBasic)((object[])t_list[t_elue])[0];
                intersection_data id_elue = (intersection_data)((object[])t_list[t_elue])[1];
                vec3 normale = id_elue.Normale;
                vec3 point = id_elue.Position;

                if (elue.Materiel.Mat == MATERIEL_TYPE.SPECULAIRE)
                {
                    compteur_reflets++;
                    if (compteur_reflets >= REFLETS_MAX)
                    {
                        return new vec3();
                    }
                    else
                    {
                        vec3 direction_refl = normale * -2 * vec3.Scalaire(r.Direction, normale) + r.Direction;
                        vec3 point_refl = point + normale * OFFSET;
                        Ray r_refl = new Ray(point_refl, direction_refl);
                        return GetColor(r_refl, compteur_reflets, inverseur_normale);
                    }
                }
                else if (elue.Materiel.Mat == MATERIEL_TYPE.TRANSPARENTE)
                {
                    compteur_reflets++;
                    if (compteur_reflets >= REFLETS_MAX)
                    {
                        return new vec3(1, 1, 1);
                    }
                    else
                    {
                        normale = vec3.Normalized(normale * inverseur_normale);
                        float cosAngle = (float)Math.Abs(vec3.Scalaire(normale, r.Direction) / (vec3.Norme(normale) * vec3.Norme(r.Direction)));

                        float phi = (float)Math.Pow(REFRAC_AIR / elue.Materiel.Indice, inverseur_normale);
                        if (Math.Pow(phi, -1) < Math.Sqrt(1 - cosAngle * cosAngle))
                        {
                            return new vec3();
                        }
                        vec3 direction_refr = r.Direction * phi + normale * (float)(phi * cosAngle - Math.Sqrt(1 - phi * phi * (1 - cosAngle * cosAngle)));

                        vec3 point_refr = point - vec3.Normalized(normale) * OFFSET;
                        Ray r_refr = new Ray(point_refr, direction_refr);
                        return GetColor(r_refr, compteur_reflets, -inverseur_normale);
                    }
                }
                else
                {
                    float intensite_totale = 0;
                    foreach (Lumiere l in Lumieres)
                    {
                        // rayon lumiere
                        vec3 lum_rayon = vec3.Normalized(point - l.Centre);
                        Ray r_lum = new Ray(l.Centre, lum_rayon);

                        SortedList t_list_lum = new SortedList();
                        foreach (PrimitiveBasic s in Objets)
                        {
                            // is not in shadow
                            intersection_data id_lum = s.Intersection(r_lum);
                            if (id_lum.T > 0)
                            {
                                if (!t_list_lum.ContainsKey(id_lum.T))
                                {
                                    t_list_lum.Add(id_lum.T, id_lum.Position);
                                }
                            }
                        }
                        if (t_list_lum.Count > 0 && vec3.Norme(((vec3)t_list_lum[t_list_lum.GetKey(0)]) - point) <= 0.01f)
                        {
                            //float cosAngle = vec3.Scalaire(normale, lum_rayon) / (vec3.Norme(normale) * vec3.Norme(lum_rayon));
                            //intensite_totale += Math.Abs(cosAngle) * l.Intensité;
                            intensite_totale += l.Intensité * Math.Abs(vec3.Scalaire(normale, lum_rayon)) / vec3.NormeCarre(l.Centre - point);
                        }
                    }

                    intensite_totale = Utils.Clamp(intensite_totale, 0, 1);

                    float coul1 = elue.Materiel.Albedo.x * intensite_totale;
                    float coul2 = elue.Materiel.Albedo.y * intensite_totale;
                    float coul3 = elue.Materiel.Albedo.z * intensite_totale;
                    //Console.WriteLine("couleur=" + coul1 + ", " + coul2 + ", " + coul3);
                    //elue.Couleur = new int[]{coul1, coul2, coul3};
                    //Console.WriteLine("couleur=" + elue.Couleur[0] + ", " + elue.Couleur[1] + ", " + elue.Couleur[2]);
                    return new vec3( coul1, coul2, coul3 );
                }
            }
            else
            {
                return new vec3();
            }
        }
        #endregion
    }

    class Ray
    {
        #region variables
        vec3 _origine;
        vec3 _direction;
        #endregion

        #region constructors
        public Ray(vec3 origine, vec3 direction)
        {
            _origine = origine;
            _direction = vec3.Normalized(direction);
        }
        #endregion
        
        #region getters and setters
        public vec3 Origine
        {
            get
            {
                return _origine;
            }
            set
            {
                _origine = value;
            }
        }

        public vec3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }
        #endregion

        #region public functions
        public vec3 PointPourT(float t)
        {
            if (t < 0)
            {
                return new vec3(-1, -1, -1);
            }
            else
            {
                return _origine + vec3.Normalized(_direction) * t;
            }
        }
        #endregion
    }

    class vec3
    {
        #region variables
        float _x, _y, _z;
        #endregion

        #region constructors
        public vec3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public vec3()
        {
            _x = 0;
            _y = 0;
            _z = 0;
        }
        #endregion

        #region getters and setters
        public float x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public float y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public float z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }
        #endregion

        #region operators
        public static vec3 operator +(vec3 vec1, vec3 vec2)
        {
            return new vec3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
        }

        public static vec3 operator -(vec3 vec1, vec3 vec2)
        {
            return new vec3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
        }
        
        public static vec3 operator *(vec3 vec1, vec3 vec2)
        {
            return new vec3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
        }

        public static vec3 operator /(vec3 vec1, vec3 vec2)
        {
            return new vec3(vec1.x / vec2.x, vec1.y / vec2.y, vec1.z / vec2.z);
        }

        public static vec3 operator +(vec3 vec1, float num)
        {
            return new vec3(vec1.x + num, vec1.y + num, vec1.z + num);
        }

        public static vec3 operator -(vec3 vec1, float num)
        {
            return new vec3(vec1.x - num, vec1.y - num, vec1.z - num);
        }

        public static vec3 operator *(vec3 vec1, float num)
        {
            return new vec3(vec1.x * num, vec1.y * num, vec1.z * num);
        }

        public static vec3 operator /(vec3 vec1, float num)
        {
            return new vec3(vec1.x / num, vec1.y / num, vec1.z / num);
        }
        #endregion

        #region public functions
        public void Print()
        {
            Console.WriteLine("({0},{1},{2})", _x, _y, _z);
        }
        #endregion

        #region static functions
        public static float Norme(vec3 vec)
        {
            return (float) Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
        }

        public static float NormeCarre(vec3 vec)
        {
            return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z;
        }

        public static vec3 Normalized(vec3 vec)
        {
            float norme = Norme(vec);
            return new vec3(vec.x / norme, vec.y / norme, vec.z / norme);
        }

        public static float Scalaire(vec3 vec1, vec3 vec2)
        {
            return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
        }

        public static vec3 Vectoriel(vec3 vec1, vec3 vec2)
        {
            return new vec3(
                vec1.y * vec2.z - vec1.z * vec2.y,
                vec1.z * vec2.x - vec1.x * vec2.z,
                vec1.x * vec2.y - vec1.y * vec2.x);
        }
        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            #region test vecteur
            vec3 v1 = new vec3(1, 2, 3);
            vec3 v2 = new vec3(4, 5, 6);
            Console.Write("v1="); v1.Print();
            Console.Write("v2="); v2.Print();
            Console.Write("v1 + v2="); (v1 + v2).Print();
            Console.Write("v1 - v2="); (v1 - v2).Print();
            Console.Write("v1 * v2="); (v1 * v2).Print();
            Console.Write("v1 / v2="); (v1 / v2).Print();
            Console.WriteLine("Norme v1=" + vec3.Norme(v1));
            Console.WriteLine("NormeCarre v1=" + vec3.NormeCarre(v1));
            Console.Write("Normalized v1="); vec3.Normalized(v1).Print();
            Console.WriteLine("Norme Normalized v1=" + vec3.Norme(vec3.Normalized(v1)));
            Console.Write("Normalized v2="); vec3.Normalized(v2).Print();
            Console.WriteLine("Norme Normalized v2=" + vec3.Norme(vec3.Normalized(v2)));
            Console.WriteLine("Scalaire=" + vec3.Scalaire(v1, v2));
            Console.Write("Vectoriel="); vec3.Vectoriel(v1, v2).Print();
            #endregion

            // separateur
            Console.WriteLine("");

            #region test Utils
            Console.WriteLine("Clamp -1=" + Utils.Clamp(-1, 0, 255));
            Console.WriteLine("Clamp 128=" + Utils.Clamp(128, 0, 255));
            Console.WriteLine("Clamp 260=" + Utils.Clamp(260, 0, 255));

            int width = 3, height = 2;
            vec3[] colors = new vec3[width * height];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new vec3();
                colors[i].x = i * 20;
                colors[i].y = i * 30;
                colors[i].z = i * 40;
            }

            //Utils.Save_Img(width, height, colors);
            #endregion

            // separateur
            Console.WriteLine("");

            #region test rayon
            Ray r = new Ray(new vec3(0, 0, 0), new vec3(1, 1, 0));

            r.PointPourT(5).Print();
            #endregion

            #region test sphere
            // doint avoir le point d'intersection entre 1 et 1.5
            Sphere s1 = new Sphere(new vec3(1, 0, 0), 1.5f, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3()));
            intersection_data id1 = s1.Intersection(r);
            Console.WriteLine("t1=" + id1.T);
            Console.Write("Point intersection avec s1="); if(id1.Position == null) Console.WriteLine("null"); else id1.Position.Print();
            
            // pas d'intersection, delta negative
            Sphere s2 = new Sphere(new vec3(1, 0, 0), 0.5f, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 1, 1)));
            intersection_data id2 = s2.Intersection(r);
            Console.WriteLine("t2=" + id2.T);
            Console.Write("Point intersection avec s2="); if(id2.Position == null) Console.WriteLine("null"); else id2.Position.Print();

            // doint avoir le point d'intersection entre 0 et 1
            Sphere s3 = new Sphere(new vec3(1, 1, 0), 0.5f, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 1, 1)));
            intersection_data id3 = s3.Intersection(r);
            Console.WriteLine("t3=" + id3.T);
            Console.Write("Point intersection avec s3="); if(id3.Position == null) Console.WriteLine("null"); else id3.Position.Print();

            // hors point de vue t1 et t2 negatifs
            Sphere s4 = new Sphere(new vec3(-1, -1, 0), 0.5f, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 1, 1)));
            intersection_data id4 = s4.Intersection(r);
            Console.WriteLine("t4=" + id4.T);
            Console.Write("Point intersection avec s4="); if(id4.Position == null) Console.WriteLine("null"); else id4.Position.Print();
            #endregion

            #region test lancer rayon
            Sphere s = new Sphere(new vec3(0, 0, -55), 20, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 0, 1)));
            Scene c = new Scene(new vec3(), 60, 1024, 1024);
            //c.DrawSphere(s);
            #endregion

            #region scene
            //Scene sc1 = new Scene(new vec3(0, 0, 0), 60, 1024, 1024);
            //Scene sc1 = new Scene(new vec3(0, 0, 0), 80, 1024, 1024);
            Scene sc1 = new Scene(new vec3(0, 0, 150), 80, 1024, 1024);
            
            Sphere sph1 = new Sphere(new vec3(20, -20, -55), 20, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 0, 0)));
            sc1.AddPrimitiveBasic(sph1);
            
            /*Bas/
            Sphere sph2 = new Sphere(new vec3(0, -2000 - 20, 0), 2000, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0, 1, 0)));
            sc1.AddPrimitiveBasic(sph2);
            /*/
            Plan p2 = new Plan(new vec3(0, -20, 0), new vec3(0, 1, 0), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0, 1, 0)));
            sc1.AddPrimitiveBasic(p2);
            //*/
            
            /*Haut/
            Sphere sph3 = new Sphere(new vec3(0, 2000 + 100, 0), 2000, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0, 0, 1)));
            sc1.AddPrimitiveBasic(sph3);
            /*/
            Plan p3 = new Plan(new vec3(0, 100, 0), new vec3(0, -1, 0), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0, 0, 1)));
            sc1.AddPrimitiveBasic(p3);
            //*/

            /*Gauche/
            //Sphere sph4 = new Sphere(new vec3(-2000 - 50, 0, 0), 2000, new Materiel(MATERIEL_TYPE.MIRROIR, 0, new vec3(1, 1, 0));
            Sphere sph4 = new Sphere(new vec3(-2000 - 50, 0, 0), 2000, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 1, 0)));
            sc1.AddPrimitiveBasic(sph4);
            /*/
            Plan p4 = new Plan(new vec3(-50, 0, 0), new vec3(1, 0, 0), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 1, 0)));
            sc1.AddPrimitiveBasic(p4);
            //*/

            /*Droite/
            Sphere sph5 = new Sphere(new vec3(2000 + 50, 0, 0), 2000, new Materiel(MATERIEL_TYPE.SPECULAIRE, 0, new vec3(0, 1, 1)));
            //Sphere sph5 = new Sphere(new vec3(2000 + 50, 0, 0), 2000, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0, 1, 1)));
            sc1.AddPrimitiveBasic(sph5);
            /*/
            Plan p5 = new Plan(new vec3(50, 0, 0), new vec3(-1, 0, 0), new Materiel(MATERIEL_TYPE.SPECULAIRE, 0, new vec3(0, 1, 1)));
            sc1.AddPrimitiveBasic(p5);
            //*/

            /*Fond/
            Sphere sph6 = new Sphere(new vec3(0, 0, -2000 - 100), 2000, new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 0, 1)));
            sc1.AddPrimitiveBasic(sph6);
            /*//**/
            Plan p6 = new Plan(new vec3(0, 0, -100), new vec3(0, 0, 1), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 0, 1)));
            sc1.AddPrimitiveBasic(p6);
            /*/
            Triangle t6 = new Triangle(new vec3(-50, -20, -100), new vec3(-50, 100, -100), new vec3(50, -20, -100), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(1, 0, 1)));
            sc1.AddPrimitiveBasic(t6);
            //*/
            
            Sphere sph7 = new Sphere(new vec3(0, 30, -55), 10, new Materiel(MATERIEL_TYPE.SPECULAIRE, 0, new vec3(0.5f, 0.5f, 0.5f)));
            //Sphere sph7 = new Sphere(new vec3(0, 30, -55), 10, new Materiel(MATERIEL_TYPE.MAT, 0, new vec3(0.5f, 0.5f, 0.5f)));
            sc1.AddPrimitiveBasic(sph7);

            Sphere sph8 = new Sphere(new vec3(-10, 0, -15), 10, new Materiel(MATERIEL_TYPE.TRANSPARENTE, 1.458f, new vec3(0.5f, 0, 1)));
            sc1.AddPrimitiveBasic(sph8);

            Quad t9 = new Quad(new vec3(-30, -20, -30), new vec3(-30, -20, -50), new vec3(-30, 20, -50), new vec3(-30, 20, -30), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0.5f, 0.5f, 1.0f)));
            sc1.AddPrimitiveBasic(t9);

            /*Triangle t10 = new Triangle(new vec3(-50, -20, -50), new vec3(-50, 50, -50), new vec3(-10, -20, -30), new Materiel(MATERIEL_TYPE.DIFFUSE, 0, new vec3(0.5f, 1.0f, 0.5f)));
            sc1.AddPrimitiveBasic(t10);*/

            Model m = new Model("TeapotPot.obj");

            int count_before = 0;
            foreach(Mesh ms in m.arrMesh)
            {
                for (int i = 0; i < ms.getCount("f"); i++)
                {
                    Face f = ms.getFace(i);
                    try
                    {
                        if (f.D == null)
                        {
                            Triangle q = new Triangle(
                                ms[int.Parse(f.A.Split('/')[0]) - 1 - count_before, "v"],
                                ms[int.Parse(f.B.Split('/')[0]) - 1 - count_before, "v"],
                                ms[int.Parse(f.C.Split('/')[0]) - 1 - count_before, "v"],
                                new Materiel(MATERIEL_TYPE.DIFFUSE, 1, new vec3(0.56f, 0.11f, 0.88f)));
                            sc1.AddPrimitiveBasic(q);
                        }
                        else
                        {
                            Quad q = new Quad(
                                ms[int.Parse(f.A.Split('/')[0]) - 1 - count_before, "v"],
                                ms[int.Parse(f.B.Split('/')[0]) - 1 - count_before, "v"],
                                ms[int.Parse(f.C.Split('/')[0]) - 1 - count_before, "v"],
                                ms[int.Parse(f.D.Split('/')[0]) - 1 - count_before, "v"],
                                new Materiel(MATERIEL_TYPE.DIFFUSE, 1, new vec3(0.56f, 0.11f, 0.88f)));
                            sc1.AddPrimitiveBasic(q);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("");
                    }
                }
                count_before += ms.getCount("v");
            }
            
            Lumiere lum1 = new Lumiere(new vec3(15, 70, -30), 5000f);
            sc1.AddLumiere(lum1);
            
            Lumiere lum2 = new Lumiere(new vec3(-40, 10, -35), 2000f);
            sc1.AddLumiere(lum2);

            sc1.DrawScene();
            #endregion

            // pour pouvoir voir les resultats
            Console.ReadLine();
        }
    }
}
