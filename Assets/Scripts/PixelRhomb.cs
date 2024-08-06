using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DEPRECATED
public class PixelRhomb : PixelShape
{
    public Vector2Int center;
    public int diagonal;

    public PixelRhomb(Vector2Int c, int  d)
    {
        center = c;
        diagonal = d;
    }

    public override void Move(Vector2Int disp)
    {
        center += disp;
    }
}
