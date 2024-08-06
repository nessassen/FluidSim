using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelDiag : PixelShape
{
    public Vector2Int leftVert;
    public Vector2Int rightVert;
    public Vector2Int downVert;
    public Vector2Int upVert;
    public int downDiag;
    public int upDiag;

    public PixelDiag(Vector2Int l, int d, int u)
    {
        leftVert = l;
        rightVert = new Vector2Int(l.x + u + d, l.y - d + u);
        downVert = new Vector2Int(l.x + d, l.y - d);
        upVert = new Vector2Int(l.x + u, l.y + u);
        upDiag = u;
        downDiag = d;
    }

    public override void Move(Vector2Int disp)
    {
        leftVert += disp;
        rightVert += disp;
        downVert += disp;
        upVert += disp;
    }

    public override Vector2Int GetSize()
    {
        return new Vector2Int(rightVert.x - leftVert.x + 1, upVert.y - downVert.y + 1);
    }

    public override PixelRect GetBounds()
    {
        return new PixelRect(leftVert.x, rightVert.x, downVert.y, upVert.y);
    }
}
