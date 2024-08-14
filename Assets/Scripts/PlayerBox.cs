using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBox : OctBox
{
    public PortalEffect portalEffectA;
    public PortalEffect portalEffectB;
    public ScreenMask teleMaskA;
    public ScreenMask teleMaskB;
    public ScreenFlash entryFlash;
    public ScreenFlash exitFlash;
    public AudioSource portalAudio;
    public float flashDuration;
    public Vector3 cameraOffset;

    protected override void Start()
    {
        base.Start();
    }

    public override void EnterPortal(PixelPortal portal, Vector2Int step)
    {
        bool isPortalA = !inPortalA;
        base.EnterPortal(portal, step);
        SetPortalEffects(portal, isPortalA);

        if(entryFlash != null)
        {
            StartCoroutine(entryFlash.Flash(portal.color, flashDuration));
        }
        if (portalAudio != null)
        {
            portalAudio.Play();
        }
    }

    public override void ExitPortal(bool isPortalA, Vector2Int step)
    {
        PixelPortal curPortal;
        ScreenMask curMask;
        if (isPortalA)
        {
            curPortal = telePortalA;
            curMask = teleMaskA;
        }
        else
        {
            curPortal = telePortalB;
            curMask = teleMaskB;
        }

        if (exitFlash != null)
        {
            StartCoroutine(exitFlash.Flash(curPortal.linkedPortal.color, flashDuration));
        }

        if (curMask != null)
        {
            curMask.gameObject.SetActive(false);
        }

        base.ExitPortal(isPortalA, step);
    }

    private void SetPortalEffects(PixelPortal portal, bool isPortalA)
    {
        PortalEffect curEffect;
        ScreenMask curMask;
        if (isPortalA)
        {
            curEffect = portalEffectA;
            curMask = teleMaskA;
        }
        else
        {
            curEffect = portalEffectB;
            curMask = teleMaskB;
        }

        PixelPortal linkPortal = portal.linkedPortal;
        int norm = portal.teleNormal;
        float cos = Mathf.Cos(norm * Mathf.Deg2Rad);
        float sin = Mathf.Sin(norm * Mathf.Deg2Rad);

        if (curEffect != null)
        {
            curEffect.transform.position = new Vector3((portal.start.x + portal.end.x) / 2f + 1f, (portal.start.y + portal.end.y) / 2f + 1f, curEffect.transform.position.z) + cameraOffset;
            curEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
            curEffect.SetColor(portal.color);
        }
        
        if(curMask != null)
        {
            curMask.gameObject.SetActive(true);
            curMask.transform.position = new Vector3((linkPortal.start.x + linkPortal.end.x + curMask.size.x * cos) / 2f, (portal.start.y + portal.end.y - curMask.size.y * sin) / 2f + 1f, curMask.transform.position.z) + cameraOffset;
            curMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
        }

        if(entryFlash != null)
        {
            entryFlash.transform.position = new Vector3((portal.start.x + portal.end.x + entryFlash.size.x * cos) / 2f, (portal.start.y + portal.end.y + entryFlash.size.y * sin) / 2f, entryFlash.transform.position.z) + cameraOffset;
            entryFlash.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
        }
        if (exitFlash != null)
        {
            exitFlash.transform.position = new Vector3((portal.start.x + portal.end.x - exitFlash.size.x * cos) / 2f, (portal.start.y + portal.end.y - exitFlash.size.y * sin) / 2f, exitFlash.transform.position.z) + cameraOffset;
            exitFlash.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
        }
    }
}
