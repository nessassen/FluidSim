using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSim : MonoBehaviour
{
    public Vector2 dimensions;
    public float fluidForce;
    public Camera cam;
    public GameObject cyan;
    public GameObject rendObjA;
    public GameObject rendObjB;
    public RenderTexture rendTexA;
    public RenderTexture rendTexB;
    public MeshRenderer effectRenderer;
    public MeshRenderer copyRenderer;
    public Texture2D dstTex;

    private Material effectMaterial;
    private Material copyMaterial;
    private Rect cameraViewRect;
    private bool isTexATarget;

    void Awake()
    {
        cyan.SetActive(true);
        rendObjA.SetActive(false);
        rendObjB.SetActive(true);
        
        cameraViewRect = new Rect(0, 0, dimensions.x, dimensions.y);
        Camera.onPostRender += OnPostRenderCallback;
        cam.targetTexture = rendTexA;
        isTexATarget = true;

        dstTex = new Texture2D(Mathf.RoundToInt(dimensions.x), Mathf.RoundToInt(dimensions.y));
        dstTex.filterMode = FilterMode.Point;
        dstTex.anisoLevel = 0;

        effectMaterial = effectRenderer.material;
        effectMaterial.SetTexture("_FluidTex", rendTexB);

        copyMaterial = copyRenderer.material;
        copyMaterial.SetTexture("_MainTex", dstTex);
        Application.targetFrameRate = 60;
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
        Camera.onPostRender -= OnPostRenderCallback;
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
