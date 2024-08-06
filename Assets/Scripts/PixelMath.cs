using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelMath
{
    public static int IntDiv(int a, int b)
    {
        if (a < 0 ^ b < 0)
            return (a - b / 2) / b;
        else
            return (a + b / 2) / b;
    }

    public static int RoundBy(int a, int b)
    {
        return IntDiv(a, b) * b;
    }

    public static int PixelDist(int aX, int aY, int bX, int bY)
    {
        return Math.Abs(bX - aX) + Math.Abs(bY - aY);
    }

    public static int PixelDist(Vector2Int a, Vector2Int b)
    {
        return Math.Abs(b.x - a.x) + Math.Abs(b.y - a.y);
    }

    public static Vector2Int VectorToVectorInt(Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }


    public static Vector2Int VectorToVectorInt(Vector3 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }

    public static Vector2 VectorIntToVector(Vector2Int v)
    {
        return new Vector2(v.x, v.y);
    }

    // Returns the sign (-1, 0, 1) of angle a in degrees.
    public static int SinInt(int a)
    {
        int x = a % 360;
        if (x == 0)
        {
            return 0;
        }
        else if (x < 180)
        {
            return 1;
        }
        else if (x == 180)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

    public static int CosInt(int a)
    {
        int x = a % 360;
        if (x < 90)
        {
            return 1;
        }
        else if (x == 90)
        {
            return 0;
        }
        else if (x < 270)
        {
            return -1;
        }
        else if (x == 270)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    public static Vector2 RotateVector(Vector2 v, int a)
    {
        float sinA = Mathf.Sin(a * Mathf.Deg2Rad);
        float cosA = Mathf.Cos(a * Mathf.Deg2Rad);
        return new Vector2(v.x * cosA - v.y * sinA, v.x * sinA + v.y * cosA);
    }

    public static Vector2 RotateVector(Vector2 v, Vector2 o, int a)
    {
        float sinA = Mathf.Sin(a * Mathf.Deg2Rad);
        float cosA = Mathf.Cos(a * Mathf.Deg2Rad);
        Vector2 d = v - o;
        return o + new Vector2(d.x * cosA - d.y * sinA, d.x * sinA + d.y * cosA);
    }

    public static Vector2Int RotateVectorInt(Vector2Int v, int a)
    {
        int sinA = SinInt(a);
        int cosA = CosInt(a);
        return new Vector2Int(v.x * cosA - v.y * sinA, v.x * sinA + v.y * cosA);
    }

    public static Vector2Int RotateVectorInt(Vector2Int v, Vector2Int o, int a)
    {
        int sinA = SinInt(a);
        int cosA = CosInt(a);
        Vector2Int d = v - o;
        return o + new Vector2Int(d.x * cosA - d.y * sinA, d.x * sinA + d.y * cosA);
    }

    public static Vector2Int RotateVectorInt(Vector2Int v, Vector2 o, int a)
    {
        float sinA = Mathf.Sin(a);
        float cosA = Mathf.Cos(a);
        Vector2 d = v - o;
        return VectorToVectorInt(o + new Vector2(d.x * cosA - d.y * sinA, d.x * sinA + d.y * cosA));
    }

    public static Vector2Int ReflectVectorInt(Vector2Int v, int n)
    {
        switch(n % 180)
        {
            case 0:
                return new Vector2Int(v.x, -v.y);
            case 45:
                return new Vector2Int(v.y, v.x);
            case 90:
                return new Vector2Int(-v.x, v.y);
            case 135:
                return new Vector2Int(-v.y, -v.x);
            default:
                Vector2 tv = Vector2.Reflect(new Vector2(v.x, v.y), new Vector2(Mathf.Cos(n), Mathf.Sin(n)));
                return VectorToVectorInt(tv);
        }
    }

    public static Vector2Int ReflectVectorInt(Vector2Int v,Vector2Int o, int n)
    {
        switch (n % 180)
        {
            case 0:
                return new Vector2Int(v.x, o.y + o.y -v.y);
            case 45:
                return new Vector2Int(o.x + v.y - o.y, o.y + v.x - o.x);
            case 90:
                return new Vector2Int(o.x + o.x - v.x, v.y);
            case 135:
                return new Vector2Int(o.x + o.y - v.y, o.y + o.x - v.x);
            default:
                Vector2 tv = o + Vector2.Reflect(new Vector2(v.x - o.x, v.y - o.y), new Vector2(Mathf.Cos(n), Mathf.Sin(n)));
                return VectorToVectorInt(tv);
        }
    }

    public static Vector2Int ReflectVectorInt(Vector2Int v, Vector2Int s, Vector2Int e, int n)
    {
        switch (n % 180)
        {
            case 0:
                return new Vector2Int(v.x, e.y + s.y - v.y);
            case 45:
                return new Vector2Int(e.x + v.y - s.x, e.y + v.x - s.x);
            case 90:
                return new Vector2Int(e.x + s.x - v.x, v.y);
            case 135:
                return new Vector2Int(e.x + s.y - v.y, e.y + s.x - v.x);
            default:
                Vector2 o = (s + e) / 2;
                Vector2 tv = o + Vector2.Reflect(new Vector2(v.x - o.x, v.y - o.y), new Vector2(Mathf.Cos(n), Mathf.Sin(n)));
                return VectorToVectorInt(tv);
        }
    }

    public static Vector2Int ReflectVectorInt(Vector2Int v, Vector2 o, int n)
    {
        switch (n % 180)
        {
            case 0:
                return new Vector2Int(v.x, Mathf.RoundToInt(o.y + o.y - v.y));
            case 45:
                return new Vector2Int(Mathf.RoundToInt(o.x + v.y - o.y), Mathf.RoundToInt(o.y + v.x - o.x));
            case 90:
                return new Vector2Int(Mathf.RoundToInt(o.x + o.x - v.x), v.y);
            case 135:
                return new Vector2Int(Mathf.RoundToInt(o.x + o.y - v.y), Mathf.RoundToInt(o.x - v.x + o.y));
            default:
                Vector2 tv = o + Vector2.Reflect(new Vector2(v.x - o.x, v.y - o.y), new Vector2(Mathf.Cos(n), Mathf.Sin(n)));
                return VectorToVectorInt(tv);
        }
    }
}