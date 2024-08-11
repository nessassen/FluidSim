using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoLocater : MonoBehaviour
{
    [SerializeField] protected List<SpriteRenderer> sprends;
    [SerializeField] protected FluidSim fluidSim;
    [SerializeField] protected PixelBody pb;
    [SerializeField] protected float moveForce;
    [SerializeField] protected float moveThreshold;
    [SerializeField] protected float brakeDrag;
    [SerializeField] protected float defaultDrag;
    protected Color[] pixels = null;
    protected Vector2Int dimensions = Vector2Int.zero;

    protected void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 netAmplitude = Vector2.zero;
        float netMagnitude = 0;
        if (pixels == null)
        {
            if (sprends != null && sprends.Count > 0 && sprends[0].sprite != null && sprends[0].sprite.texture != null)
            {
                dimensions = new Vector2Int(sprends[0].sprite.texture.width, sprends[0].sprite.texture.height);
                pixels = sprends[0].sprite.texture.GetPixels(0, 0, dimensions.x, dimensions.y);
            }
            return;
        }
        if (fluidSim.dstTex == null)
        {
            return;
        }
        foreach (SpriteRenderer sprend in sprends)
        {
            int minX, maxX, countX, minY, maxY, countY;
            if (!sprend.enabled || !sprend.gameObject.activeInHierarchy) continue;
            Vector2 offset = new Vector2(sprend.transform.position.x - fluidSim.transform.position.x + fluidSim.dstTex.width / 2, sprend.transform.position.y - fluidSim.transform.position.y + fluidSim.dstTex.height / 2);
            minX = (int)Mathf.Max(offset.x, 0f);
            maxX = (int)Mathf.Min(offset.x + dimensions.x, fluidSim.dstTex.width - 1);
            countX = maxX - minX;
            minY = (int)Mathf.Max(offset.y, 0f);
            maxY = (int)Mathf.Min(offset.y + dimensions.y, fluidSim.dstTex.height - 1);
            countY = maxY - minY;
            if (offset.x > maxX || offset.y > maxY || offset.x + dimensions.x < minX || offset.y + dimensions.y < minY) continue; //Return if out of bounds
            Color[] colors = fluidSim.dstTex.GetPixels(minX, minY, countX, countY);
            for (int i = 0; i < colors.Length; ++i)
            {
                if (pixels[i].a == 0) continue;
                float blueVariance = (float)(colors[i].b - .5f);
                if (Mathf.Abs(blueVariance) < 1f / 256f) continue;
                float u = (i % countX) + minX - offset.x;
                float v = dimensions.y - (i / countX);
                float uDist = u + .5f - dimensions.x / 2f;
                float vDist = v + .5f - dimensions.y / 2f;
                float dist = (new Vector2(uDist, vDist)).magnitude;
                Vector2 pixelForce = new Vector2(uDist, -vDist) * Mathf.Abs(blueVariance) * fluidSim.fluidForce * pixels[i].a / dist;
                netAmplitude += pixelForce;
                netMagnitude += pixelForce.magnitude;
            }
        }
        if (pb == null) return;
        if (netMagnitude >= moveThreshold)
        {
            pb.drag = defaultDrag;
            pb.AddForce(moveForce * netAmplitude.normalized);
        }
        else
        {
            pb.drag = brakeDrag;
        }
    }
}
