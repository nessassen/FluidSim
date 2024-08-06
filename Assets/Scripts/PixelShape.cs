using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelShape : ICloneable
{
    // Translates this by disp.
    public virtual void Move(Vector2Int disp) { }

    public virtual void Rotate(Vector2Int o, int a) { }                             // Always rotates/reflects around origin point (even if that point is not within shape).

    public virtual void Rotate(Vector2 o, int a) { }

    public virtual void Reflect(Vector2Int o, int n) { }

    public virtual void Reflect(Vector2Int s, Vector2Int e, int n) { }              // Use this for portals etc.(The midpoint of a portal is a subpixel).

    public virtual void Reflect(Vector2 o, int n) { }

    public virtual Vector2Int GetSize()
    {
        return Vector2Int.zero;
    }

    public virtual PixelRect GetBounds()
    {
        return new PixelRect(0, 0, 0, 0);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public static bool CheckOverlap(PixelShape shapeA, PixelShape shapeB)
    {
        if (shapeA is PixelLine)
        {
            PixelLine lineA = (PixelLine)shapeA;
            if (shapeB is PixelLine)
            {
                PixelLine lineB = (PixelLine)shapeB;
                return LineLineOverlap(lineA, lineB);
            }
            else if (shapeB is PixelRect)
            {
                PixelRect rectB = (PixelRect)shapeB;
                return LineRectOverlap(lineA, rectB);
            }
            else if (shapeB is PixelRhomb)
            {
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                return LineRhombOverlap(lineA, rhombB);
            }
            else if (shapeB is PixelDiag)
            {
                PixelDiag diagB = (PixelDiag)shapeB;
                return LineDiagOverlap(lineA, diagB);
            }
            else if (shapeB is PixelOct)
            {
                PixelOct octB = (PixelOct)shapeB;
                return LineOctOverlap(lineA, octB);
            }
        }
        else if (shapeA is PixelRect)
        {
            PixelRect rectA = (PixelRect)shapeA;
            if (shapeB is PixelLine)
            {
                PixelLine lineB = (PixelLine)shapeB;
                return LineRectOverlap(lineB, rectA);
            }
            else if (shapeB is PixelRect)
            {
                PixelRect rectB = (PixelRect)shapeB;
                return RectRectOverlap(rectA, rectB);
            }
            else if (shapeB is PixelRhomb)
            {
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                return RectRhombOverlap(rectA, rhombB);
            }
            else if (shapeB is PixelDiag)
            {
                PixelDiag diagB = (PixelDiag)shapeB;
                return RectDiagOverlap(rectA, diagB);
            }
            else if (shapeB is PixelOct)
            {
                PixelOct octB = (PixelOct)shapeB;
                return RectOctOverlap(rectA, octB);
            }
        }
        else if (shapeA is PixelRhomb)
        {
            PixelRhomb rhombA = (PixelRhomb)shapeA;
            if (shapeB is PixelLine)
            {
                PixelLine lineB = (PixelLine)shapeB;
                return LineRhombOverlap(lineB, rhombA);
            }
            else if (shapeB is PixelRect)
            {
                PixelRect rectB = (PixelRect)shapeB;
                return RectRhombOverlap(rectB, rhombA);
            }
            else if (shapeB is PixelRhomb)
            {
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                return RhombRhombOverlap(rhombA, rhombB);
            }
            else if (shapeB is PixelOct)
            {
                PixelOct octB = (PixelOct)shapeB;
                return RhombOctOverlap(rhombA, octB);
            }
        }
        else if (shapeA is PixelDiag)
        {
            PixelDiag diagA = (PixelDiag)shapeA;
            if (shapeB is PixelLine)
            {
                PixelLine lineB = (PixelLine)shapeB;
                return LineDiagOverlap(lineB, diagA);
            }
            else if (shapeB is PixelRect)
            {
                PixelRect rectB = (PixelRect)shapeB;
                return RectDiagOverlap(rectB, diagA);
            }
            else if (shapeB is PixelDiag)
            {
                PixelDiag diagB = (PixelDiag)shapeB;
                return DiagDiagOverlap(diagA, diagB);
            }
            else if (shapeB is PixelOct)
            {
                PixelOct octB = (PixelOct)shapeB;
                return DiagOctOverlap(diagA, octB);
            }
        }
        else if (shapeA is PixelOct)
        {
            PixelOct octA = (PixelOct)shapeA;
            if (shapeB is PixelLine)
            {
                PixelLine lineB = (PixelLine)shapeB;
                return LineOctOverlap(lineB, octA);
            }
            else if (shapeB is PixelRect)
            {
                PixelRect rectB = (PixelRect)shapeB;
                return RectOctOverlap(rectB, octA);
            }
            else if (shapeB is PixelRhomb)
            {
                PixelRhomb rhombB = (PixelRhomb)shapeB;
                return RhombOctOverlap(rhombB, octA);
            }
            else if (shapeB is PixelDiag)
            {
                PixelDiag diagB = (PixelDiag)shapeB;
                return DiagOctOverlap(diagB, octA);
            }
            else if (shapeB is PixelOct)
            {
                PixelOct octB = (PixelOct)shapeB;
                return OctOctOverlap(octA, octB);
            }
        }
        return false;
    }

    public static bool LineLineOverlap(PixelLine lineA, PixelLine lineB)
    {
        return LineIntersection(lineA.start, lineA.end, lineB.start, lineB.end);
    }

    public static bool LineRectOverlap(PixelLine lineA, PixelRect rectB)
    {
        if (PointInRect(lineA.start, rectB) || PointInRect(lineA.end, rectB))
        {
            return true;
        }
        else
        {
            Vector2Int ld = new Vector2Int(rectB.left, rectB.down);
            Vector2Int lu = new Vector2Int(rectB.left, rectB.up);
            Vector2Int rd = new Vector2Int(rectB.right, rectB.down);
            Vector2Int ru = new Vector2Int(rectB.right, rectB.up);
            return LineIntersection(lineA.start, lineA.end, ld, lu)
                || LineIntersection(lineA.start, lineA.end, lu, ru)
                || LineIntersection(lineA.start, lineA.end, ru, rd)
                || LineIntersection(lineA.start, lineA.end, rd, ld);
        }
    }

    public static bool LineRhombOverlap(PixelLine lineA, PixelRhomb rhombB)
    {
        if ((Math.Abs(rhombB.center.x - lineA.start.x) + Math.Abs(rhombB.center.y - lineA.start.y) <= rhombB.diagonal) || (Math.Abs(rhombB.center.x - lineA.end.x) + Math.Abs(rhombB.center.y - lineA.end.y) <= rhombB.diagonal))
        {
            return true;
        }
        else
        {
            Vector2Int ld = new Vector2Int(rhombB.center.x - rhombB.diagonal, rhombB.center.y - rhombB.diagonal);
            Vector2Int lu = new Vector2Int(rhombB.center.x - rhombB.diagonal, rhombB.center.y + rhombB.diagonal);
            Vector2Int rd = new Vector2Int(rhombB.center.x + rhombB.diagonal, rhombB.center.y - rhombB.diagonal);
            Vector2Int ru = new Vector2Int(rhombB.center.x + rhombB.diagonal, rhombB.center.y + rhombB.diagonal);
            return LineIntersection(lineA.start, lineA.end, ld, lu) || LineIntersection(lineA.start, lineA.end, lu, ru) || LineIntersection(lineA.start, lineA.end, ru, rd) || LineIntersection(lineA.start, lineA.end, rd, ld);
        }
    }

    public static bool LineDiagOverlap(PixelLine lineA, PixelDiag diagB)
    {
        if (PointInDiag(lineA.start, diagB) || PointInDiag(lineA.end, diagB))
        {
            return true;
        }
        else
        {
            return LineIntersection(lineA.start, lineA.end, diagB.rightVert, diagB.upVert)
                || LineIntersection(lineA.start, lineA.end, diagB.upVert, diagB.leftVert)
                || LineIntersection(lineA.start, lineA.end, diagB.leftVert, diagB.downVert)
                || LineIntersection(lineA.start, lineA.end, diagB.downVert, diagB.rightVert);
        }
    }

    public static bool LineOctOverlap(PixelLine lineA, PixelOct octB)
    {
        if (PointInOct(lineA.start, octB) || PointInOct(lineA.end, octB))
        {
            return true;
        }
        else
        {
            Vector2Int ld = new Vector2Int(octB.left, octB.down + octB.ldDiag);
            Vector2Int lu = new Vector2Int(octB.left, octB.up - octB.luDiag);
            Vector2Int rd = new Vector2Int(octB.right, octB.down + octB.rdDiag);
            Vector2Int ru = new Vector2Int(octB.right, octB.up - octB.ruDiag);
            Vector2Int dl = new Vector2Int(octB.left + octB.ldDiag, octB.down);
            Vector2Int dr = new Vector2Int(octB.right - octB.rdDiag, octB.down);
            Vector2Int ul = new Vector2Int(octB.left + octB.luDiag, octB.up);
            Vector2Int ur = new Vector2Int(octB.right - octB.ruDiag, octB.up);
            return LineIntersection(lineA.start, lineA.end, ld, lu)
                || LineIntersection(lineA.start, lineA.end, rd, ru)
                || LineIntersection(lineA.start, lineA.end, dl, dr)
                || LineIntersection(lineA.start, lineA.end, ld, lu)
                || LineIntersection(lineA.start, lineA.end, ld, dl)
                || LineIntersection(lineA.start, lineA.end, lu, ul)
                || LineIntersection(lineA.start, lineA.end, rd, dr)
                || LineIntersection(lineA.start, lineA.end, ru, ur);
        }
    }

    public static bool RectRectOverlap(PixelRect rectA, PixelRect rectB)
    {
        return rectA.left <= rectB.right && rectB.left <= rectA.right && rectA.down <= rectB.up && rectB.down <= rectA.up;
    }

    public static bool RectRhombOverlap(PixelRect rectA, PixelRhomb rhombB)
    {
        int closestX = rectA.left <= rhombB.center.x ? (rhombB.center.x <= rectA.right ? rhombB.center.x : rectA.right) : rectA.left;
        int closestY = rectA.down <= rhombB.center.y ? (rhombB.center.y <= rectA.up ? rhombB.center.y : rectA.up) : rectA.down;
        return PixelMath.PixelDist(closestX, closestY, rhombB.center.x, rhombB.center.y) <= rhombB.diagonal;
    }

    public static bool RectDiagOverlap(PixelRect rectA, PixelDiag diagB)
    {
        if (rectA.left <= diagB.rightVert.x && diagB.leftVert.x <= rectA.right && rectA.down <= diagB.upVert.y && diagB.downVert.y <= rectA.up)
        {
            int fromLeft = diagB.leftVert.x - rectA.right;
            int fromRight = rectA.left - diagB.rightVert.x;
            int fromDown = diagB.downVert.y - rectA.up;
            int fromUp = rectA.down - diagB.upVert.y;
            return fromLeft + fromDown + diagB.downDiag <= 0
                && fromLeft + fromUp + diagB.upDiag <= 0
                && fromRight + fromDown + diagB.upDiag <= 0
                && fromRight + fromUp + diagB.downDiag <= 0;
        }
        else
        {
            return false;
        }
    }

    public static bool RectOctOverlap(PixelRect rectA, PixelOct octB)
    {
        if (rectA.left <= octB.right && octB.left <= rectA.right && rectA.down <= octB.up && octB.down <= rectA.up)
        {
            int fromRight = octB.right - rectA.left;
            int fromLeft = rectA.right - octB.left;
            int fromUp = octB.up - rectA.down;
            int fromDown = rectA.up - octB.down;
            return octB.ruDiag <= fromRight + fromUp
                && octB.rdDiag <= fromRight + fromDown
                && octB.luDiag <= fromLeft + fromUp
                && octB.ldDiag <= fromLeft + fromDown;
        }
        else
        {
            return false;
        }
    }

    public static bool RhombRhombOverlap(PixelRhomb rhombA, PixelRhomb rhombB)
    {
        return PixelMath.PixelDist(rhombA.center, rhombB.center) <= rhombA.diagonal + rhombB.diagonal;
    }

    public static bool RhombOctOverlap(PixelRhomb rhombA, PixelOct octB)
    {
        int closestX = octB.left <= rhombA.center.x ? (rhombA.center.x <= octB.right ? rhombA.center.x : octB.right) : octB.left;
        int closestY = octB.down <= rhombA.center.y ? (rhombA.center.y <= octB.up ? rhombA.center.y : octB.up) : octB.down;
        if (PixelMath.PixelDist(closestX, closestY, rhombA.center.x, rhombA.center.y) <= rhombA.diagonal)
        {
            int fromRight = octB.right - rhombA.center.x;
            int fromLeft = rhombA.center.x - octB.left;
            int fromUp = octB.up - rhombA.center.y;
            int fromDown = rhombA.center.y - octB.down;
            return rhombA.diagonal + octB.ruDiag < fromRight + fromUp
                && rhombA.diagonal + octB.rdDiag < fromRight + fromDown
                && rhombA.diagonal + octB.luDiag < fromLeft + fromUp
                && rhombA.diagonal + octB.ldDiag < fromLeft + fromDown;
        }
        else
        {
            return false;
        }
    }

    public static bool DiagDiagOverlap(PixelDiag diagA, PixelDiag diagB)
    {
        int fromLeft = diagB.leftVert.x - diagA.rightVert.x;
        int fromRight = diagA.leftVert.x - diagB.rightVert.x;
        int fromDown = diagB.downVert.y - diagA.upVert.y;
        int fromUp = diagA.downVert.y - diagB.upVert.y;
        return fromLeft + fromDown + diagA.downDiag + diagB.downDiag <= 0
            && fromLeft + fromUp + diagA.upDiag + diagB.upDiag <= 0
            && fromRight + fromDown + diagA.upDiag + diagB.upDiag <= 0
            && fromRight + fromUp + diagA.downDiag + diagB.downDiag <= 0;
    }

    public static bool DiagOctOverlap(PixelDiag diagA, PixelOct octB)
    {
        if (diagA.leftVert.x <= octB.right && octB.left <= diagA.rightVert.x && diagA.downVert.y <= octB.up && octB.down <= diagA.upVert.y)
        {
            int fromLeft = octB.left - diagA.rightVert.x;
            int fromRight = diagA.leftVert.x - octB.right;
            int fromDown = octB.down - diagA.upVert.y;
            int fromUp = diagA.downVert.y - octB.up;
            return fromLeft + fromDown + diagA.downDiag + octB.ldDiag <= 0
                && fromLeft + fromUp + diagA.upDiag + octB.luDiag <= 0
                && fromRight + fromDown + diagA.upDiag + octB.rdDiag <= 0
                && fromRight + fromUp + diagA.downDiag + octB.ruDiag <= 0;
        }
        else
        {
            return false;
        }
    }

    public static bool OctOctOverlap(PixelOct octA, PixelOct octB)
    {
        if (octA.left <= octB.right && octB.left <= octA.right && octA.down <= octB.up && octB.down <= octA.up)
        {
            int fromRight = octB.right - octA.left;
            int fromLeft = octA.right - octB.left;
            int fromUp = octB.up - octA.down;
            int fromDown = octA.up - octB.down;
            return octA.ldDiag + octB.ruDiag <= fromRight + fromUp
                && octA.luDiag + octB.rdDiag <= fromRight + fromDown
                && octA.rdDiag + octB.luDiag <= fromLeft + fromUp
                && octA.ruDiag + octB.ldDiag <= fromLeft + fromDown;
        }
        else
        {
            return false;
        }
    }

    public static bool PointInRect(Vector2Int pointA, PixelRect rectB)
    {
        return rectB.left <= pointA.x
            && pointA.x <= rectB.right
            && rectB.down <= pointA.y
            && pointA.y <= rectB.up;
    }

    public static bool PointInDiag(Vector2Int pointA, PixelDiag diagB)
    {
        int fromLeft = diagB.leftVert.x - pointA.x;
        int fromRight = pointA.x - diagB.rightVert.x;
        int fromDown = diagB.downVert.y - pointA.y;
        int fromUp = pointA.y - diagB.upVert.y;
        return fromLeft + fromDown + diagB.downDiag <= 0
            && fromLeft + fromUp + diagB.upDiag <= 0
            && fromRight + fromDown + diagB.upDiag <= 0
            && fromRight + fromUp + diagB.downDiag <= 0;
    }

    public static bool PointInOct(Vector2Int pointA, PixelOct octB)
    {
        int fromLeft = octB.left - pointA.x;
        int fromRight = pointA.x - octB.right;
        int fromDown = octB.down - pointA.y;
        int fromUp = pointA.y - octB.up;
        return fromLeft + fromDown + octB.ldDiag <= 0
            && fromLeft + fromUp + octB.luDiag <= 0
            && fromRight + fromDown + octB.rdDiag <= 0
            && fromRight + fromUp + octB.ruDiag <= 0;
    }

    // Whether two lines intersect. Uses only ints for horizontal, vertical, 45 degree lines.
    public static bool LineIntersection(Vector2Int a1, Vector2Int a2, Vector2Int b1, Vector2Int b2)
    {
        int aMinX = Math.Min(a1.x, a2.x);
        int aMaxX = Math.Max(a1.x, a2.x);
        int aMinY = Math.Min(a1.y, a2.y);
        int aMaxY = Math.Max(a1.y, a2.y);
        int bMinX = Math.Min(b1.x, b2.x);
        int bMaxX = Math.Max(b1.x, b2.x);
        int bMinY = Math.Min(b1.y, b2.y);
        int bMaxY = Math.Max(b1.y, b2.y);
        int aDistX = a2.x - a1.x;
        int aDistY = a2.y - a1.y;
        int bDistX = b2.x - b1.x;
        int bDistY = b2.y - b1.y;

        if (aMaxX < bMinX || aMaxY < bMinY || bMaxX < aMinX || bMaxY < aMinY)
        // Bounding boxes don't overlap
        {
            return false;
        }
        else if (a1.x == a2.x)
        // Vertical
        {
            if (b1.x == b2.x)
            // Parallel/Colinear
            {
                return true;
            }
            else if (b1.y == b2.y)
            //Perpendicular
            {
                return true;
            }
            else
            // y-intercept
            {
                if (Math.Abs(bDistX) == Math.Abs(bDistY))
                {
                    int bSlope = Math.Sign(bDistX) * Math.Sign(bDistY);
                    int yIntercept = b1.y + (a1.x - b1.x) * bSlope;
                    return yIntercept <= aMaxY && aMinY <= yIntercept;
                }
            }
        }
        // Horizontal
        else if (a1.y == a2.y)
        {
            if (b1.y == b2.y)
            // Parallel/Colinear
            {
                return true;
            }
            else if (b1.x == b2.x)
            // Perpendicular
            {
                return true;
            }
            else
            // x-intercept
            {
                if (Math.Abs(bDistX) == Math.Abs(bDistY))
                {
                    int bSlope = Math.Sign(bDistX) * Math.Sign(bDistY);
                    int xIntercept = b1.x + (a1.y - b1.y) * bSlope;
                    return xIntercept <= aMaxX && aMinX <= xIntercept;
                }
            }
        }
        else if (Math.Abs(aDistX) == Math.Abs(aDistY))
        {
            if (b1.x == b2.x)
            // y-intercept
            {
                int yIntercept = a1.y + (b1.x - a1.x) * Math.Sign(aDistX) * Math.Sign(aDistY);
                return yIntercept <= bMaxY && bMinY <= yIntercept;
            }
            else if (b1.y == b2.y)
            // x-intercept
            {
                int xIntercept = a1.x + (b1.y - a1.y) * Math.Sign(aDistY) * Math.Sign(aDistX);
                return xIntercept <= bMaxY && bMinY <= xIntercept;
            }
            else if (Math.Abs(bDistX) == Math.Abs(bDistY))
            {
                int aSlope = Math.Sign(aDistX) * Math.Sign(aDistY);
                int bSlope = Math.Sign(bDistX) * Math.Sign(bDistY);
                if (aSlope == bSlope)
                // Parallel/Colinear
                {
                    return b1.x - a1.x == b1.y - a1.y;
                }
                else
                // Perpendicular
                {
                    int hitX = PixelMath.IntDiv(b1.y - a1.y + a1.x * aSlope - b1.x * bSlope, (aSlope - bSlope));
                    return aMinX < hitX && hitX < aMaxX && bMinX < hitX && hitX < bMaxX;
                }
            }
        }
        // Fallback function
        return LineIntersectionFloat(a1, a2, b1, b2);
    }

    // For when LineIntersection needs float operations.
    private static bool LineIntersectionFloat(Vector2Int a1, Vector2Int a2, Vector2Int b1, Vector2Int b2)
    {
        float denom = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);
        float numA = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
        float numB = (a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x);

        // Check for equivalent lines
        if (denom == 0 && numA == 0 && numB == 0)
        {
            return true;
        }
        else
        {
            float r = numA / denom;
            float s = numB / denom;
            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }
    }
}
