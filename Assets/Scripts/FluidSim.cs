using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSim : MonoBehaviour
{
    public Camera cam;
    public GameObject cyan;
    public GameObject rendObjA;
    public GameObject rendObjB;
    public RenderTexture rendTexA;
    public RenderTexture rendTexB;
    public Texture2D dstTex;
    public Sprite dstSprite; // For testing
    public MeshRenderer effectRenderer;
    public bool isTexATarget;

    private Material effectMaterial;
    private Rect cameraViewRect;

    public Vector2 dimensions;

    void Start()
    {
        cyan.SetActive(true);
        rendObjA.SetActive(false);
        rendObjB.SetActive(true);
        cam.targetTexture = rendTexA;
        isTexATarget = true;
        effectMaterial = effectRenderer.material;
        effectMaterial.SetTexture("_FluidTex", rendTexB);
        dstTex = new Texture2D(Mathf.RoundToInt(dimensions.x), Mathf.RoundToInt(dimensions.y));
        dstTex.filterMode = FilterMode.Point;
        dstTex.anisoLevel = 0;
        //effectMaterial.SetTexture("_MainTex", dstTex);
        cameraViewRect = new Rect(0, 0, dimensions.x, dimensions.y);
        //Camera.onPostRender += OnPostRenderCallback;
    }

    void Update()
    {
        if(isTexATarget)
        {
            //Rendering.AsyncGPUReadback.RequestIntoNativeArray(rendTexA, dstTex);
            rendObjA.SetActive(true);
            rendObjB.SetActive(false);
            cam.targetTexture = rendTexB;
            effectMaterial.SetTexture("_FluidTex", rendTexA);
        }
        else
        {
            //Rendering.AsyncGPUReadback.RequestIntoNativeArray(rendTexB, dstTex);
            rendObjA.SetActive(false);
            rendObjB.SetActive(true);
            cam.targetTexture = rendTexA;
            effectMaterial.SetTexture("_FluidTex", rendTexB);
            cyan.SetActive(false);
        }
        isTexATarget = !isTexATarget;
    }
    
    void OnDestroy()
    {
        //Camera.onPostRender -= OnPostRenderCallback;
    }

    void OnPostRenderCallback(Camera c)
    {
        if(c == cam && dstTex != null)
        {
            dstTex.ReadPixels(cameraViewRect, 0, 0, false);
            dstTex.Apply();
        }
    }
}
