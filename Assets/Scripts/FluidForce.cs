using System;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Vector2 dimensions;
    public FluidSim fluidSim;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        double blueVariance = 0;
        if(fluidSim.dstTex == null)
        {
            return;
        }
        Vector2 offset = (new Vector2(transform.position.x, -transform.position.y) * 16 - dimensions/2) - (new Vector2(fluidSim.transform.position.x, -fluidSim.transform.position.y) * 16 - fluidSim.dimensions/2);
        Color[] colors = fluidSim.dstTex.GetPixels(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), Mathf.RoundToInt(dimensions.x), Mathf.RoundToInt(dimensions.y));
        for (int i = 0; i < colors.Length; ++i)
        {
            blueVariance += Math.Abs(colors[i].b - .5);
        }
        //Debug.Log(blueVariance);
    }
}
