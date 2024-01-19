using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferToggle : MonoBehaviour
{
    public Camera cam;
    public GameObject cyan;
    public GameObject rendObjA;
    public GameObject rendObjB;
    public RenderTexture rendTexA;
    public RenderTexture rendTexB;
    public MeshRenderer effectRenderer;
    public bool drawTexA;

    private Material effectMaterial;

    void Start()
    {
        cyan.SetActive(true);
        rendObjA.SetActive(false);
        rendObjB.SetActive(true);
        cam.targetTexture = rendTexA;
        drawTexA = true;
        effectMaterial = effectRenderer.material;
        effectMaterial.SetTexture("Fluid", rendTexB);
    }

    void Update()
    {
        if(drawTexA)
        {
            rendObjA.SetActive(true);
            rendObjB.SetActive(false);
            cam.targetTexture = rendTexB;
            effectMaterial.SetTexture("Fluid", rendTexA);
        }
        else
        {
            cyan.SetActive(false);
            rendObjA.SetActive(false);
            rendObjB.SetActive(true);
            cam.targetTexture = rendTexA;
            effectMaterial.SetTexture("Fluid", rendTexB);
        }
        drawTexA = !drawTexA;
    }
}
