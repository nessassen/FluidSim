using System;
using System.Collections.Generic;
using UnityEngine;

public class FluidForce : MonoBehaviour
{
    public SpriteRenderer sprend;
    [SerializeField] protected Vector2Int dimensions;
    [SerializeField] protected FluidSim fluidSim;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected PixelBody pb;
    protected Color[] pixels;
    // Start is called before the first frame update
    void Start()
    {
        pixels = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 netForce;
        float netMagnitude;
        if (pixels == null)
        {
            if (sprend != null && sprend.sprite != null && sprend.sprite.texture != null)
            {
                pixels = sprend.sprite.texture.GetPixels(0, 0, dimensions.x, dimensions.y);
            }
            return;
        }
        if (fluidSim.dstTex == null)
        {
            return;
        }
        netForce = Vector2.zero;
        netMagnitude = 0;
        Vector2 offset = (new Vector2(transform.position.x, -transform.position.y) * 16 - dimensions / 2) - (new Vector2(fluidSim.transform.position.x, -fluidSim.transform.position.y) * 16 - fluidSim.dimensions / 2);
        if (offset.x < 0 || offset.y < 0 || offset.x + dimensions.x > fluidSim.dstTex.width || offset.y + dimensions.y > fluidSim.dstTex.height) return;
        Color[] colors = fluidSim.dstTex.GetPixels(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), dimensions.x, dimensions.y);
        for (int i = 0; i < colors.Length; ++i)
        {
            if (pixels[i].a == 0) continue;
            float blueVariance = (float)(colors[i].b - .5);
            if (Mathf.Abs(blueVariance) < 1 / 256) continue;
            float u = i % dimensions.x;
            float v = i / dimensions.x;
            if (u + .5f == dimensions.x / 2 || v + .5f == dimensions.y / 2) continue;
            float dist = (new Vector2(u + .5f, v + .5f) - (dimensions / 2)).magnitude;
            float uDist = u + .5f - dimensions.x / 2;
            float vDist = v + .5f - dimensions.y / 2;
            Vector2 pixelForce = new Vector2(-uDist, vDist) * Mathf.Abs(blueVariance) * fluidSim.fluidForce / dist;
            netForce += pixelForce;
            netMagnitude += pixelForce.magnitude;
        }
        if(rb != null && rb.IsAwake()) rb.AddForce(netForce);
        if (pb != null && pb.isActiveAndEnabled) pb.AddForce(netForce);
    }
}
