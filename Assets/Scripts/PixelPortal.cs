using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPortal : PixelBody
{
    public Color color;
    public Vector2Int start;
    public Vector2Int end;
    public Vector2Int endDisp;
    public int teleNormal;
    public Vector2 teleNormalVec;
    public bool isClockwise;
    public bool islinked;
    public PixelPortal linkedPortal;
    public PixelPortal defaultLinkedPortal;
    public Vector2Int linkOffset;
    public bool isLinkFlipped;
    public int linkRotation;                        // (linkRotation % 90 != 0) causes undefined behavior. Orthogonal movement cannot be translated to diagonal movement.
    protected override void Awake()
    {
        base.Awake();
        endDisp = end - start;
        teleNormalVec = new Vector2(Mathf.Cos(teleNormal * Mathf.Deg2Rad), Mathf.Sin(teleNormal * Mathf.Deg2Rad ));
        if (!islinked && defaultLinkedPortal != null)
        {
            linkPortal(defaultLinkedPortal);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CreateColliders()
    {
        base.CreateColliders();
        PixelShape shape = new PixelLine(start, end);
        col = new PixelCollider(shape, (int)PixelPhysics.layers.portal, 0);
    }

    public void Reposition(Vector2Int s, Vector2Int e, int n)
    {
        //Unfinished
        Vector2Int offset = s - start;
        int rot = (n - teleNormal + 360) % 360;
        start = s;
        end = e;
        teleNormal = n;
    }

    public override bool PreCollision(PixelBody other, int normal)
    {
        return false;
    }

    public void linkPortal(PixelPortal other)
    {
        islinked = true;
        linkedPortal = other;
        isLinkFlipped = !(isClockwise ^ linkedPortal.isClockwise);
        linkRotation = (linkedPortal.teleNormal - teleNormal + 540) % 360;
        if (isLinkFlipped)
        {
            linkOffset = new Vector2Int(linkedPortal.end.x - start.x, linkedPortal.end.y - start.y);
        }
        else
        {
            linkOffset = new Vector2Int(linkedPortal.start.x - start.x, linkedPortal.start.y - start.y);
        }
    }

    public void unlinkPortal()
    {
        islinked = false;
        linkedPortal = null;
        linkOffset = Vector2Int.zero;
        isLinkFlipped = false;
        linkRotation = 0;
    }
}
