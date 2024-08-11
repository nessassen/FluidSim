using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    public SpriteRenderer rend0;
    public SpriteRenderer rend1;

    public void SetColors(Color col0, Color col1)
    {
        rend0.color = col0;
        rend1.color = col1;
    }
}
