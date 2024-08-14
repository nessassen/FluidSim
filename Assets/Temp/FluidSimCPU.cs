using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FluidSimCPU : MonoBehaviour
{
    [SerializeField] protected Vector2Int dimensions;
    [SerializeField] protected Texture2D tex;
    [SerializeField] protected SpriteRenderer sprend;
    [SerializeField] protected Color baseColor;
    [SerializeField] protected bool isSrcA;
    [SerializeField] protected float damping;
    [SerializeField] protected int step;
    protected Color[] bufA;
    protected Color[] bufB;

    protected void Awake()
    {
        bufA = new Color[dimensions.y * dimensions.x];
        bufB = new Color[dimensions.y * dimensions.x];
        for(int i = 0; i < dimensions.y; i++)
        {
            for(int j = 0; j < dimensions.x; j++)
            {
                int k = i * dimensions.x + j;
                bufA[k] = baseColor;
                bufB[k] = baseColor;
            }
        }
        tex = new Texture2D(dimensions.x, dimensions.y);
        tex.filterMode = FilterMode.Point;
        tex.anisoLevel = 0;
        tex.SetPixels(bufA);
        Sprite spr = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(.5f, .5f));
        sprend.sprite = spr;
    }

    protected void FixedUpdate()
    {
        Color[] srcBuf;
        Color[] tgtBuf;
        if(isSrcA)
        {
            srcBuf = bufA;
            tgtBuf = bufB;
        }
        else
        {
            srcBuf = bufB;
            tgtBuf = bufA;
        }
        for(int i = 0; i < dimensions.y; i++)
        {
            for(int j = 0; j < dimensions.x; j++)
            {
                int k = i * dimensions.x + j;
                Color bufCol = srcBuf[k];

                Color north = srcBuf[(k - (dimensions.x * step) + srcBuf.Length) % srcBuf.Length];
                Color south = srcBuf[(k + (dimensions.x * step)) % srcBuf.Length];
                Color west = srcBuf[(k - step + srcBuf.Length) % srcBuf.Length];
                Color east = srcBuf[(k + step) % srcBuf.Length];

                float northB = north.b * (1f - north.r) + .5f * north.r;
                float southB = south.b * (1f - south.r) + .5f * south.r;
                float westB = west.b * (1f - west.r) + .5f * west.r;
                float eastB = east.b * (1f - east.r) + .5f * east.r;

                float smooth = (northB + southB + westB + eastB) - 2f;
                float vel = (.5f - bufCol.g) * (1f - bufCol.r) * 2f;

                float g = bufCol.b * (1f - bufCol.r) + (bufCol.b * 2f - bufCol.r / 2) * bufCol.r;
                float b = (smooth + vel) * damping * .5f + .5f;
                tgtBuf[k] = new Color(0f, g, b, 1f);
            }
        }
        tex.SetPixels(tgtBuf);
        isSrcA = !isSrcA;
    }
}
