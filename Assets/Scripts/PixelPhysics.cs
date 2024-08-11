using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PixelPhysics : MonoBehaviour
{
    public enum layers
    {
        none            = 0b_0000_0000,  // 0
        structure       = 0b_0000_0001,  // 1
        interactable    = 0b_0000_0010,  // 2
        hazard          = 0b_0000_0100,  // 4
        player          = 0b_0000_1000,  // 8
        creature        = 0b_0001_0000,  // 16
        projectile      = 0b_0010_0000,  // 32
        portal          = 0b_0100_0000,  // 64
        effect          = 0b_1000_0000,  // 128
        all             = 0b_1111_1111   // MaxValue
    }
    public static PixelPhysics instance { get; private set; }
    public List<PixelBody> pixelBodys;
    private List<PixelBody> dynamicBodys;
    private List<PixelBody> colliderObjects;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Setup();
        if (dynamicBodys.Count > 0)
        {
            Movement();
        }
        //Scanning();
        Cleanup();
    }

    public void RegisterBody(PixelBody body)
    {
        pixelBodys.Add(body);
    }

    public void RemoveObject(PixelBody body)
    {
        if (pixelBodys.Contains(body))
        {
            pixelBodys.Remove(body);
        }
    }

    private void Setup()
    {
        dynamicBodys = pixelBodys.Where(body => body.isDynamic).ToList();
        //colliderObjects = pixelBodys.Where(body => body.colliders.Count > 0).ToList();
        foreach (PixelBody body in pixelBodys)
        {
            body.PrePhysics();
        }

        foreach (PixelBody body in dynamicBodys)
        {
            foreach (PixelCollider curCol in body.GetActiveColliders())
            {
                curCol.nearbyColliders = GetNearbyColliders(curCol);
            }
        }
    }

    private void Movement()
    {
        bool isMarching = true;
        bool lateCycle = false;
        PixelBody curBody;
        PixelBody otherBody;
        Vector2Int step;
        float maxX;
        float maxY;
        float maxRatioX;
        float maxRatioY;
        int maxXI;
        int maxYI;
        bool colPhantom;
        // Loop through all kinematic and dynamic pixelBodys, finding the one with the largest x or y delta. Move that one in the specified direction or trigger a collision if dynamic.
        // Exits when no bodyect has a delta.x or delta.y greater than .5f
        while (isMarching)
        {
            maxX = 0f;
            maxY = 0f;
            maxRatioX = 0f;
            maxRatioY = 0f;
            maxXI = 0;
            maxYI = 0;
            colPhantom = false;

            for (int i = 0; i < dynamicBodys.Count; i++)
            {
                curBody = dynamicBodys[i];
                float absX = Mathf.Abs(curBody.delta.x);
                float absY = Mathf.Abs(curBody.delta.y);
                float absSpentX = Mathf.Abs(curBody.spentDelta.x);
                float absSpentY = Mathf.Abs(curBody.spentDelta.y);
                float ratioX = (absX + absSpentX) / (absSpentX + 1);
                float ratioY = (absY + absSpentY) / (absSpentY + 1);
                if (maxRatioX < ratioX && !dynamicBodys[i].xMovementDone)
                {
                    maxRatioX = ratioX;
                    maxX = absX;
                    maxXI = i;
                }
                if (maxRatioY < ratioY && !dynamicBodys[i].yMovementDone)
                {
                    maxRatioY = ratioY;
                    maxY = absY;
                    maxYI = i;
                }
            }

            if ((maxX == 0f) && (maxY == 0f))
            {
                if (lateCycle)
                {
                    isMarching = false;
                }
                else
                {
                    foreach (PixelBody body in dynamicBodys)
                    {
                        body.delta += body.lateDelta;
                        body.lateDelta = Vector2.zero;
                        lateCycle = true;
                    }
                }
            }
            else
            {
                if (maxY < maxX)
                {
                    curBody = dynamicBodys[maxXI];
                    step = new Vector2Int(Math.Sign(curBody.delta.x), 0);
                    curBody.xMovementDone = Math.Abs(curBody.delta.x) < .5f;
                }
                else
                {
                    curBody = dynamicBodys[maxYI];
                    step = new Vector2Int(0, Math.Sign(curBody.delta.y));
                    curBody.yMovementDone = Math.Abs(curBody.delta.y) < .5f;
                }
                foreach (PixelCollider curCol in curBody.GetActiveColliders())
                {
                    foreach (PixelCollider otherCol in curCol.nearbyColliders)
                    {
                        if (CheckCollision(curCol.shape, otherCol.shape, step))
                        {
                            otherBody = otherCol.body;
                            int normal = GetCollisionNormal(curCol, otherCol, step);
                            switch (HandleCollision(curBody, otherBody, step, normal))
                            {
                                case 0:
                                    //Normal Collision
                                    curBody.xMovementDone = true;
                                    curBody.yMovementDone = true;
                                    otherBody.xMovementDone = true;
                                    otherBody.yMovementDone = true;
                                    break;

                                case 1:
                                    //Bad Normal/Clipping
                                    curBody.xMovementDone = true;
                                    curBody.yMovementDone = true;
                                    otherBody.xMovementDone = true;
                                    otherBody.yMovementDone = true;
                                    break;

                                case 2:
                                    //Cancelled Collision
                                    break;

                                case 3:
                                    //Phantom Collision
                                    colPhantom = true;
                                    break;
                            }
                            break;
                        }
                    }
                }
                if ((step.x != 0 && !curBody.xMovementDone) || (step.y != 0 && !curBody.yMovementDone))
                {
                    if (colPhantom)
                    {
                        curBody.delta -= step;
                        curBody.lateDelta += step;
                    }
                    else
                    {
                        curBody.Move(step);
                        curBody.delta -= step;
                        curBody.spentDelta += step;
                    }
                }
            }
        }
    }

    /// I think scanning should be done as needed, instead of having its own phase.
    /// This code is VERY outdated in case I change my mind.
    //private void Scanning()
    //{
    //    foreach (PixelBody body in pixelBodys)
    //    {
    //        if (body.isScanner)
    //        {
    //            body.scannedObjects = Scan(body.shape, pixelBodys.Where(other => (other.layers & body.scanMask) != 0));
    //        }
    //    }
    //}

    private void Cleanup()
    {
        foreach (PixelBody body in pixelBodys)
        {
            body.PostPhysics();
            if (body.isDead)
            {
                RemoveObject(body);
            }
        }
    }

    // Returns a list of all bodies with colliders matching col.mask overlapping col.
    // otherCol.mask is not checked (Scanning colliders should have layers = 0x0000 so they don't interact with anything.)
    public List<PixelBody> Scan(PixelShape shape, int mask)
    {
        List<PixelBody> ret = new List<PixelBody>();
        foreach (PixelBody otherBody in pixelBodys)
        {
            PixelCollider otherCol = otherBody.col;
            if ((otherCol.layers & mask) != 0 && PixelShape.CheckOverlap(shape, otherCol.shape))
            {
                ret.Add(otherBody);
            }
        }
        return ret;
    }

    public List<PixelCollider> GetNearbyColliders(PixelCollider col)
    {
        List<PixelCollider> ret = new List<PixelCollider>();
        foreach (PixelBody otherBody in pixelBodys.Where(otherBody => otherBody != col.body))
        {
            foreach (PixelCollider otherCol in otherBody.GetActiveColliders())
            {
                if ((col.layers & otherCol.mask) != 0 && (otherCol.layers & col.mask) != 0 && PixelShape.CheckOverlap(col.bounds, otherCol.bounds))
                {
                    ret.Add(otherCol);
                }
            }
        }
        return ret;
    }

    // Resolves collision between bodyA and bodyB, where bodyA moved into bodyB while attempting to move along step.
    // Returns 0 for a valid collision, 1 for a bad normal (clipping), 2 for a failed precheck, 3 for a phantom collision.
    private static int HandleCollision(PixelBody bodyA, PixelBody bodyB, Vector2Int step, int normal)
    {   
        if (normal < 0)
        {
            Debug.Log("colBadNormal");
            return 1;
        }

        bool preCheckA = bodyA.PreCollision(bodyB, normal);
        bool preCheckB = bodyB.PreCollision(bodyA, (normal + 180) % 360);

        if (!preCheckA || !preCheckB)
        {
            return 2;
        }

        bool colCheck;
        if (bodyB.isDynamic)
        {
            colCheck = DynamicCollision(bodyA, bodyB, normal);
        }
        else
        {
            colCheck = StaticCollision(bodyA, bodyB, normal);
        }

        if (!colCheck)
        {
            return 3;
        }

        bodyA.PostCollision(bodyB, normal);
        bodyB.PostCollision(bodyA, (normal + 180) % 360);

        return 0;
    }

    private static bool StaticCollision(PixelBody bodyA, PixelBody bodyB, int normal)
    {
        float elasticProd = bodyA.elastic * bodyB.elastic;
        float frictionProd = bodyA.friction * bodyB.friction;
        float speed = bodyA.vel.magnitude;
        float velAngle = Vector2.SignedAngle(Vector2.right, bodyA.vel);
        float impactAngle = (velAngle - normal + 360f) % 360f; 
        float normalFactor = Mathf.Cos(impactAngle * Mathf.Deg2Rad);
        float parallelFactor = -Mathf.Sin(impactAngle * Mathf.Deg2Rad);
        float frictionFactor = Mathf.Abs(normalFactor) * frictionProd;
        float bounceFactor = -normalFactor * elasticProd;
        float normalVel = speed * bounceFactor;
        float parallelVel;

        if (normalFactor >= 0)
        {
            return false;
        }

        if (Mathf.Abs(parallelFactor) <= Mathf.Abs(frictionFactor * normalFactor))
        {
            parallelVel = 0;
        }
        else
        {
            parallelVel = speed * (parallelFactor - (Mathf.Abs(frictionFactor * normalFactor) * Mathf.Sign(parallelFactor)));
        }

        float sinNorm = Mathf.Sin(normal * Mathf.Deg2Rad);
        float cosNorm = Mathf.Cos(normal * Mathf.Deg2Rad);
        float newVelX = normalVel * cosNorm + parallelVel * sinNorm;
        newVelX = Math.Abs(newVelX) < 0.1f ? 0f : newVelX;
        float newVelY = normalVel * sinNorm - parallelVel * cosNorm;
        newVelY = Math.Abs(newVelY) < 0.1f ? 0f : newVelY;
        bodyA.vel = new Vector2(newVelX, newVelY);
        bodyA.delta = Vector2.zero;// bodyA.vel.normalized * bodyA.delta.magnitude;
        return true;
    }

    private static bool KinematicCollision(PixelBody bodyA, PixelBody bodyB, int normal)
    {
        float massSum = bodyA.mass + bodyB.mass;
        float elasticProd = bodyA.elastic * bodyB.elastic;
        float frictionProd = bodyA.friction * bodyB.friction;
        float speedA = bodyA.vel.magnitude * bodyA.mass;
        float velAngleA = Vector2.SignedAngle(Vector2.right, bodyA.vel);
        float impactAngleA = (velAngleA - normal + 360f) % 360f;
        float normalVelA = speedA * Mathf.Cos(impactAngleA * Mathf.Deg2Rad);
        float parallelVelA = speedA * -Mathf.Sin(impactAngleA * Mathf.Deg2Rad);

        float speedB = bodyB.vel.magnitude * bodyB.mass;
        float velAngleB = Vector2.SignedAngle(Vector2.right, bodyB.vel);
        float impactAngleB = (velAngleB - normal + 360f) % 360f;
        float normalVelB = speedB * Mathf.Cos(impactAngleB * Mathf.Deg2Rad);
        float parallelVelB = speedB * -Mathf.Sin(impactAngleB * Mathf.Deg2Rad);

        float bounceA = elasticProd * bodyB.mass * (normalVelB - normalVelA) / massSum;
        float frictionA = frictionProd * bodyB.mass * (normalVelA - normalVelB) / massSum;
        float resultNormalVelA = bounceA + (normalVelA * bodyA.mass + normalVelB * bodyB.mass) / massSum;
        float resultParallelVelA = parallelVelA - Mathf.Min(Mathf.Abs(frictionA), Mathf.Abs(parallelVelA)) * Mathf.Sign(parallelVelA) / bodyA.mass;
        float sinNorm = Mathf.Sin(normal * Mathf.Deg2Rad);
        float cosNorm = Mathf.Cos(normal * Mathf.Deg2Rad);
        float newVelX = resultNormalVelA * cosNorm + resultParallelVelA * sinNorm;
        float newVelY = resultNormalVelA * sinNorm - resultParallelVelA * cosNorm;
        bodyA.vel = new Vector2(newVelX, newVelY);
        bodyA.delta = bodyA.vel.normalized * bodyA.delta.magnitude;
        return true;
    }

    // Collision between two dynamic bodyects along normal.
    // Skips collision and returns false for phantom hits, bodyects moving relatively away that clip one another.
    private static bool DynamicCollision(PixelBody bodyA, PixelBody bodyB, int normal)
    {
        float massSum = bodyA.mass + bodyB.mass;
        float elasticProd = bodyA.elastic * bodyB.elastic;
        float frictionProd = bodyA.friction * bodyB.friction;

        float speedA = bodyA.vel.magnitude;
        float velAngleA = Vector2.SignedAngle(Vector2.right, bodyA.vel);
        float impactAngleA = (velAngleA - normal + 360f) % 360f;
        float normalVelA = speedA * Mathf.Cos(impactAngleA * Mathf.Deg2Rad);
        float parallelVelA = speedA * -Mathf.Sin(impactAngleA * Mathf.Deg2Rad);

        float speedB = bodyB.vel.magnitude;
        float velAngleB = Vector2.SignedAngle(Vector2.right, bodyB.vel);
        float impactAngleB = (velAngleB - normal + 360f) % 360f;
        float normalVelB = speedB * Mathf.Cos(impactAngleB * Mathf.Deg2Rad);
        float parallelVelB = speedB * -Mathf.Sin(impactAngleB * Mathf.Deg2Rad);

        if (normalVelB <= normalVelA)
        {
            return false;
        }

        float normalMoment = (normalVelA * bodyA.mass + normalVelB * bodyB.mass);

        float bounceA = Mathf.Max(elasticProd * bodyB.mass * (normalVelB - normalVelA) / massSum, 0f);
        float frictionA = frictionProd * bodyB.mass * (normalVelA - normalVelB) / massSum;
        float resultNormalVelA = bounceA + normalMoment / massSum;
        float resultParallelVelA = parallelVelA - Mathf.Min(Mathf.Abs(frictionA), Mathf.Abs(parallelVelA)) * Mathf.Sign(parallelVelA) / bodyA.mass;

        float bounceB = Mathf.Min(elasticProd * bodyA.mass * (normalVelA - normalVelB) / massSum, 1f);
        float frictionB = frictionProd * bodyA.mass * (normalVelB - normalVelA) / massSum;
        float resultNormalVelB = bounceB + normalMoment / massSum;
        float resultParallelVelB = parallelVelB - Mathf.Min(Mathf.Abs(frictionB), Mathf.Abs(parallelVelB)) * Mathf.Sign(parallelVelB) / bodyB.mass;

        float sinNorm = Mathf.Sin(normal * Mathf.Deg2Rad);
        float cosNorm = Mathf.Cos(normal * Mathf.Deg2Rad);
        float newVelAX = resultNormalVelA * cosNorm + resultParallelVelA * sinNorm;
        newVelAX = Math.Abs(newVelAX) < 0.1f ? 0f : newVelAX;
        float newVelAY = resultNormalVelA * sinNorm - resultParallelVelA * cosNorm;
        newVelAY = Math.Abs(newVelAY) < 0.1f ? 0f : newVelAY;
        float newVelBX = resultNormalVelB * cosNorm + resultParallelVelB * sinNorm;
        newVelBX = Math.Abs(newVelBX) < 0.1f ? 0f : newVelBX;
        float newVelBY = resultNormalVelB * sinNorm - resultParallelVelB * cosNorm;
        newVelBY = Math.Abs(newVelBY) < 0.1f ? 0f : newVelBY;
        bodyA.vel = new Vector2(newVelAX, newVelAY);
        bodyB.vel = new Vector2(newVelBX, newVelBY);
        bodyA.delta = Vector2.zero;
        bodyB.delta = Vector2.zero;
        return true;
    }

    private static int GetCollisionNormal(PixelCollider colA, PixelCollider colB, Vector2Int step)
    {
        int normal = -1;
        PixelShape shapeA = colA.shape;
        PixelShape shapeB = colB.shape;
        if (shapeB is PixelLine)
        {
            normal = GetLineNormal((PixelLine)shapeB, step);
        }
        else if (shapeB is PixelRect)
        {
            if (shapeA is PixelLine)
            {
                normal = GetLineNormal((PixelLine)shapeA, -step);
            }
            else if (shapeA is PixelRect)
            {
                if (step.x < 0)
                {
                    return 0;
                }
                else if (0 < step.x)
                {
                    return 180;
                }
                else if (step.y < 0)
                {
                    return 90;
                }
                else
                {
                    return 270;
                }
            }
            else if (shapeA is PixelRhomb)
            {
                PixelRhomb rhombA = (PixelRhomb)shapeA;
                PixelRect rectB = (PixelRect)shapeB;
                if (rhombA.center.x < rectB.left)
                {
                    if (rhombA.center.y < rectB.down)
                    {
                        normal = 225;
                    }
                    else if (rectB.up < rhombA.center.y)
                    {
                        normal = 135;
                    }
                    else
                    {
                        normal = 180;
                    }
                }
                else if (rectB.right < rhombA.center.x)
                {
                    if (rhombA.center.y < rectB.down)
                    {
                        normal = 315;
                    }
                    else if (rectB.up < rhombA.center.y)
                    {
                        normal = 45;
                    }
                    else
                    {
                        normal = 0;
                    }
                }
                else
                {
                    if (rhombA.center.y < rectB.up)
                    {
                        normal = 270;
                    }
                    else
                    {
                        normal = 90;
                    }
                }
            }
            else if (shapeA is PixelDiag)
            {
                PixelDiag diagA = (PixelDiag)shapeA;
                PixelRect rectB = (PixelRect)shapeB;
                if (diagA.rightVert.x < rectB.left)
                {
                    normal = 180;
                }
                else if (rectB.right < diagA.leftVert.x)
                {
                    normal = 0;
                }
                else if (diagA.upVert.y < rectB.down)
                {
                    normal = 270;
                }
                else if (rectB.up < diagA.downVert.y)
                {
                    normal = 90;
                }
                else
                {
                    int fromLeft = rectB.left - diagA.rightVert.x;
                    int fromRight = diagA.leftVert.x - rectB.right;
                    int fromDown = rectB.down - diagA.upVert.y;
                    int fromUp = diagA.downVert.y - rectB.up;
                    if (0 < fromLeft + fromDown + diagA.downDiag)
                    {
                        normal = 225;
                    }
                    else if (0 < fromLeft + fromUp + diagA.upDiag)
                    {
                        normal = 135;
                    }
                    else if (0 < fromRight + fromDown + diagA.upDiag)
                    {
                        normal = 315;
                    }
                    else if (0 < fromRight + fromUp + diagA.downDiag)
                    {
                        normal = 45;
                    }
                }
            }
            else if (shapeA is PixelOct)
            {
                PixelOct octA = (PixelOct)shapeA;
                PixelRect rectB = (PixelRect)shapeB;
                if (octA.right < rectB.left) 
                {
                    normal = 180;
                }
                else if (rectB.right < octA.left)
                {
                    normal = 0;
                }
                else if (octA.up < rectB.down)
                {
                    normal = 270;
                }
                else if (rectB.up < octA.down)
                {
                    normal = 90;
                }
                else if (octA.right - rectB.left + octA.up - rectB.down <= octA.ruDiag)
                {
                    normal = 225;
                }
                else if (octA.right - rectB.left + rectB.up - octA.down <= octA.rdDiag)
                {
                    normal = 135;
                }
                else if (rectB.right - octA.left + octA.up - rectB.down <= octA.luDiag)
                {
                    normal = 315;
                }
                else if (rectB.right - octA.left + rectB.up - octA.down <= octA.rdDiag)
                {
                    normal = 45;
                }
            }
        }
        else if (shapeB is PixelRhomb)
        {
            if (shapeA is PixelLine)
            {
                normal = GetLineNormal((PixelLine)shapeA, -step);
            }
            else if (shapeA is PixelRect)
            {
                PixelRect rectA = (PixelRect)shapeA;
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                if (rectA.right < rhombB.center.x)
                {
                    if (rectA.up < rhombB.center.y)
                    {
                        normal = 225;
                    }
                    else if (rhombB.center.y < rectA.down)
                    {
                        normal = 135;
                    }
                    else
                    {
                        normal = 180;
                    }
                }
                else if (rhombB.center.x < rectA.left)
                {
                    if (rectA.up < rhombB.center.y)
                    {
                        normal = 315;
                    }
                    else if (rhombB.center.y < rectA.down)
                    {
                        normal = 45;
                    }
                    else
                    {
                        normal = 0;
                    }
                }
                else
                {
                    if (rectA.down < rhombB.center.y)
                    {
                        normal = 270;
                    }
                    else
                    {
                        normal = 90;
                    }
                }
            }
            else if (shapeA is PixelRhomb)
            {
                PixelRhomb rhombA = (PixelRhomb)shapeA;
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                if (rhombA.center.x < rhombB.center.x)
                {
                    if (rhombA.center.y < rhombB.center.y)
                    {
                        normal = 225;
                    }
                    else if (rhombB.center.y < rhombA.center.y)
                    {
                        normal = 135;
                    }
                    else
                    {
                        normal = 180;
                    }
                }
                else if (rhombB.center.y < rhombA.center.x)
                {
                    if (rhombA.center.y < rhombB.center.y)
                    {
                        normal = 315;
                    }
                    else if (rhombB.center.y < rhombA.center.y)
                    {
                        normal = 45;
                    }
                    else
                    {
                        normal = 0;
                    }
                }
                else
                {
                    if (rhombA.center.y < rhombB.center.x)
                    {
                        normal = 270;
                    }
                    else
                    {
                        normal = 90;
                    }
                }
            }
            else if (shapeA is PixelOct)
            {
                PixelOct octA = (PixelOct)shapeA;
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                if (octA.right < rhombB.center.x - rhombB.diagonal)
                {
                    normal = 180;
                }
                else if (rhombB.center.x + rhombB.diagonal < octA.left)
                {
                    normal = 0;
                }
                if (octA.up < rhombB.center.y - rhombB.diagonal)
                {
                    normal = 270;
                }
                else if (rhombB.center.y + rhombB.diagonal < octA.down)
                {
                    normal = 90;
                }
                else if (octA.right - rhombB.center.x + octA.up - rhombB.center.y <= octA.ruDiag + rhombB.diagonal)
                {
                    normal = 225;
                }
                else if (octA.right - rhombB.center.x + rhombB.center.y - octA.up <= octA.ldDiag + rhombB.diagonal)
                {
                    normal = 135;
                }
                else if (rhombB.center.x - octA.right + octA.up - rhombB.center.y <= octA.ruDiag + rhombB.diagonal)
                {
                    normal = 315;
                }
                else if (rhombB.center.x - octA.right + rhombB.center.y - octA.up <= octA.ruDiag + rhombB.diagonal)
                {
                    normal = 45;
                }
                //TODO fix above code with oct diagonals
            }
        }
        else if (shapeB is PixelDiag)
        {
            if (shapeA is PixelLine)
            {
                normal = GetLineNormal((PixelLine)shapeA, -step);
            }
            else if (shapeA is PixelRect)
            {
                PixelRect rectA = (PixelRect)shapeA;
                PixelDiag diagB = (PixelDiag)shapeB;
                if (rectA.right < diagB.leftVert.x)
                {
                    normal = 180;
                }
                else if (diagB.rightVert.x < rectA.left)
                {
                    normal = 0;
                }
                else if (rectA.up < diagB.downVert.y)
                {
                    normal = 270;
                }
                else if (diagB.upVert.y < rectA.down)
                {
                    normal = 90;
                }
                else
                {
                    int fromLeft = diagB.leftVert.x - rectA.right;
                    int fromRight = rectA.left - diagB.rightVert.x;
                    int fromDown = diagB.downVert.y - rectA.up;
                    int fromUp = rectA.down - diagB.upVert.y;
                    if (0 < fromLeft + fromDown + diagB.downDiag)
                    {
                        normal = 225;
                    }
                    else if (0 < fromLeft + fromUp + diagB.upDiag)
                    {
                        normal = 135;
                    }
                    else if (0 < fromRight + fromDown + diagB.upDiag)
                    {
                        normal = 315;
                    }
                    else if (0 < fromRight + fromUp + diagB.downDiag)
                    {
                        normal = 45;
                    }
                }
            }
            else if (shapeA is PixelDiag)
            {
                PixelDiag diagA = (PixelDiag)shapeA;
                PixelDiag diagB = (PixelDiag)shapeB;
                if (diagA.rightVert.x < diagB.leftVert.x)
                {
                    normal = 180;
                }
                else if (diagB.rightVert.x < diagA.leftVert.x)
                {
                    normal = 0;
                }
                else if (diagA.upVert.y < diagB.downVert.y)
                {
                    normal = 270;
                }
                else if (diagB.upVert.y < diagA.downVert.y)
                {
                    normal = 90;
                }
                else
                {
                    int fromLeft = diagB.leftVert.x - diagA.rightVert.x;
                    int fromRight = diagA.leftVert.x - diagB.rightVert.x;
                    int fromDown = diagB.downVert.y - diagA.upVert.y;
                    int fromUp = diagA.downVert.y - diagB.upVert.y;
                    if (0 < fromLeft + fromDown + diagA.downDiag + diagB.downDiag)
                    {
                        normal = 225;
                    }
                    else if (0 < fromLeft + fromUp + diagA.upDiag + diagB.upDiag)
                    {
                        normal = 135;
                    }
                    else if (0 < fromRight + fromDown + diagA.upDiag + diagB.upDiag)
                    {
                        normal = 315;
                    }
                    else if (0 < fromRight + fromUp + diagA.downDiag + diagB.downDiag)
                    {
                        normal = 45;
                    }
                }
            }
            else if (shapeA is PixelOct)
            {
                PixelOct octA = (PixelOct)shapeA;
                PixelDiag diagB = (PixelDiag)shapeB;
                if (octA.right < diagB.leftVert.x)
                {
                    normal = 180;
                }
                else if (diagB.rightVert.x < octA.left)
                {
                    normal = 0;
                }
                else if (octA.up < diagB.downVert.y)
                {
                    normal = 270;
                }
                else if (diagB.upVert.y < octA.down)
                {
                    normal = 90;
                }
                else
                {
                    int fromLeft = diagB.leftVert.x - octA.right;
                    int fromRight = octA.left - diagB.rightVert.x;
                    int fromDown = diagB.downVert.y - octA.up;
                    int fromUp = octA.down - diagB.upVert.y;
                    if (0 < fromLeft + fromDown + octA.ldDiag + diagB.downDiag)
                    {
                        normal = 225;
                    }
                    else if (0 < fromLeft + fromUp + octA.luDiag + diagB.upDiag)
                    {
                        normal = 135;
                    }
                    else if (0 < fromRight + fromDown + octA.rdDiag + diagB.upDiag)
                    {
                        normal = 315;
                    }
                    else if (0 < fromRight + fromUp + octA.ruDiag + diagB.downDiag)
                    {
                        normal = 45;
                    }
                }
            }
        }
        else if (shapeB is PixelOct)
        {
            if (shapeA is PixelLine)
            {
                normal = GetLineNormal((PixelLine)shapeA, -step);
            }
            else if (shapeA is PixelRect)
            {
                PixelRect rectA = (PixelRect)shapeA;
                PixelOct octB = (PixelOct)shapeB;
                if (rectA.right < octB.left)
                {
                    normal = 180;
                }
                else if (octB.right < rectA.left)
                {
                    normal = 0;
                }
                if (rectA.up < octB.down)
                {
                    normal = 270;
                }
                else if (octB.up < rectA.down)
                {
                    normal = 90;
                }
                else if (rectA.right - octB.left + rectA.up - octB.down <= octB.ldDiag)
                {
                    normal = 225;
                }
                else if (rectA.right - octB.left + octB.up - rectA.down <= octB.luDiag)
                {
                    normal = 135;
                }
                else if (octB.right - rectA.left + rectA.up - octB.down <= octB.rdDiag)
                {
                    normal = 315;
                }
                else if (octB.right - rectA.left + octB.up - rectA.down <= octB.ruDiag)
                {
                    normal = 45;
                }
            }
            else if (shapeA is PixelRhomb)
            {
                PixelRhomb rhombA = (PixelRhomb)shapeA;
                PixelOct octB = (PixelOct)shapeB;
                if (rhombA.center.x + rhombA.diagonal < octB.left)
                {
                    normal = 180;
                }
                else if (octB.right < rhombA.center.x - rhombA.diagonal)
                {
                    normal = 0;
                }
                else if (rhombA.center.y + rhombA.diagonal < octB.down)
                {
                    normal = 270;
                }
                else if (octB.up < rhombA.center.y - rhombA.diagonal)
                {
                    normal = 90;
                }
                else if (rhombA.center.x - octB.left + rhombA.center.y - octB.down <= rhombA.diagonal + octB.ruDiag)
                {
                    normal = 225;
                }
                else if (rhombA.center.x - octB.left + octB.up - rhombA.center.y <= rhombA.diagonal + octB.ruDiag)
                {
                    normal = 135;
                }
                else if (octB.right - rhombA.center.x + rhombA.center.y - octB.down <= rhombA.diagonal + octB.ruDiag)
                {
                    normal = 315;
                }
                else if (octB.right - rhombA.center.x + octB.up - rhombA.center.y <= rhombA.diagonal + octB.ruDiag)
                {
                    normal = 45;
                }
            }
            else if (shapeA is PixelDiag)
            {
                PixelDiag diagA = (PixelDiag)shapeA;
                PixelOct octB = (PixelOct)shapeB;
                if (diagA.rightVert.x < octB.left)
                {
                    normal = 180;
                }
                else if (octB.right < diagA.leftVert.x)
                {
                    normal = 0;
                }
                else if (diagA.upVert.y < octB.down)
                {
                    normal = 270;
                }
                else if (octB.up < diagA.downVert.y)
                {
                    normal = 90;
                }
                else
                {
                    int fromLeft = octB.left - diagA.rightVert.x;
                    int fromRight = diagA.leftVert.x - octB.right;
                    int fromDown = octB.down - diagA.upVert.y;
                    int fromUp = diagA.downVert.y - octB.up;
                    if (0 < fromLeft + fromDown + diagA.downDiag + octB.ldDiag)
                    {
                        normal = 225;
                    }
                    else if (0 < fromLeft + fromUp + diagA.upDiag + octB.luDiag)
                    {
                        normal = 135;
                    }
                    else if (0 < fromRight + fromDown + diagA.upDiag + octB.rdDiag)
                    {
                        normal = 315;
                    }
                    else if (0 < fromRight + fromUp + diagA.downDiag + octB.ruDiag)
                    {
                        normal = 45;
                    }
                }
            }
            else if (shapeA is PixelOct)
            {
                PixelOct octA = (PixelOct)shapeA;
                PixelOct octB = (PixelOct)shapeB;
                if (octA.right < octB.left)
                {
                    normal = 180;
                }
                else if (octB.right < octA.left)
                {
                    normal = 0;
                }
                if (octA.up < octB.down)
                {
                    normal = 270;
                }
                else if (octB.up < octA.down)
                {
                    normal = 90;
                }
                else if (octA.right - octB.left + octA.up - octB.down <= octA.ruDiag + octB.ldDiag)
                {
                    normal = 225;
                }
                else if (octA.right - octB.left + octB.up - octA.down <= octA.rdDiag + octB.luDiag)
                {
                    normal = 135;
                }
                else if (octB.right - octA.left + octA.up - octB.down <= octA.luDiag + octB.rdDiag)
                {
                    normal = 315;
                }
                else if (octB.right - octA.left + octB.up - octA.down <= octA.ldDiag + octB.rdDiag)
                {
                    normal = 45;
                }
            }
        }
        if (normal == -1) Debug.Log("badNormal");
        return normal;
    }

    private static int GetLineNormal(PixelLine line, Vector2Int step)
    {
        int lineX = line.end.x - line.start.x;
        int lineY = line.end.y - line.start.y;
        if (Math.Abs(lineX) * 2 < Math.Abs(lineY))
        {
            return step.x < 0 ? 0 : 180;
        }
        else if (Math.Abs(lineY) * 2 < Math.Abs(lineX))
        {
            return step.y < 0 ? 90 : 270;
        }
        else if (Math.Sign(lineX) != Math.Sign(lineY))
        {
            return step.x + step.y < 0 ? 45 : 225;
        }
        else
        {
            return 0 < step.x || step.y < 0 ? 135 : 315;
        }
    }

    private static bool CheckCollision(PixelShape shapeA, PixelShape shapeB, Vector2Int disp)
    {
        PixelShape movedShape = (PixelShape)shapeA.Clone();
        movedShape.Move(disp);
        return PixelShape.CheckOverlap(movedShape, shapeB);
    }
}