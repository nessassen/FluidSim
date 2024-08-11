using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBox : PixelTeleBody
{
    public PortalEffect portalEffect;
    public ScreenMask teleMask;                     // Used for splitscreen effect.
    public ScreenFlash entryFlash;
    public ScreenFlash exitFlash;
    public AudioSource portalAudio;
    public float flashDuration;

    protected override void Start()
    {
        base.Start();
    }

    public override void EnterPortal(PixelPortal portal, Vector2Int step)
    {
        bool isPortalA = !inPortalA;
        base.EnterPortal(portal, step);
        SetPortalEffects(portal, isPortalA);
        portalEffect.SetColor(telePortal.color);
        StartCoroutine(entryFlash.Flash(telePortal.color, flashDuration));
        portalAudio.Play();
    }

    public override void ExitPortal(bool isPortalA, Vector2Int step)
    {
        StartCoroutine(exitFlash.Flash(telePortal.linkedPortal.color, flashDuration));
        //portalAudio.Play();
        base.ExitPortal(isPortalA, step);
    }

    private void SetPortalEffects(PixelPortal portal, bool isPortalA)
    {
        int norm = portal.teleNormal;
        float cos = Mathf.Cos(norm * Mathf.Deg2Rad);
        float sin = Mathf.Sin(norm * Mathf.Deg2Rad);
        portalEffect.transform.position = new Vector3((portal.start.x + portal.end.x + 1f) / 2f, (portal.start.y + portal.end.y + 1f) / 2f, portalEffect.transform.position.z);
        portalEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
        teleMask.transform.position = new Vector3((portal.start.x + portal.end.x - teleMask.size.x * cos) / 2f, (portal.start.y + portal.end.y - teleMask.size.y * sin) / 2f, teleMask.transform.position.z);
        teleMask.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
        entryFlash.transform.position = new Vector3((portal.start.x + portal.end.x + entryFlash.size.x * cos) / 2f, (portal.start.y + portal.end.y + entryFlash.size.y * sin) / 2f, entryFlash.transform.position.z);
        entryFlash.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
        exitFlash.transform.position = new Vector3((portal.start.x + portal.end.x - exitFlash.size.x * cos) / 2f, (portal.start.y + portal.end.y - exitFlash.size.y * sin) / 2f, exitFlash.transform.position.z);
        exitFlash.transform.rotation = Quaternion.Euler(new Vector3(0, 0, norm));
    }
}
