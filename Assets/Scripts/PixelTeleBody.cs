using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PixelTeleBody : PixelBody
{
    public PixelCollider teleColA;
    public SpriteRenderer teleSprendA;
    public PixelCollider teleColB;
    public SpriteRenderer teleSprendB;
    public PixelCollider teleColAB;
    public SpriteRenderer teleSprendAB;
    public PixelPortal telePortal;
    public PixelPortal telePortalA;
    public PixelPortal telePortalB;
    public bool inPortalA;
    public bool inPortalB;

    protected override void Awake()
    {
        base.Awake();
        CreateTeleColliders();
        teleColA.isActive = false;
        teleSprendA.enabled = false;
        teleSprendA.transform.position = transform.position;
        teleColB.isActive = false;
        teleSprendB.enabled = false;
        teleSprendB.transform.position = transform.position;
        teleColAB.isActive = false;
        teleSprendAB.enabled = false;
        teleSprendAB.transform.position = transform.position;
    }

    protected virtual void CreateTeleColliders()
    {
        if (col == null) return;
        PixelShape teleShape = (PixelShape)col.shape.Clone();
        teleColA = new PixelTeleCollider(teleShape, (ushort)col.layers, col.mask);
        teleColA.body = this;
        teleColA.isActive = false;
        teleShape = (PixelShape)col.shape.Clone();
        teleColB = new PixelTeleCollider(teleShape, (ushort)col.layers, col.mask);
        teleColB.body = this;
        teleColB.isActive = false;
        teleShape = (PixelShape)col.shape.Clone();
        teleColAB = new PixelTeleCollider(teleShape, (ushort)col.layers, col.mask);
        teleColAB.body = this;
        teleColAB.isActive = false;
    }

    protected override void Update()
    {
    }

    public override void Move(Vector2Int step)
    {
        TeleMove(step);
        PixelOct oct = (PixelOct)col.shape;
        Vector2 vec = new Vector2(oct.left, oct.down);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        if (vec != pos)
        {
            print("Main Desync");
            print(vec);
            print(pos);
            print(step);
            Debug.Break();
        }
        oct = (PixelOct)teleColA.shape;
        vec = new Vector2(oct.left, oct.down);
        pos = new Vector2(teleSprendA.transform.position.x, teleSprendA.transform.position.y);
        if (vec != pos)
        {
            print("A Desync");
            print(teleSprendA.transform.position);
            print(vec);
            print(pos);
            print(step);
            Debug.Break();
        }
        oct = (PixelOct)teleColB.shape;
        vec = new Vector2(oct.left, oct.down);
        pos = new Vector2(teleSprendB.transform.position.x, teleSprendB.transform.position.y);
        if (vec != pos)
        {
            print("B Desync");
            print(transform.position);
            print(vec);
            print(pos);
            print(step);
            Debug.Break();
        }
        oct = (PixelOct)teleColAB.shape;
        vec = new Vector2(oct.left, oct.down);
        pos = new Vector2(teleSprendAB.transform.position.x, teleSprendAB.transform.position.y);
        if (vec != pos)
        {
            print("AB Desync");
            print(transform.position);
            print(vec);
            print(pos);
            print(step);
            Debug.Break();
        }
    }

    public virtual void TeleMove(Vector2Int step)
    {
        Vector2Int teleStepA = step;
        Vector2Int teleStepB = step;
        Vector2Int teleStepAB = step;
        if (inPortalA)
        {
            if (telePortalA.isLinkFlipped)
            {
                teleStepA = PixelMath.RotateVectorInt(PixelMath.ReflectVectorInt(step, telePortalA.teleNormal), telePortalB.linkRotation);
            }
            else
            {
                teleStepA = PixelMath.RotateVectorInt(step, telePortalA.linkRotation);
            }
            if (inPortalB)
            {
                if (telePortalA.isLinkFlipped)
                {
                    teleStepAB = PixelMath.RotateVectorInt(PixelMath.ReflectVectorInt(step, telePortalA.teleNormal), telePortalB.linkRotation);
                }
                else
                {
                    teleStepAB = PixelMath.RotateVectorInt(step, telePortalA.linkRotation);
                }
                if (telePortalB.isLinkFlipped)
                {
                    teleStepAB = PixelMath.RotateVectorInt(PixelMath.ReflectVectorInt(teleStepAB, telePortalA.teleNormal), telePortalB.linkRotation);
                }
                else
                {
                    teleStepAB = PixelMath.RotateVectorInt(teleStepAB, telePortalA.linkRotation);
                }
            }
        }
        if (inPortalB)
        {
            if (telePortalB.isLinkFlipped)
            {
                teleStepB = PixelMath.RotateVectorInt(PixelMath.ReflectVectorInt(step, telePortalB.teleNormal), telePortalB.linkRotation);
            }
            else
            {
                teleStepB = PixelMath.RotateVectorInt(step, telePortalB.linkRotation);
            }
        }
        base.Move(step);
        teleColA.Move(teleStepA);
        teleColB.Move(teleStepB);
        teleColAB.Move(teleStepAB);
        
        List<PixelBody> overlapPortals = new List<PixelBody>();
        overlapPortals.AddRange(pixelPhysics.Scan(col.shape, (int)PixelPhysics.layers.portal));
        if (inPortalA)
        {
            if (!overlapPortals.Contains(telePortalA))
            {
                ExitPortal(true, step);
            }
        }
        if (inPortalB)
        {
            if (!overlapPortals.Contains(telePortalB))
            {
                ExitPortal(false, step);
            }
        }
        if (!inPortalA)            // No else here in case the player exits one portal and enters another in the same frame.
        {
            if (overlapPortals.Count > 0)
            {
                if(inPortalB && (PixelPortal)overlapPortals[0] == telePortalB)
                {
                    if(overlapPortals.Count > 1)
                    {
                        EnterPortal((PixelPortal)overlapPortals[1], step);
                    }
                }
                else
                {
                    EnterPortal((PixelPortal)overlapPortals[0], step);
                }
            }
        }
        if(inPortalA && !inPortalB)
        {
            if (overlapPortals.Count > 1)
            {
                if ((PixelPortal)overlapPortals[0] == telePortalA)
                {
                    EnterPortal((PixelPortal)overlapPortals[1], step);
                }
                else
                {
                    EnterPortal((PixelPortal)overlapPortals[0], step);
                }
            }
        }
    }

    public virtual void EnterPortal(PixelPortal portal, Vector2Int step)
    {
        Vector2 teleDelta;
        PixelCollider curCol;
        SpriteRenderer curSprend;
        if(inPortalA)
        {
            inPortalB = true;
            telePortalB = portal;
            curCol = teleColB;
            curSprend = teleSprendB;
        }
        else
        {
            inPortalA = true;
            telePortalA = portal;
            curCol = teleColA;
            curSprend = teleSprendA;
        }
        curCol.isActive = true;
        curSprend.enabled = true;
        if(inPortalA && inPortalB)
        {
            teleColAB.isActive = true;
            teleSprendAB.gameObject.SetActive(true);
        }
        float cos = Mathf.Cos(portal.linkRotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(portal.linkRotation * Mathf.Deg2Rad);
        if (portal.isLinkFlipped)
        {
            curCol.Reflect(portal.start, portal.end, portal.teleNormal);
            curCol.Rotate(portal.start, portal.linkRotation);
            curCol.Move(portal.linkOffset);

            curSprend.transform.RotateAround(new Vector3(portal.start.x + portal.end.x + 1f, portal.start.y + portal.end.y + 1f) / 2f, new Vector3(Mathf.Cos(portal.teleNormal), Mathf.Sin(portal.teleNormal), 0f), 180f);
            curSprend.transform.RotateAround(new Vector3(portal.start.x + cos, portal.start.y + sin), Vector3.forward, portal.linkRotation);
            curSprend.transform.position += new Vector3(portal.linkOffset.x, portal.linkOffset.y);
            curSprend.transform.position = new Vector3(Mathf.RoundToInt(curSprend.transform.position.x), Mathf.RoundToInt(curSprend.transform.position.y), Mathf.RoundToInt(curSprend.transform.position.z));
            teleDelta = PixelMath.RotateVector(Vector2.Reflect(delta, portal.teleNormalVec), portal.linkRotation);
        }
        else
        {
            curCol.Rotate(portal.start, portal.linkRotation);
            curCol.Move(portal.linkOffset);

            curSprend.transform.RotateAround(new Vector3(portal.start.x + .5f, portal.start.y + .5f), Vector3.forward, portal.linkRotation);
            curSprend.transform.position += new Vector3(portal.linkOffset.x, portal.linkOffset.y);
            curSprend.transform.position = new Vector3(Mathf.Round(curSprend.transform.position.x), Mathf.Round(curSprend.transform.position.y), Mathf.Round(curSprend.transform.position.z));
            teleDelta = PixelMath.RotateVector(delta, portal.linkRotation);
        }
        int boundDispX = Mathf.FloorToInt(teleDelta.x + (0f < teleDelta.x ? 1f : 0f));
        int boundDispY = Mathf.FloorToInt(teleDelta.y + (0f < teleDelta.y ? 1f : 0f));
        curCol.SetBounds(new Vector2Int(boundDispX, boundDispY));
        curCol.nearbyColliders.Clear();
        foreach (PixelCollider other in pixelPhysics.GetNearbyColliders(curCol))
        {
            curCol.nearbyColliders.Add(other);
            other.nearbyColliders.Add(curCol);
        }
        curCol.isActive = true;
        curSprend.gameObject.SetActive(true);

        if(inPortalA && inPortalB)
        {
            if (telePortalA.isLinkFlipped)
            {
                teleColAB.Reflect(telePortalA.start, telePortalA.end, telePortalA.teleNormal);
                teleColAB.Rotate(telePortalA.start, telePortalA.linkRotation);
                teleColAB.Move(telePortalA.linkOffset);

                teleSprendAB.transform.RotateAround(new Vector3(telePortalA.start.x + telePortalA.end.x + 1f, telePortalA.start.y + telePortalA.end.y + 1f) / 2f, new Vector3(Mathf.Cos(telePortalA.teleNormal), Mathf.Sin(telePortalA.teleNormal), 0f), 180f);
                teleSprendAB.transform.RotateAround(new Vector3(telePortalA.start.x + cos, telePortalA.start.y + sin), Vector3.forward, telePortalA.linkRotation);
                teleSprendAB.transform.position += new Vector3(telePortalA.linkOffset.x, telePortalA.linkOffset.y);
                teleSprendAB.transform.position = new Vector3(Mathf.RoundToInt(teleSprendAB.transform.position.x), Mathf.RoundToInt(teleSprendAB.transform.position.y), Mathf.RoundToInt(teleSprendAB.transform.position.z));
            }
            else
            {
                teleColAB.Rotate(telePortalA.start, telePortalA.linkRotation);
                teleColAB.Move(telePortalA.linkOffset);

                teleSprendAB.transform.RotateAround(new Vector3(telePortalA.start.x + .5f, telePortalA.start.y + .5f), Vector3.forward, telePortalA.linkRotation);
                teleSprendAB.transform.position += new Vector3(telePortalA.linkOffset.x, telePortalA.linkOffset.y);
                teleSprendAB.transform.position = new Vector3(Mathf.Round(teleSprendAB.transform.position.x), Mathf.Round(teleSprendAB.transform.position.y), Mathf.Round(teleSprendAB.transform.position.z));
            }
            if (telePortalB.isLinkFlipped)
            {
                teleColAB.Reflect(telePortalB.start, telePortalB.end, telePortalB.teleNormal);
                teleColAB.Rotate(telePortalB.start, telePortalB.linkRotation);
                teleColAB.Move(telePortalB.linkOffset);

                teleSprendAB.transform.RotateAround(new Vector3(telePortalB.start.x + telePortalB.end.x + 1f, telePortalB.start.y + telePortalB.end.y + 1f) / 2f, new Vector3(Mathf.Cos(telePortalB.teleNormal), Mathf.Sin(telePortalB.teleNormal), 0f), 180f);
                teleSprendAB.transform.RotateAround(new Vector3(telePortalB.start.x + cos, telePortalB.start.y + sin), Vector3.forward, telePortalB.linkRotation);
                teleSprendAB.transform.position += new Vector3(telePortalB.linkOffset.x, telePortalB.linkOffset.y);
                teleSprendAB.transform.position = new Vector3(Mathf.RoundToInt(teleSprendAB.transform.position.x), Mathf.RoundToInt(teleSprendAB.transform.position.y), Mathf.RoundToInt(teleSprendAB.transform.position.z));
            }
            else
            {
                teleColAB.Rotate(telePortalB.start, telePortalB.linkRotation);
                teleColAB.Move(telePortalB.linkOffset);

                teleSprendAB.transform.RotateAround(new Vector3(telePortalB.start.x + .5f, telePortalB.start.y + .5f), Vector3.forward, telePortalB.linkRotation);
                teleSprendAB.transform.position += new Vector3(telePortalB.linkOffset.x, telePortalB.linkOffset.y);
                teleSprendAB.transform.position = new Vector3(Mathf.Round(teleSprendAB.transform.position.x), Mathf.Round(teleSprendAB.transform.position.y), Mathf.Round(teleSprendAB.transform.position.z));
            }
            curCol.SetBounds(new Vector2Int(boundDispX, boundDispY));
            curCol.nearbyColliders.Clear();
            foreach (PixelCollider other in pixelPhysics.GetNearbyColliders(curCol))
            {
                curCol.nearbyColliders.Add(other);
                other.nearbyColliders.Add(curCol);
            }
            curCol.isActive = true;
            curSprend.enabled = true;
        }
    }

    public virtual void ExitPortal(bool isPortalA, Vector2Int step)
    {
        PixelPortal curPortal;
        PixelCollider curCol;
        SpriteRenderer curSprend;
        if(isPortalA)
        {
            inPortalA = false;
            curPortal = telePortalA;
            curCol = teleColA;
            curSprend = teleSprendA;
        }
        else
        {
            inPortalB = false;
            curPortal = telePortalB;
            curCol = teleColB;
            curSprend = teleSprendB;
        }
        if (Math.Abs(step.y) < Math.Abs(step.x))
        {
            if (curPortal.teleNormalVec.x != 0 && Math.Sign(step.x) == -Math.Sign(curPortal.teleNormalVec.x))
            {
                TeleportThrough(isPortalA);
            }
            else
            {
                TeleportBack(isPortalA);
            }
        }
        else
        {
            if (curPortal.teleNormalVec.y != 0 && Math.Sign(step.y) == -Math.Sign(curPortal.teleNormalVec.y))
            {

                TeleportThrough(isPortalA);
            }
            else
            {
                TeleportBack(isPortalA);
            }
        }

        if (isPortalA) telePortalA = null;
        else telePortalB = null;
        curCol.isActive = false;
        curSprend.gameObject.SetActive(false);
        teleColAB.isActive = false;
        teleSprendAB.gameObject.SetActive(false);
    }

    protected virtual void TeleportThrough(bool isPortalA)
    {
        Vector2 teleDelta;
        Vector2 teleVel;
        PixelPortal curPortal;
        PixelCollider curCol;
        SpriteRenderer curSprend;
        PixelPortal otherPortal;
        PixelCollider otherCol;
        SpriteRenderer otherSprend;

        if (isPortalA)
        {
            curPortal = telePortalA;
            curCol = teleColA;
            curSprend = teleSprendA;
            otherPortal = telePortalB;
            otherCol = teleColB;
            otherSprend = teleSprendB;
        }
        else
        {
            curPortal = telePortalB;
            curCol = teleColB;
            curSprend = teleSprendB;
            otherPortal = telePortalA;
            otherCol = teleColA;
            otherSprend = teleSprendA;
        }

        col.shape = (PixelShape)curCol.shape.Clone();
        transform.rotation = curSprend.transform.rotation;
        transform.position = curSprend.transform.position;

        curSprend.transform.localRotation = Quaternion.identity;
        curSprend.transform.localPosition = Vector3.zero;

        if(otherPortal == null)
        {
            otherCol.shape = (PixelShape)col.shape.Clone();
        }
        else
        {
            otherCol.shape = (PixelShape)teleColAB.shape.Clone();
        }
        //otherSprend.transform.localRotation = teleSprendAB.transform.localRotation;
        //otherSprend.transform.localPosition = teleSprendAB.transform.localPosition;

        teleColAB.shape = (PixelShape)col.shape.Clone();
        teleSprendAB.transform.localRotation = Quaternion.identity;
        teleSprendAB.transform.localPosition = Vector3.zero;

        col.bounds = curCol.bounds;
        col.nearbyColliders = curCol.nearbyColliders;

        if (curPortal.isLinkFlipped)
        {
            teleDelta = PixelMath.RotateVector(Vector2.Reflect(delta, curPortal.teleNormalVec), curPortal.linkRotation);
            teleVel = PixelMath.RotateVector(Vector2.Reflect(vel, curPortal.teleNormalVec), curPortal.linkRotation);
        }
        else
        {
            teleDelta = PixelMath.RotateVector(delta, curPortal.linkRotation);
            teleVel = PixelMath.RotateVector(vel, curPortal.linkRotation);
        }

        delta = teleDelta;
        vel = teleVel;
    }

    protected void TeleportBack(bool isPortalA)
    {
        PixelCollider curCol;
        SpriteRenderer curSprend;

        if (isPortalA)
        {
            curCol = teleColA;
            curSprend = teleSprendA;
        }
        else
        {
            curCol = teleColB;
            curSprend = teleSprendB;
        }

        curCol.shape = (PixelShape)col.shape.Clone();
        curSprend.transform.localRotation = Quaternion.identity;
        curSprend.transform.localPosition = Vector3.zero;

        teleColAB.shape = (PixelShape)col.shape.Clone();
        teleSprendAB.transform.localRotation = Quaternion.identity;
        teleSprendAB.transform.localPosition = Vector3.zero;
    }

    public override void PrePhysics()
    {
        base.PrePhysics();
        /*
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
        */
    }

    public override List<PixelCollider> GetActiveColliders()
    {
        List<PixelCollider> ret = new List<PixelCollider>();
        if (isActive)
        {
            ret.Add(col);
            if(inPortalA)
            {
                ret.Add(teleColA);
                if(inPortalB)
                {
                    ret.Add(teleColAB);
                }
            }
            if(inPortalB)
            {
                ret.Add(teleColB);
            }
        }
        return ret;
    }
}
