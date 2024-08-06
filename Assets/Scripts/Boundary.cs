using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : PixelBody
{
    public Vector2Int end;
    protected override void Awake()
    {
        base.Awake();
        PixelShape shape = new PixelLine(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)), end);
        PixelCollider col = new PixelCollider(shape, 1, int.MaxValue);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
