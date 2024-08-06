using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCollider : ICloneable
{
    public PixelShape shape;
    public PixelRect bounds;
    public int layers;
    public int mask;
    public List<PixelCollider> nearbyColliders;           // Colliders whose bounds overlap this.bounds
    public PixelBody body;
    public bool isActive = true;

    public PixelCollider(PixelShape s, ushort l, int m)
    {
        shape = s;
        layers = l;
        mask = m;
        SetBounds(Vector2Int.zero);
        nearbyColliders = new List<PixelCollider>();
    }

    public virtual void Move(Vector2Int d)
    {
        shape.Move(d);
    }

    public virtual void Rotate(Vector2Int origin, int rot)
    {
        shape.Rotate(origin, rot);
    }

    public virtual void Reflect(Vector2Int origin, int norm)
    {
        shape.Reflect(origin, norm);
    }

    public virtual void Reflect(Vector2Int start, Vector2Int end, int norm)
    {
        shape.Reflect(start, end, norm);
    }

    // Sets bounds as a PixelRect containing shape's position and its position + d
    public void SetBounds(Vector2Int d)
    {
        int boundLeft;
        int boundRight;
        int boundDown;
        int boundUp;
        if (shape is PixelLine)
        {
            PixelLine line = (PixelLine)shape;
            boundLeft = Math.Min(line.start.x, line.end.x) + Math.Min(d.x, 0);
            boundRight = Math.Max(line.start.x, line.end.x) + Math.Max(d.x, 0);
            boundDown = Math.Min(line.start.y, line.end.y) + Math.Min(d.y, 0);
            boundUp = Math.Max(line.start.y, line.end.y) + Math.Max(d.y, 0);
            bounds =  new PixelRect(boundLeft, boundRight, boundDown, boundUp);
        }
        else if (shape is PixelRect)
        {
            PixelRect rect = (PixelRect)shape;
            boundLeft = rect.left + Math.Min(d.x, 0);
            boundRight = rect.right + Math.Max(d.x, 0);
            boundDown = rect.down + Math.Min(d.y, 0);
            boundUp = rect.up + Math.Max(d.y, 0);
            bounds = new PixelRect(boundLeft, boundRight, boundDown, boundUp);
        }
        else if (shape is PixelRhomb)
        {
            PixelRhomb rhomb = (PixelRhomb)shape;
            boundLeft = rhomb.center.x - rhomb.diagonal + Math.Min(d.x, 0);
            boundRight = rhomb.center.x + rhomb.diagonal + Math.Max(d.x, 0);
            boundDown = rhomb.center.y - rhomb.diagonal + Math.Min(d.y, 0);
            boundUp = rhomb.center.y + rhomb.diagonal + Math.Max(d.y, 0);
            bounds = new PixelRect(boundLeft, boundRight, boundDown, boundUp);
        }
        else if (shape is PixelDiag)
        {
            PixelDiag diag = (PixelDiag)shape;
            boundLeft = diag.leftVert.x + Math.Min(d.x, 0);
            boundRight = diag.leftVert.x + diag.downDiag + diag.upDiag + Math.Max(d.x, 0);
            boundDown = diag.leftVert.y - diag.downDiag + Math.Min(d.y, 0);
            boundUp = diag.leftVert.y + diag.upDiag + Math.Max(d.y, 0);
            bounds = new PixelRect(boundLeft, boundRight, boundDown, boundUp);
        }
        else if (shape is PixelOct)
        {
            PixelOct oct = (PixelOct)shape;
            boundLeft = oct.left + Math.Min(d.x, 0);
            boundRight = oct.right + Math.Max(d.x, 0);
            boundDown = oct.down + Math.Min(d.y, 0);
            boundUp = oct.up + Math.Max(d.y, 0);
            bounds = new PixelRect(boundLeft, boundRight, boundDown, boundUp);
        }
    }

    public object Clone()
    {
        PixelCollider ret = (PixelCollider)MemberwiseClone();
        ret.shape = (PixelShape)shape.Clone();
        ret.bounds = (PixelRect)bounds.Clone();
        ret.nearbyColliders = new List<PixelCollider>();

        return ret;
    }
}
