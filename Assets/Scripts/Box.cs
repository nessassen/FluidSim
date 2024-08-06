using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : PixelTeleBody
{
    public Vector2Int size;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        PixelShape shape = new PixelRect((int)transform.position.x, (int)transform.position.x + size.x - 1, (int)transform.position.y, (int)transform.position.y + size.y - 1);
        PixelCollider col = new PixelCollider(shape, 1, int.MaxValue);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}