using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asciigame
{
    class Tools
    {
    }

    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public IntVector2 ToIntVector2()
        {
            return new IntVector2(x, y);
        }

        public double magnitudeD
        {
            get
            {
                return Math.Sqrt((x * x) + (y * y));
            }
        }

        public float magnitude
        {
            get
            {
                return (float)magnitudeD;
            }
        }

        public Vector2 Normalized()
        {
            float magn = magnitude;
            return new Vector2(x / magn, y / magn);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator *(Vector2 v1, float f2)
        {
            return new Vector2(v1.x * f2, v1.y * f2);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0f, 0f);
            }
        }

        public override string ToString()
        {
            return ("(" + x + ", " + y + ")");
        }
    }

    public struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public IntVector2(float _x, float _y)
        {
            x = (int)_x;
            y = (int)_y;
        }

        public Vector2 toVector2()
        {
            return new Vector2((float)x, (float)y);
        }

        public float magnitude()
        {
            return new Vector2((float)x, (float)y).magnitude;
        }

        public override string ToString()
        {
            return ("(" + x + ", " + y + ")");
        }
    }
}
