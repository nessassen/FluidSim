using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelRect : PixelShape
{
    public int left;
    public int right;
    public int down;
    public int up;

    public PixelRect(int l, int r, int d, int u)
    {
        left = l;
        right = r;
        down = d;
        up = u;
    }

    public override void Move(Vector2Int disp)
    {
        left += disp.x;
        right += disp.x;
        down += disp.y;
        up += disp.y;
    }

    public override void Rotate(Vector2Int o, int a)
    {
        int tl, tr, td, tu;
        switch(a)
        {
            case 0:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                break;
            case 90:
                tl = o.x + o.y - up;
                tr = o.x + o.y - down;
                td = left - o.x + o.y;
                tu = right - o.x + o.y;
                break;
            case 180:
                tl = o.x + o.x - right;
                tr = o.x + o.x - left;
                td = o.y + o.y - up;
                tu = o.y + o.y - down;
                break;
            case 270:
                tl = o.x + down - o.y;
                tr = o.x + up - o.y;
                td = o.x - right + o.y;
                tu = o.x - left + o.y;
                break;
            default:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                break;
        }
        left = tl;
        right = tr;
        down = td;
        up = tu;
    }

    public override void Reflect(Vector2Int o, int n)
    {
        int tl, tr, td, tu;
        switch(n % 180)
        {
            case 0:
                tl = left;
                tr = right;
                td = o.y + o.y - up;
                tu = o.y + o.y - down;
                break;
            case 45:
                tl = o.x + down - o.y;
                tr = o.x + up - o.y;
                td = left - o.x + o.y;
                tu = right - o.x + o.y;
                break;
            case 90:
                tl = o.x + o.x - right;
                tr = o.x + o.x - left;
                td = down;
                tu = up;
                break;
            case 135:
                tl = o.x + o.y - up;
                tr = o.x + o.y - down;
                td = o.x - right + o.y;
                tu = o.x - left + o.y;
                break;
            default:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                break;
        }
        left = tl;
        right = tr;
        down = td;
        up = tu;
    }

    public override void Reflect(Vector2Int s, Vector2Int e, int n)
    {
        int tl, tr, td, tu;
        switch(n % 180)
        {

            case 0:
                tl = left;
                tr = right;
                td = e.y + s.y - up;
                tu = e.y + s.y - down;
                break;
            case 45:
                tl = e.x + down - s.y;
                tr = e.x + up - s.y;
                td = e.y + left - s.x;
                tu = e.y + right - s.y;
                break;
            case 90:
                tl = e.x + s.x - right;
                tr = e.x + s.x - left;
                td = down;
                tu = up;
                break;
            case 135:
                tl = e.x + s.y - up;
                tr = e.x + s.y - down;
                td = e.x - right + s.y;
                tu = e.x - left + s.y;
                break;
            default:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                break;
        }
        left = tl;
        right = tr;
        down = td;
        up = tu;
    }

    public override Vector2Int GetSize()
    {
        return new Vector2Int(right - left + 1, up - down + 1);
    }

    public override PixelRect GetBounds()
    {
        return this;
    }
}
