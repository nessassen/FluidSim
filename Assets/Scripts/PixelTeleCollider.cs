using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelTeleCollider : PixelCollider
{
    public bool isTeleporting = false;
    public PixelShape teleportShape;
    public PixelRect teleportBounds;

    public PixelTeleCollider(PixelShape s, ushort l, int m) : base(s, l, m)
    {
        teleportShape = (PixelShape)s.Clone();
    }

    public override void Move(Vector2Int disp)
    {
        base.Move(disp);
        teleportShape.Move(disp);
    }
}
