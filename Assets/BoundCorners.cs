using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoundCorners
{
    public Vector3 FrontBottomLeft;
    public Vector3 FrontTopLeft;
    public Vector3 FrontTopRight;
    public Vector3 FrontBottomRight;
    public Vector3 BackBottomLeft;
    public Vector3 BackTopLeft;
    public Vector3 BackTopRight;
    public Vector3 BackBottomRight;

    public BoundCorners(Vector3 FBL, Vector3 FTL, Vector3 FTR, Vector3 FBR, Vector3 BBL, Vector3 BTL, Vector3 BTR, Vector3 BBR)
    {
        this.FrontBottomLeft = FBL;
        this.FrontTopLeft = FTL;
        this.FrontTopRight = FTR;
        this.FrontBottomRight = FBR;
        this.BackBottomLeft = BBL;
        this.BackTopLeft = BTL;
        this.BackTopRight = BTR;
        this.BackBottomRight = BBR;
    }
};
