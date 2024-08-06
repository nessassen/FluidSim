using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFlash : MonoBehaviour
{
    public SpriteRenderer flashRenderer;
    public Vector2Int size;
    // Start is called before the first frame update
    void Start()
    {
        if(flashRenderer)
        {
            flashRenderer.enabled = false;
            flashRenderer.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Flash(Color color, float duration)
    {
        flashRenderer.color = color;
        flashRenderer.enabled = true;
        yield return new WaitForSeconds(duration);
        flashRenderer.color = Color.white;
        flashRenderer.enabled = false;
    }
}
