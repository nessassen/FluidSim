using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PixelTeleBody : PixelBody
{
    public PixelCollider teleCol;
    public SpriteRenderer teleSprend;
    public PixelCollider teleColA;
    public SpriteRenderer teleSprendA;
    public PixelCollider teleColB;
    public SpriteRenderer teleSprendB;
    public PixelCollider teleColAB;
    public SpriteRenderer teleSprendAB;
    public PixelPortal telePortal;
    public PixelPortal telePortalA;
    public PixelPortal telePortalB;
    public bool inPortal;
    public bool inPortalA;
    public bool inPortalB;

    protected override void Awake()
    {
        base.Awake();
        teleSprend.enabled = false;
        teleSprend.transform.position = transform.position;
    }

    public override void Move(Vector2Int step)
    {
        TeleMove(step);
    }

    public virtual void TeleMove(Vector2Int step)
    {
        Vector2Int teleStep;
        if (inPortal)
        {
            if (telePortal.isLinkFlipped)
            {
                teleStep = PixelMath.RotateVectorInt(PixelMath.ReflectVectorInt(step, telePortal.teleNormal), telePortal.linkRotation);
            }
            else
            {
                teleStep = PixelMath.RotateVectorInt(step, telePortal.linkRotation);
            }
        }
        else
        {
            teleStep = step;
        }
        base.Move(step);
        teleCol.Move(teleStep);
        
        List<PixelBody> overlapPortals = new List<PixelBody>();
        overlapPortals.AddRange(pixelPhysics.Scan(col.shape, (int)PixelPhysics.layers.portal));
        if (inPortal)
        {
            if (!overlapPortals.Contains(telePortal))
            {
                ExitPortal(step);
                Move(step);
            }
        }
        if (!inPortal)            // No else here in case the player exits one portal and enters another in the same frame.
        {
            if (overlapPortals.Count > 0)
            {
                EnterPortal((PixelPortal)overlapPortals[0], step);
            }
        }
    }

    public virtual void EnterPortal(PixelPortal portal, Vector2Int step)
    {
        Debug.Log("enter");
        inPortal = true;
        telePortal = portal;
        Vector2 teleDelta;
        float cos = Mathf.Cos(telePortal.linkRotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(telePortal.linkRotation * Mathf.Deg2Rad);
        if (telePortal.isLinkFlipped)
        {
            teleCol.Reflect(telePortal.start, telePortal.end, telePortal.teleNormal);
            teleCol.Rotate(telePortal.start, telePortal.linkRotation);
            teleCol.Move(telePortal.linkOffset);

            teleSprend.transform.RotateAround(new Vector3(telePortal.start.x + telePortal.end.x + 1f, telePortal.start.y + telePortal.end.y + 1f) / 2f, new Vector3(Mathf.Cos(telePortal.teleNormal), Mathf.Sin(telePortal.teleNormal), 0f), 180f);
            teleSprend.transform.RotateAround(new Vector3(telePortal.start.x + cos, telePortal.start.y + sin), Vector3.forward, telePortal.linkRotation);
            teleSprend.transform.position += new Vector3(telePortal.linkOffset.x, telePortal.linkOffset.y);
            teleSprend.transform.position = new Vector3(Mathf.RoundToInt(teleSprend.transform.position.x), Mathf.RoundToInt(teleSprend.transform.position.y), Mathf.RoundToInt(teleSprend.transform.position.z));
            teleDelta = PixelMath.RotateVector(Vector2.Reflect(delta, telePortal.teleNormalVec), telePortal.linkRotation);
        }
        else
        {
            teleCol.Rotate(telePortal.start, telePortal.linkRotation);
            teleCol.Move(telePortal.linkOffset);

            teleSprend.transform.RotateAround(new Vector3(telePortal.start.x + .5f, telePortal.start.y + .5f), Vector3.forward, telePortal.linkRotation);
            teleSprend.transform.position += new Vector3(telePortal.linkOffset.x, telePortal.linkOffset.y);
            teleSprend.transform.position = new Vector3(Mathf.Round(teleSprend.transform.position.x), Mathf.Round(teleSprend.transform.position.y), Mathf.Round(teleSprend.transform.position.z));
            teleDelta = PixelMath.RotateVector(delta, telePortal.linkRotation);
        }
        int boundDispX = Mathf.FloorToInt(teleDelta.x + (0f < teleDelta.x ? 1f : 0f));
        int boundDispY = Mathf.FloorToInt(teleDelta.y + (0f < teleDelta.y ? 1f : 0f));
        teleCol.SetBounds(new Vector2Int(boundDispX, boundDispY));
        teleCol.nearbyColliders.Clear();
        foreach (PixelCollider other in pixelPhysics.GetNearbyColliders(teleCol))
        {
            teleCol.nearbyColliders.Add(other);
            other.nearbyColliders.Add(teleCol);
        }
        teleSprend.enabled = true;
    }

    public virtual void ExitPortal(Vector2Int step)
    {
        inPortal = false;
        if (Math.Abs(step.y) < Math.Abs(step.x))
        {
            if (telePortal.teleNormalVec.x != 0 && Math.Sign(step.x) == -Math.Sign(telePortal.teleNormalVec.x))
            {
                Teleport();
            }
            else
            {
                teleSprend.transform.localRotation = Quaternion.identity;
                teleSprend.transform.localPosition = Vector3.zero;
                teleCol.shape = (PixelShape)col.shape.Clone();
            }
        }
        else
        {
            if (telePortal.teleNormalVec.y != 0 && Math.Sign(step.y) == -Math.Sign(telePortal.teleNormalVec.y))
            {
                Teleport();
            }
            else
            {
                teleSprend.transform.localRotation = Quaternion.identity;
                teleSprend.transform.localPosition = Vector3.zero;
                teleCol.shape = (PixelShape)col.shape.Clone();
            }
        }
        telePortal = null;
        teleSprend.enabled = false;
    }

    protected virtual void Teleport()
    {
        Vector2 teleDelta;
        Vector2 teleVel;
        if (telePortal.isLinkFlipped)
        {
            col.shape = (PixelOct)teleCol.shape.Clone();

            transform.rotation = teleSprend.transform.rotation;
            transform.position = teleSprend.transform.position;
            teleSprend.transform.localRotation = Quaternion.identity;
            teleSprend.transform.localPosition = Vector3.zero;
            Vector3 reflectAxis = new Vector3(Mathf.Cos(telePortal.teleNormal), Mathf.Sin(telePortal.teleNormal));
            //transform.RotateAround(new Vector3(telePortal.start.x + telePortal.end.x + 1, telePortal.start.y +  telePortal.end.y + 1, transform.position.z), reflectAxis, 180f);
            //transform.RotateAround(new Vector3(telePortal.start.x, telePortal.start.y), Vector3.forward, telePortal.linkRotation);
            //transform.position += new Vector3(telePortal.linkOffset.x, telePortal.linkOffset.y);
            //transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
            //Reflect(telePortal.start, telePortal.end, telePortal.teleNormal);
            //Rotate(telePortal.start, telePortal.linkRotation);
            //Move(telePortal.linkOffset);
            teleDelta = PixelMath.RotateVector(Vector2.Reflect(delta, telePortal.teleNormalVec), telePortal.linkRotation);
            teleVel = PixelMath.RotateVector(Vector2.Reflect(vel, telePortal.teleNormalVec), telePortal.linkRotation);
        }
        else
        {
            col.shape = (PixelOct)teleCol.shape.Clone();

            transform.position = teleSprend.transform.position;
            transform.rotation = teleSprend.transform.rotation;
            teleSprend.transform.localRotation = Quaternion.identity;
            teleSprend.transform.localPosition = Vector3.zero;
            //transform.RotateAround(new Vector3(telePortal.start.x, telePortal.start.y), Vector3.forward, telePortal.linkRotation);
            //transform.position += new Vector3(telePortal.linkOffset.x, telePortal.linkOffset.y);
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            teleDelta = PixelMath.RotateVector(delta, telePortal.linkRotation);
            teleVel = PixelMath.RotateVector(vel, telePortal.linkRotation);
        }
        int boundDispX = Mathf.FloorToInt(teleDelta.x + (0f < teleDelta.x ? 1f : 0f));
        int boundDispY = Mathf.FloorToInt(teleDelta.y + (0f < teleDelta.y ? 1f : 0f));

        col.bounds = teleCol.bounds;
        col.nearbyColliders = teleCol.nearbyColliders;
        teleCol.SetBounds(new Vector2Int(boundDispX, boundDispY));
        teleCol.nearbyColliders.Clear();
        foreach (PixelCollider other in pixelPhysics.GetNearbyColliders(teleCol))
        {
            teleCol.nearbyColliders.Add(other);
            other.nearbyColliders.Add(teleCol);
        }
        delta = Vector2.zero;// teleDelta;
        vel = teleVel;
    }

    public override void PrePhysics()
    {
        base.PrePhysics();
        if (inPortal)
        {
            Vector2 teleDelta;
            if (telePortal.isLinkFlipped)
            {
                teleDelta = PixelMath.RotateVector(Vector2.Reflect(delta, telePortal.teleNormalVec), telePortal.linkRotation);
            }
            else
            {
                teleDelta = PixelMath.RotateVector(delta, telePortal.linkRotation);
            }

            int boundDispX = Mathf.FloorToInt(teleDelta.x + (0f < teleDelta.x ? 1f : 0f));
            int boundDispY = Mathf.FloorToInt(teleDelta.y + (0f < teleDelta.y ? 1f : 0f));
            Vector2Int boundDisp = new Vector2Int(boundDispX, boundDispY);
            teleCol.SetBounds(boundDisp);
            teleCol.nearbyColliders = pixelPhysics.GetNearbyColliders(teleCol);
        }
    }

    public override List<PixelCollider> GetActiveColliders()
    {
        List<PixelCollider> ret = new List<PixelCollider>();
        if (isActive)
        {
            ret.Add(col);
            if(inPortal)
            {
                ret.Add(teleCol);
            }
        }
        return ret;
    }
}
