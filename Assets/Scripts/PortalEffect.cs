using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    public SpriteRenderer sprend;

    public void SetColor(Color color)
    {
        sprend.color = color;
    }
}
