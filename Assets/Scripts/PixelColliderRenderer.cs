using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelColliderRenderer : MonoBehaviour
{
    public Color color;
    public PixelCollider col;
    public bool isTele;
    private Texture2D tex;
    private Sprite spr;
    private SpriteRenderer sprend;
    void Start()
    {
        
    }

    void Update()
    {
        sprend.enabled = GetColStatus();
    }

    public void CreateTex()
    {
        PixelShape shape = col.shape;
        Vector2Int colSize = shape.GetSize();
        tex = new Texture2D(colSize.x, colSize.y);
        tex.filterMode = FilterMode.Point;
        Vector2Int colPos = new Vector2Int();
        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                tex.SetPixel(j, i, Color.clear);
            }
        }
        if (shape is PixelLine)
        {
            PixelLine l = (PixelLine)shape;
            colPos = new Vector2Int(Math.Min(l.start.x, l.end.x), Math.Min(l.start.y, l.end.y));
            int lx = l.end.x - l.start.x;
            int ly = l.end.y - l.start.y;
            if (lx == 0)
            {
                for (int i = 0; i < tex.height; i++)
                {
                    tex.SetPixel(0, i, color);
                }
            }
            else if (ly == 0)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    tex.SetPixel(j, 0, color);
                }
            }
            else if (lx == ly)
            {
                for (int i = 0; i < tex.width && i < tex.height; i++)
                {
                    tex.SetPixel(i, i, color);
                }
            }
            else if (Math.Abs(lx) == Math.Abs(ly))
            {
                for (int i = 0; i < tex.width && i < tex.height; i++)
                {
                    tex.SetPixel(lx - i, i, color);
                }
            }
            else
            {
                float lMag = Mathf.Sqrt(lx * lx + ly * ly);
                for (int i = 0; i < tex.height; i++)
                {
                    for (int j = 0; j < tex.width; j++)
                    {
                        if (Mathf.Abs(ly * j - lx * i + l.end.x * l.start.y - l.start.x * l.end.y) / lMag <= .5f)
                        {
                            tex.SetPixel(j, i, color);
                        }
                    }
                }
            }
        }
        else if (shape is PixelRect)
        {
            PixelRect r = (PixelRect)shape;
            colPos = new Vector2Int(r.left, r.down);
            for (int i = 0; i < tex.height; i++)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    tex.SetPixel(j, i, color);
                }
            }
        }
        else if (shape is PixelDiag)
        {
            PixelDiag d = (PixelDiag)shape;
            colPos = new Vector2Int(d.leftVert.x, d.downVert.y);
            for (int i = 0; i < tex.height; i++)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    if (d.downDiag <= (j + i)
                    && d.upDiag <= (j + (d.upVert.y - d.downVert.y) - i)
                    && d.upDiag <= ((d.rightVert.x - d.leftVert.x) - j + i)
                    && d.downDiag <= ((d.rightVert.x - d.leftVert.x) - j + (d.upVert.y - d.downVert.y) - i))
                    {
                        tex.SetPixel(j, i, color);
                    }
                }
            }
        }
        else if (shape is PixelOct)
        {
            PixelOct o = (PixelOct)shape;
            colPos = new Vector2Int(o.left, o.down);
            for (int i = 0; i < tex.height; i++)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    if (o.ldDiag <= (j + i)
                     && o.luDiag <= (j + (o.up - o.down) - i)
                     && o.rdDiag <= ((o.right - o.left) - j + i)
                     && o.ruDiag <= ((o.right - o.left) - j + (o.up - o.down) - i))
                    {
                        tex.SetPixel(j, i, color);
                    }
                }
            }
        }
        tex.Apply();
        transform.localPosition = new Vector3(colPos.x, colPos.y) - transform.position;
    }

    public void Render()
    {
        CreateTex();
        spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0f), 1f);
        sprend = gameObject.AddComponent<SpriteRenderer>();
        sprend.sprite = spr;
        sprend.enabled = GetColStatus();
    }

    public bool GetColStatus()
    {
        if (isTele)
        {
            return col.isActive && ((PixelTeleBody)col.body).inPortal;
        }
        else
        {
            return col.isActive;
        }
    }
}
