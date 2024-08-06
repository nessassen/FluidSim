using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctBox : PixelTeleBody
{
    public Vector2Int size;
    public int diagonal;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void CreateColliders()
    {
        base.CreateColliders();
        PixelShape shape = new PixelOct((int)transform.position.x, (int)transform.position.x + size.x - 1, (int)transform.position.y, (int)transform.position.y + size.y - 1, diagonal);
        col = new PixelCollider(shape, (int)PixelPhysics.layers.player, int.MaxValue);
        col.body = this;
        PixelShape teleShape = (PixelOct)shape.Clone();
        teleCol = new PixelTeleCollider(teleShape, 1, int.MaxValue);
        teleCol.body = this;
    }
}