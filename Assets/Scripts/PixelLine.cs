using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelLine : PixelShape
{
    public Vector2Int start;
    public Vector2Int end;

    public PixelLine(Vector2Int s, Vector2Int e)
    {
        start = s;
        end = e;
    }

    public override void Move(Vector2Int disp)
    {
        start += disp;
        end += disp;
    }

    public override void Rotate(Vector2Int o, int a)
    {
        start = PixelMath.RotateVectorInt(start, o, a);
        end = PixelMath.RotateVectorInt(end, o, a);
    }
    public override void Rotate(Vector2 o, int a)
    {
        start = PixelMath.RotateVectorInt(start, o, a);
        end = PixelMath.RotateVectorInt(end, o, a);
    }

    public override void Reflect(Vector2Int o, int n)
    {
        start = PixelMath.ReflectVectorInt(start, o, n);
        end = PixelMath.ReflectVectorInt(end, o, n);
    }

    public override void Reflect(Vector2Int s, Vector2Int e, int n)
    {
        start = PixelMath.ReflectVectorInt(start, s, e, n);
        end = PixelMath.ReflectVectorInt(end, s, e, n);
    }

    public override Vector2Int GetSize()
    {
        return new Vector2Int(Math.Abs(end.x - start.x) + 1, Math.Abs(end.y - start.y) + 1);
    }

    public override PixelRect GetBounds()
    {
        int minX = Math.Min(start.x, end.x);
        int maxX = Math.Max(start.x, end.x);
        int minY = Math.Min(start.y, end.y);
        int maxY = Math.Max(start.y, end.y);
        return new PixelRect(minX, maxX, minY, maxY);
    }
}
