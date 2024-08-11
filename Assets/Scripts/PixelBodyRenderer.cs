using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelBodyRenderer : MonoBehaviour
{
    [SerializeField] private Color color = new Color(1, 1, 1, 1);
    [SerializeField] private PixelBody body;
    [SerializeField] private PixelCollider col;
    [SerializeField] private SpriteRenderer sprend;
    [SerializeField] private bool isRendered = false;
    [SerializeField] private Sprite spr;
    [SerializeField] private Texture2D tex;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        col = body.col;
        sprend = body.sprend;
        isRendered = Render();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(!isRendered)
        {
            isRendered = Render();
        }
    }

    private bool Render()
    {
        sprend.color = color;
        tex = CreateColTex(col);
        spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0f), 1);
        sprend.sprite = spr;
        if(body is PixelTeleBody)
        {
            PixelTeleBody teleBody = (PixelTeleBody)body;
            teleBody.teleSprendA.sprite = spr;
            teleBody.teleSprendA.color = color;
            teleBody.teleSprendB.sprite = spr;
            teleBody.teleSprendB.color = color;
            teleBody.teleSprendAB.sprite = spr;
            teleBody.teleSprendAB.color = color;
        }
        return true;
    }

    private PixelRect GetBounds(PixelCollider c)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        PixelShape s;
        s = col.shape;
        if (s is PixelLine)
        {
            PixelLine l = (PixelLine)s;
            minX = Math.Min(minX, Math.Min(l.start.x, l.end.x));
            maxX = Math.Max(maxX, Math.Max(l.start.x, l.end.x));
            minY = Math.Min(minY, Math.Min(l.start.y, l.end.y));
            maxY = Math.Max(maxY, Math.Max(l.start.y, l.end.y));
            //return new Vector2Int(Math.Abs(l.end.x - l.start.x) + 1, Math.Abs(l.end.y - l.start.y) + 1);
        }
        else if (s is PixelRect)
        {
            PixelRect r = (PixelRect)s;
            minX = Math.Min(minX, r.left);
            maxX = Math.Max(maxX, r.right);
            minY = Math.Min(minY, r.down);
            maxY = Math.Max(maxY, r.up);
            //return new Vector2Int(r.right - r.left + 1, r.up - r.down + 1);
        }
        else if (s is PixelDiag)
        {
            PixelDiag d = (PixelDiag)s;
            minX = Math.Min(minX, d.leftVert.x);
            maxX = Math.Max(maxX, d.rightVert.x);
            minY = Math.Min(minY, d.downVert.y);
            maxY = Math.Max(maxY, d.upVert.y);
            //return new Vector2Int(d.rightVert.x - d.leftVert.x + 1, d.upVert.y - d.downVert.y + 1);
        }
        else if (s is PixelOct)
        {
            PixelOct o = (PixelOct)s;
            minX = Math.Min(minX, o.left);
            maxX = Math.Max(maxX, o.right);
            minY = Math.Min(minY, o.down);
            maxY = Math.Max(maxY, o.up);
            //return new Vector2Int(o.right - o.left + 1, o.up - o.down + 1);
        }
        
        if (minX <= maxX && minY <= maxY)
        {
            return new PixelRect(minX, maxX, minY, maxY);
        }
        else
        {
            return new PixelRect(0, 0, 0, 0);
        }
    }

    private Texture2D CreateColTex(PixelCollider c)
    {
        PixelRect bounds = GetBounds(c);
        tex = new Texture2D(1 + bounds.right - bounds.left, 1 + bounds.up - bounds.down);
        tex.filterMode = FilterMode.Point;
        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                tex.SetPixel(j, i , Color.clear);
            }
        }
        PixelShape s = col.shape;
        if (s is PixelLine)
        {
            PixelLine l = (PixelLine)s;
            int lx = l.end.x - l.start.x;
            int ly = l.end.y - l.start.y;
            int lMinX = Math.Min(l.start.x, l.end.x);
            int lMaxX = Math.Max(l.start.x, l.end.x);
            int lMinY = Math.Min(l.start.y, l.end.y);
            int lMaxY = Math.Max(l.start.y, l.end.y);
            if (lx == 0)
            {
                for (int i = lMinY; i <= lMaxY; i++)
                {
                    tex.SetPixel(l.start.x - bounds.left, i - bounds.down, color);
                }
            }
            else if (ly == 0)
            {
                for (int i = lMinX; i <= lMaxX; i++)
                {
                    tex.SetPixel(i - bounds.left, l.start.y - bounds.down, color);
                }
            }
            else if (Math.Abs(lx) == Math.Abs(ly))
            {
                for (int i = 0; i <= lx; i++)
                {
                    tex.SetPixel(l.start.x - bounds.left + i * Math.Sign(lx), l.start.y - bounds.down + i * Math.Sign(ly), color);
                }
            }
            //else
            //{
            //    float lMag = Mathf.Sqrt(lx * lx + ly * ly);
            //    for (int i = 0; i < tex.height; i++)
            //    {
            //        for (int j = 0; j < tex.width; j++)
            //        {
            //            if (Mathf.Abs(ly * j - lx * i + l.end.x * l.start.y - l.start.x * l.end.y) / lMag <= .5f)
            //            {
            //                tex.SetPixel(j, i, color);
            //            }
            //        }
            //    }
            //}
        }
        else if (s is PixelRect)
        {
            PixelRect r = (PixelRect)s;
            for (int i = r.left; i <= r.right; i++)
            {
                for (int j = r.down; j <= r.up; j++)
                {
                    tex.SetPixel(i - bounds.left, j - bounds.down, color);
                }
            }
        }
        else if (s is PixelDiag)
        {
            PixelDiag d = (PixelDiag)s;
            for (int i = d.leftVert.x; i <= d.rightVert.x; i++)
            {
                for (int j = d.downVert.y; j <= d.upVert.y; j++)
                {
                    if (d.downDiag <= (i - d.leftVert.x + j - d.downVert.y)
                     && d.upDiag <= (i - d.leftVert.x + d.upVert.y - j)
                     && d.upDiag <= (d.rightVert.x - i + j - d.downVert.y)
                     && d.downDiag <= (d.rightVert.x - i + d.upVert.y - j))
                    {
                        tex.SetPixel(i - bounds.left, j - bounds.down, color);
                    }
                }
            }
        }
        else if (s is PixelOct)
        {
            PixelOct o = (PixelOct)s;
            for (int i = o.left; i <= o.right; i++)
            {
                for (int j = o.down; j <= o.up; j++)
                {
                    if (o.ldDiag <= (i - o.left + j - o.down)
                     && o.luDiag <= (i - o.left + o.up - j)
                     && o.rdDiag <= (o.right - i + j - o.down)
                     && o.ruDiag <= (o.right - i + o.up - j))
                    {
                        tex.SetPixel(i - bounds.left, j - bounds.down, color);
                    }
                }
            }
        }
        
        tex.Apply();
        return tex;
    }

    private bool PixelInShape(Vector2Int p, PixelShape s)
    {
        if (s is PixelLine)
        {
            PixelLine l = (PixelLine)s;
            int lx = l.end.x - l.start.x;
            int ly = l.end.y - l.start.y;
            if (lx == 0)
            {
                return true;
            }
            else if (ly == 0)
            {
                return true;
            }
            else if (lx == ly)
            {
                return p.x == p.y;
            }
            else if (Math.Abs(lx) == Math.Abs(ly))
            {
                return p.x == ly - p.y;
            }
            else
            {
                float lMag = Mathf.Sqrt(lx * lx + ly * ly);
                return Mathf.Abs(ly * p.x - lx * p.y + l.end.x * l.start.y - l.start.x * l.end.y) / lMag <= .5f;
            }
        }
        else if (s is PixelRect)
        {
            return true;
        }
        else if (s is PixelDiag)
        {
            PixelDiag d = (PixelDiag)s;
            return d.downDiag <= (p.x + p.y)
                && d.upDiag <= (p.x + (d.upVert.y - d.downVert.y) - p.y)
                && d.upDiag <= ((d.rightVert.x - d.leftVert.x) - p.x + p.y)
                && d.downDiag <= ((d.rightVert.x - d.leftVert.x) - p.x + (d.upVert.y - d.downVert.y) - p.y);
        }
        else if (s is PixelOct)
        {
            PixelOct o = (PixelOct)s;
            return o.ldDiag <= (p.x + p.y)
                && o.luDiag <= (p.x + (o.up - o.down) - p.y)
                && o.rdDiag <= ((o.right - o.left) - p.x + p.y)
                && o.ruDiag <= ((o.right - o.left) - p.x + (o.up - o.down) - p.y);

        }
        else
        {
            return false;
        }
    }
}