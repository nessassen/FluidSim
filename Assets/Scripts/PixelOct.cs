using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelOct : PixelShape
{
    public int left;
    public int right;
    public int down;
    public int up;
    public int ldDiag;
    public int luDiag;
    public int rdDiag;
    public int ruDiag;


    public PixelOct(int l, int r, int d, int u, int ld, int lu, int rd, int ru)
    {
        left = l;
        right = r;
        down = d;
        up = u;
        ldDiag = ld;
        luDiag = lu;
        rdDiag = rd;
        ruDiag = ru;
    }

    public PixelOct(int l, int r, int d, int u, int diag)
    {
        left = l;
        right = r;
        down = d;
        up = u;
        ldDiag = diag;
        luDiag = diag;
        rdDiag = diag;
        ruDiag = diag;
    }

    public override void Move(Vector2Int disp)
    {
        left += disp.x;
        right += disp.x;
        down += disp.y;
        up += disp.y;
    }

    public override void Rotate(Vector2Int origin, int rot)
    {
        int tl, tr, td, tu, tld, tlu, trd, tru;
        switch (rot)
        {
            case 0:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                tld = ldDiag;
                tlu = luDiag;
                trd = rdDiag;
                tru = ruDiag;
                break;
            case 90:
                tl = origin.x + origin.y - up;
                tr = origin.x + origin.y - down;
                td = left - origin.x + origin.y;
                tu = right - origin.x + origin.y;
                tld = luDiag;
                tlu = ruDiag;
                trd = ldDiag;
                tru = rdDiag;
                break;
            case 180:
                tl = origin.x + origin.x - right;
                tr = origin.x + origin.x - left;
                td = origin.y + origin.y - up;
                tu = origin.y + origin.y - down;
                tld = ruDiag;
                tlu = rdDiag;
                trd = luDiag;
                tru = ldDiag;
                break;
            case 270:
                tl = origin.x + down - origin.y;
                tr = origin.x + up - origin.y;
                td = origin.x - right + origin.y;
                tu = origin.x - left + origin.y;
                tld = rdDiag;
                tlu = ldDiag;
                trd = ruDiag;
                tru = luDiag;
                break;
            default:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                tld = ldDiag;
                tlu = luDiag;
                trd = rdDiag;
                tru = ruDiag;
                break;
        }
        left = tl;
        right = tr;
        down = td;
        up = tu;
        ldDiag = tld;
        luDiag = tlu;
        rdDiag = trd;
        ruDiag = tru;
    }

    public override void Reflect(Vector2Int origin, int norm)
    {
        int tl, tr, td, tu, tld, tlu, trd, tru;
        switch (norm % 180)
        {
            case 0:
                tl = left;
                tr = right;
                td = origin.y + origin.y - up;
                tu = origin.y + origin.y - down;
                tld = luDiag;
                tlu = ldDiag;
                trd = ruDiag;
                tru = rdDiag;
                break;
            case 45:
                tl = origin.x + down - origin.y;
                tr = origin.x + up - origin.y;
                td = left - origin.x + origin.y;
                tu = right - origin.x + origin.y;
                tld = ldDiag;
                tlu = rdDiag;
                trd = luDiag;
                tru = ruDiag;
                break;
            case 90:
                tl = origin.x + origin.x - right;
                tr = origin.x + origin.x - left;
                td = down;
                tu = up;
                tld = rdDiag;
                tlu = ruDiag;
                trd = ldDiag;
                tru = luDiag;
                break;
            case 135:
                tl = origin.x + origin.y - up;
                tr = origin.x + origin.y - down;
                td = origin.x - right + origin.y;
                tu = origin.x - left + origin.y;
                tld = ruDiag;
                tlu = luDiag;
                trd = rdDiag;
                tru = ldDiag;
                break;
            default:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                tld = ldDiag;
                tlu = luDiag;
                trd = rdDiag;
                tru = ruDiag;
                break;
        }
        left = tl;
        right = tr;
        down = td;
        up = tu;
        ldDiag = tld;
        luDiag = tlu;
        rdDiag = trd;
        ruDiag = tru;
    }

    public override void Reflect(Vector2Int start, Vector2Int end, int norm)
    {
        int tl, tr, td, tu, tld, tlu, trd, tru;
        switch (norm)
        {

            case 0:
                tl = left;
                tr = right;
                td = end.y + start.y - up;
                tu = end.y + start.y - down;
                tld = luDiag;
                tlu = ldDiag;
                trd = ruDiag;
                tru = rdDiag;
                break;
            case 45:
                tl = end.x + down - start.y;
                tr = end.x + up - start.y;
                td = end.y + left - start.x;
                tu = end.y + right - start.y;
                tld = ldDiag;
                tlu = rdDiag;
                trd = luDiag;
                tru = ruDiag;
                break;
            case 90:
                tl = end.x + start.x - right;
                tr = end.x + start.x - left;
                td = down;
                tu = up;
                tld = rdDiag;
                tlu = ruDiag;
                trd = ldDiag;
                tru = luDiag;
                break;
            case 135:
                tl = end.x + start.y - up;
                tr = end.x + start.y - down;
                td = end.x - right + start.y;
                tu = end.x - left + start.y;
                tld = ruDiag;
                tlu = luDiag;
                trd = rdDiag;
                tru = ldDiag;
                break;
            default:
                tl = left;
                tr = right;
                td = down;
                tu = up;
                tld = ldDiag;
                tlu = luDiag;
                trd = rdDiag;
                tru = ruDiag;
                break;
        }
        left = tl;
        right = tr;
        down = td;
        up = tu;
        ldDiag = tld;
        luDiag = tlu;
        rdDiag = trd;
        ruDiag = tru;
    }

    public override Vector2Int GetSize()
    {
        return new Vector2Int(right - left + 1, up - down + 1);
    }

    public override PixelRect GetBounds()
    {
        return new PixelRect(left, right, down, up);
    }
}
