using System;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Vector2Int dimensions;
    public FluidSim fluidSim;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 netForce;
        float netMagnitude;
        if(fluidSim.dstTex == null)
        {
            return;
        }
        netForce = Vector2.zero;
        netMagnitude = 0;
        Vector2 offset = (new Vector2(transform.position.x, -transform.position.y) * 16 - dimensions/2) - (new Vector2(fluidSim.transform.position.x, -fluidSim.transform.position.y) * 16 - fluidSim.dimensions/2);
        Color[] colors = fluidSim.dstTex.GetPixels(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), dimensions.x, dimensions.y);
        for (int i = 0; i < colors.Length; ++i)
        {
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
        Debug.Log(netForce);
        rb.AddForce(netForce);
    }
}
