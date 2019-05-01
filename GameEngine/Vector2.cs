using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameEngine
{
    struct Vector2
    {
        public float x;
        public float y;
        public Vector2(float a, float b)
        {
            x = a;
            y = b;
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vector2 operator *(float scala, Vector2 vec)
        {
            return new Vector2(vec.x * scala, vec.y * scala);
        }
        public static Vector2 operator *(Vector2 vec, float scala)
        {
            return new Vector2(vec.x * scala, vec.y * scala);
        }

        public float GetLength()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
        public void Normalize()
        {
            float len = GetLength();
            if (len < 0.0001f)
                return;
            x = x / len;
            y = y / len;
        }
    };
}
