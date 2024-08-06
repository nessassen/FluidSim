using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public SpriteRenderer sprend;
    public FluidForce fluFor;
    public PixelBody body;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (fluFor != null && sprend != null && fluFor.enabled == false)
        {
            fluFor.enabled = true;
            fluFor.sprend = sprend;
        }
    }
}
