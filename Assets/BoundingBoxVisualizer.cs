using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxVisualizer : MonoBehaviour {
    public float EdgeIntersectMargin = 0.0003f;
    public Color NotContainedColor = Color.red;
    public Color ContainedColor = Color.green;
    public bool MinMaxIntersect = false;
    public GameObject MeshObjectToDisplayBoundsFor;
    public Vector3[] MinAndMax = new Vector3[2];
    public Vector3[] BoxCorners = new Vector3[8];

    private Color LineColor = Color.red;
    public bool GOTrigger = false;
    public bool MinMaxTrigger = false;
    public bool CornersTrigger = false;

    #region Editor Inspector Methods
    public void ToggleGameObjectBounds()
    {
        GOTrigger = !GOTrigger;
    }

    public void ToggleMinAndMaxBounds()
    {
        MinMaxTrigger = !MinMaxTrigger;
    }

    public void ToggleBoxCornersBounds()
    {
        CornersTrigger = !CornersTrigger;
    }

    public void VisualizeGameObjectBounds()
    {
        Bounds b = GetBoundsFor(MeshObjectToDisplayBoundsFor);
        BoundCorners corners = CalculateCorners(b, MeshObjectToDisplayBoundsFor);
        VisualizeBox(corners);
        //VisualizeBoxWithRays(corners);
    }

    public void VisualizeBoxFromMinAndMax()
    {
        BoundCorners corners = CalculateCorners(MinAndMax[0], MinAndMax[1]);
        VisualizeBox(corners);
    }

    public void VisualizeBoxFromBoxCorners()
    {
        BoundCorners corners = GetBoundCorners(BoxCorners[0], BoxCorners[1], BoxCorners[2], BoxCorners[3], BoxCorners[4], BoxCorners[5], BoxCorners[6], BoxCorners[7]);
        VisualizeBox(corners);
    }
    #endregion

    #region Logic
    public BoundCorners CalculateCorners(Bounds bounds, GameObject go)
    {
        Vector3 cornerMin = bounds.min; // Front, Bottom, Left
        Vector3 cornerFTL = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z); // Front, Top, Left
        Vector3 cornerFTR = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z); // Front, Top, Right
        Vector3 cornerFBR = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z); // Front, Bottom, Right
        Vector3 cornerBBL = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z); // Back, Bottom, Left
        Vector3 cornerBTL = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z); // Back, Top, Left
        Vector3 cornerMax = bounds.max; // Back, Top, Right
        Vector3 cornerBBR = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z); // Back, Bottom, Right

        if(go != null)
        {
            cornerMin = RotateBoundingBoxPoint(cornerMin, go);
            cornerFTL = RotateBoundingBoxPoint(cornerFTL, go);
            cornerFTR = RotateBoundingBoxPoint(cornerFTR, go);
            cornerFBR = RotateBoundingBoxPoint(cornerFBR, go);
            cornerBBL = RotateBoundingBoxPoint(cornerBBL, go);
            cornerBTL = RotateBoundingBoxPoint(cornerBTL, go);
            cornerMax = RotateBoundingBoxPoint(cornerMax, go);
            cornerBBR = RotateBoundingBoxPoint(cornerBBR, go);
        }

        BoundCorners corners = new BoundCorners(cornerMin, cornerFTL, cornerFTR, cornerFBR, cornerBBL, cornerBTL, cornerMax, cornerBBR);

        return corners;
    }

    public void Update()
    {
        if (GOTrigger)
        {
            VisualizeGameObjectBounds();
        }
        if (MinMaxTrigger)
        {
            VisualizeBoxFromMinAndMax();
        }
        if (CornersTrigger)
        {
            VisualizeBoxFromBoxCorners();
        }

        MinMaxIntersect = DoesMinMaxIntersectObject();
        if (MinMaxIntersect)
        {
            LineColor = ContainedColor;
        }
        else
        {
            LineColor = NotContainedColor;
        }
    }

    //public void VisualizeBoxWithRays(BoundCorners corners)
    //{
    //    Debug.Log("Corner0 = " + corners.FrontBottomLeft);
    //    Debug.Log("Corner1 = " + corners.FrontTopLeft);
    //    Debug.Log("Corner2 = " + corners.FrontTopRight);
    //    Debug.Log("Corner3 = " + corners.FrontBottomRight);
    //    Debug.Log("Corner4 = " + corners.BackBottomLeft);
    //    Debug.Log("Corner5 = " + corners.BackTopLeft);
    //    Debug.Log("Corner6 = " + corners.BackTopRight);
    //    Debug.Log("Corner7 = " + corners.BackBottomRight);

    //    // Connect front face
    //    Debug.DrawRay(corners.FrontBottomLeft, (corners.FrontTopLeft - corners.FrontBottomRight), LineColor, (corners.FrontTopLeft - corners.FrontBottomLeft).magnitude);
    //    Debug.DrawLine(corners.FrontBottomLeft, corners.FrontTopLeft, LineColor);
    //    Debug.DrawLine(corners.FrontTopLeft, corners.FrontTopRight, LineColor);
    //    Debug.DrawLine(corners.FrontTopRight, corners.FrontBottomRight, LineColor);
    //    Debug.DrawLine(corners.FrontBottomRight, corners.FrontBottomLeft, LineColor);

    //    // Connect back face
    //    Debug.DrawLine(corners.BackBottomLeft, corners.BackTopLeft, LineColor);
    //    Debug.DrawLine(corners.BackTopLeft, corners.BackTopRight, LineColor);
    //    Debug.DrawLine(corners.BackTopRight, corners.BackBottomRight, LineColor);
    //    Debug.DrawLine(corners.BackBottomRight, corners.BackBottomLeft, LineColor);

    //    // Connect faces
    //    Debug.DrawLine(corners.FrontBottomLeft, corners.BackBottomLeft, LineColor);
    //    Debug.DrawLine(corners.FrontTopLeft, corners.BackTopLeft, LineColor);
    //    Debug.DrawLine(corners.FrontTopRight, corners.BackTopRight, LineColor);
    //    Debug.DrawLine(corners.FrontBottomRight, corners.BackBottomRight, LineColor);
    //}

    public void VisualizeBox(BoundCorners corners)
    {
        // Connect front face
        Debug.DrawLine(corners.FrontBottomLeft, corners.FrontTopLeft, LineColor);
        Debug.DrawLine(corners.FrontTopLeft, corners.FrontTopRight, LineColor);
        Debug.DrawLine(corners.FrontTopRight, corners.FrontBottomRight, LineColor);
        Debug.DrawLine(corners.FrontBottomRight, corners.FrontBottomLeft, LineColor);

        // Connect back face
        Debug.DrawLine(corners.BackBottomLeft, corners.BackTopLeft, LineColor);
        Debug.DrawLine(corners.BackTopLeft, corners.BackTopRight, LineColor);
        Debug.DrawLine(corners.BackTopRight, corners.BackBottomRight, LineColor);
        Debug.DrawLine(corners.BackBottomRight, corners.BackBottomLeft, LineColor);

        // Connect faces
        Debug.DrawLine(corners.FrontBottomLeft, corners.BackBottomLeft, LineColor);
        Debug.DrawLine(corners.FrontTopLeft, corners.BackTopLeft, LineColor);
        Debug.DrawLine(corners.FrontTopRight, corners.BackTopRight, LineColor);
        Debug.DrawLine(corners.FrontBottomRight, corners.BackBottomRight, LineColor);
    }
    
    public BoundCorners GetBoundCorners(Vector3 frontBottomLeft, Vector3 frontTopLeft, Vector3 frontTopRight, Vector3 frontBottomRight, Vector3 backBottomLeft, Vector3 backTopLeft, Vector3 backTopRight, Vector3 backBottomRight)
    {
        BoundCorners corners = new BoundCorners(
            frontBottomLeft,
            frontTopLeft,
            frontTopRight,
            frontBottomRight,
            backBottomLeft,
            backTopLeft,
            backTopRight,
            backBottomRight);

        return corners;
    }

    public BoundCorners CalculateCorners(Vector3 min, Vector3 max)
    {
        Vector3 center = min + ((max - min) / 2);
        Vector3 size = max - min;
        Bounds b = new Bounds(center, size);

        BoundCorners corners = CalculateCorners(b, null);
        return corners;
    }

    public Bounds GetBoundsFor(GameObject go)
    {
        var mf = go.GetComponent<MeshFilter>();

        if(mf != null)
        {
            mf.mesh.RecalculateBounds();
            Bounds b = mf.mesh.bounds;
            b.center += go.transform.position;
            return b;
        }
        else
        {
            Debug.LogError("Could not find MeshFilter component for GameObject. Unable to calculate bounds to draw them.");
            return new Bounds();
        }
    }

    public bool DoesMinMaxIntersectObject()
    {
        BoundCorners goCorners = CalculateCorners(GetBoundsFor(MeshObjectToDisplayBoundsFor), MeshObjectToDisplayBoundsFor);
        BoundCorners minMaxCorners = CalculateCorners(MinAndMax[0], MinAndMax[1]);

        //// Calculate if a corner of the mesh bounding box is in the minmax bounding box
        //bool MeshBBCornerInMinMaxBB = true;
        //bool fbl = IsLesserThanPoint(goCorners.FrontBottomLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontBottomLeft, MinAndMax[1]);
        //      if (!fbl)
        //      {
        //          bool ftl = IsLesserThanPoint(goCorners.FrontTopLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontTopLeft, MinAndMax[1]);
        //          if (!ftl)
        //          {
        //              bool ftr = IsLesserThanPoint(goCorners.FrontTopRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontTopRight, MinAndMax[1]);
        //              if (!ftr)
        //              {
        //                  bool fbr = IsLesserThanPoint(goCorners.FrontBottomRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.FrontBottomRight, MinAndMax[1]);
        //                  if (!fbr)
        //                  {
        //                      bool bbl = IsLesserThanPoint(goCorners.BackBottomLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackBottomLeft, MinAndMax[1]);
        //                      if (!bbl)
        //                      {
        //                          bool btl = IsLesserThanPoint(goCorners.BackTopLeft, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackTopLeft, MinAndMax[1]);
        //                          if (!btl)
        //                          {
        //                              bool btr = IsLesserThanPoint(goCorners.BackTopRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackTopRight, MinAndMax[1]);
        //                              if (!btr)
        //                              {
        //                                  bool bbr = IsLesserThanPoint(goCorners.BackBottomRight, MinAndMax[0]) && IsGreaterThanPoint(goCorners.BackBottomRight, MinAndMax[1]);
        //                                  if (!bbr)
        //                                  {
        //								MeshBBCornerInMinMaxBB = false;
        //                                  }
        //                              }
        //                          }
        //                      }
        //                  }
        //              }
        //          }
        //      }

        // Calculate if a corner of the mesh bounding box is in the minmax bounding box
        bool MeshBBCornerInMinMaxBB = true;
        bool fbl = CornersContains(goCorners.FrontBottomLeft, minMaxCorners);
        if (!fbl)
        {
            bool ftl = CornersContains(goCorners.FrontTopLeft, minMaxCorners);
            if (!ftl)
            {
                bool ftr = CornersContains(goCorners.FrontTopRight, minMaxCorners);
                if (!ftr)
                {
                    bool fbr = CornersContains(goCorners.FrontBottomRight, minMaxCorners);
                    if (!fbr)
                    {
                        bool bbl = CornersContains(goCorners.BackBottomLeft, minMaxCorners);
                        if (!bbl)
                        {
                            bool btl = CornersContains(goCorners.BackTopLeft, minMaxCorners);
                            if (!btl)
                            {
                                bool btr = CornersContains(goCorners.BackTopRight, minMaxCorners);
                                if (!btr)
                                {
                                    bool bbr = CornersContains(goCorners.BackBottomRight, minMaxCorners);
                                    if (!bbr)
                                    {
                                        MeshBBCornerInMinMaxBB = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (MeshBBCornerInMinMaxBB) {
			return true;
		}

        // Calculate if a corner of the minmax bounding box is in the mesh bounding box
        //bool MinMaxBBCornerInMeshBB = true;
        //fbl = IsLesserThanPoint(minMaxCorners.FrontBottomLeft, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.FrontBottomLeft, goCorners.BackTopRight);
        //if (!fbl)
        //{
        //	bool ftl = IsLesserThanPoint(minMaxCorners.FrontTopLeft, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.FrontTopLeft, goCorners.BackTopRight);
        //	if (!ftl)
        //	{
        //		bool ftr = IsLesserThanPoint(minMaxCorners.FrontTopRight, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.FrontTopRight, goCorners.BackTopRight);
        //		if (!ftr)
        //		{
        //			bool fbr = IsLesserThanPoint(minMaxCorners.FrontBottomRight, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.FrontBottomRight, goCorners.BackTopRight);
        //			if (!fbr)
        //			{
        //				bool bbl = IsLesserThanPoint(minMaxCorners.BackBottomLeft, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.BackBottomLeft, goCorners.BackTopRight);
        //				if (!bbl)
        //				{
        //					bool btl = IsLesserThanPoint(minMaxCorners.BackTopLeft, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.BackTopLeft, goCorners.BackTopRight);
        //					if (!btl)
        //					{
        //						bool btr = IsLesserThanPoint(minMaxCorners.BackTopRight, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.BackTopRight, goCorners.BackTopRight);
        //						if (!btr)
        //						{
        //							bool bbr = IsLesserThanPoint(minMaxCorners.BackBottomRight, goCorners.FrontBottomLeft) && IsGreaterThanPoint(minMaxCorners.BackBottomRight, goCorners.BackTopRight);
        //							if (!bbr)
        //							{
        //								MinMaxBBCornerInMeshBB = false;
        //							}
        //						}
        //					}
        //				}
        //			}
        //		}
        //	}
        //}

        bool MinMaxBBCornerInMeshBB = true;
        fbl = CornersContains(minMaxCorners.FrontBottomLeft, goCorners);
        if (!fbl)
        {
            bool ftl = CornersContains(minMaxCorners.FrontTopLeft, goCorners);
            if (!ftl)
            {
                bool ftr = CornersContains(minMaxCorners.FrontTopRight, goCorners);
                if (!ftr)
                {
                    bool fbr = CornersContains(minMaxCorners.FrontBottomRight, goCorners);
                    if (!fbr)
                    {
                        bool bbl = CornersContains(minMaxCorners.BackBottomLeft, goCorners);
                        if (!bbl)
                        {
                            bool btl = CornersContains(minMaxCorners.BackTopLeft, goCorners);
                            if (!btl)
                            {
                                bool btr = CornersContains(minMaxCorners.BackTopRight, goCorners);
                                if (!btr)
                                {
                                    bool bbr = CornersContains(minMaxCorners.BackBottomRight, goCorners);
                                    if (!bbr)
                                    {
                                        MinMaxBBCornerInMeshBB = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (MinMaxBBCornerInMeshBB) {
			return true;
		}

		// Calculate if an edge of the mesh bounding box exists in the minmax bounding box
		// or vice versa
		bool edgeBoxIntersect = AnyEdgeBoxIntersect (goCorners, minMaxCorners);
		if (edgeBoxIntersect) {
			return true;
		}

        return false;
    }

	private bool AnyEdgeBoxIntersect(BoundCorners corners, BoundCorners otherCorners)
	{
		List<KeyValuePair<Vector3, Vector3>> edges = GetEdges (corners);
		List<KeyValuePair<Vector3, Vector3>> otherEdges = GetEdges (otherCorners);

		foreach (var edge in edges) {
			if (EdgeBoxIntersect (edge, otherCorners)) {
				return true;
			}
		}

		foreach (var edge in otherEdges) {
			if (EdgeBoxIntersect (edge, corners)) {
				return true;
			}
		}

		return false;
	}

	private bool EdgeBoxIntersect(KeyValuePair<Vector3, Vector3> edge, BoundCorners boxCorners)
	{
		Vector3 fbl = boxCorners.FrontBottomLeft;
		Vector3 ftl = boxCorners.FrontTopLeft;
		Vector3 ftr = boxCorners.FrontTopRight;
		Vector3 fbr = boxCorners.FrontBottomRight;
		Vector3 bbl = boxCorners.BackBottomLeft;
		Vector3 btl = boxCorners.BackTopLeft;
		Vector3 btr = boxCorners.BackTopRight;
		Vector3 bbr = boxCorners.BackBottomRight;

		// Generate a face for each of the box faces
		List<Vector3> frontFaceCorners = new List<Vector3>(new Vector3[] {fbl, ftl, ftr, fbr});
		List<Vector3> backFaceCorners = new List<Vector3> (new Vector3[] { bbr, btr, btl, bbl });
		List<Vector3> leftFaceCorners = new List<Vector3> (new Vector3[] { bbl, btl, ftl, fbl });
		List<Vector3> rightFaceCorners = new List<Vector3> (new Vector3[] { fbr, ftr, btr, bbr });
		List<Vector3> topFaceCorners = new List<Vector3> (new Vector3[] { ftl, btl, btr, ftr });
		List<Vector3> bottomFaceCorners = new List<Vector3> (new Vector3[] { fbl, bbl, bbr, fbr });

		if (!EdgeFaceIntersect (edge, frontFaceCorners.ToArray ())) {
			if (!EdgeFaceIntersect (edge, backFaceCorners.ToArray ())) {
				if (!EdgeFaceIntersect (edge, leftFaceCorners.ToArray ())) {
					if (!EdgeFaceIntersect (edge, rightFaceCorners.ToArray ())) {
						if (!EdgeFaceIntersect (edge, topFaceCorners.ToArray ())) {
							if (!EdgeFaceIntersect (edge, bottomFaceCorners.ToArray ())) {

								return false;
							}
						}
					}
				}
			}

		}

		return true;
	}

	/// <summary>
	/// Assumes that the faceCorners are the corners associated with a face, such that when you look at the face, the four corners passed in are passed in in clockwise order.
	/// </summary>
	/// <returns><c>true</c>, if face intersect was edged, <c>false</c> otherwise.</returns>
	/// <param name="edge">Edge.</param>
	/// <param name="faceCorners">Face corners.</param>
	private bool EdgeFaceIntersect(KeyValuePair<Vector3, Vector3> edge, Vector3[] faceCorners)
	{
		if (faceCorners.Length != 4) {
			return false;
		}

		Plane p = new Plane (faceCorners [0], faceCorners [1], faceCorners [2]);
		Vector3 rayOrigin = edge.Key;
		Vector3 rayDirection = (edge.Value - edge.Key).normalized;
		Ray r = new Ray (rayOrigin, rayDirection);

		float edgeLength = (edge.Value - edge.Key).magnitude;

		float enter;
		bool intersected = p.Raycast (r, out enter);
		if (intersected) {
			float edgeIntersectionRayLength = ((rayOrigin + rayDirection * enter) - rayOrigin).magnitude;
			if (edgeIntersectionRayLength > edgeLength) {
				return false;
			}

            Vector3 hitPoint = rayOrigin + rayDirection * enter;

			Vector3 faceCenter = faceCorners [0] + (faceCorners [2] - faceCorners [0]) / 2.0f;
			Vector3 centPlaneEdgeIntersectRayDirection = (hitPoint - faceCenter).normalized;
			Ray centPlaneEdgeIntersectRay = new Ray (faceCenter, centPlaneEdgeIntersectRayDirection);
			float centPlaneEdgeIntersectDistance = (hitPoint - faceCenter).magnitude;

			bool liesWithinAllFaceEdges = true;

			// Check to see if ray from faceCenter to plane-edge intersection point lies within the distance generated by an intersection for each of the face edges and the ray generated from the center to the plane-edge intersection point
			for (int i = 0; i < faceCorners.Length; i++)
			{
				Vector3 faceEdgeStart = faceCorners [i];
				Vector3 faceEdgeEnd = faceCorners [(i + 1) % faceCorners.Length];
				Vector3 faceEdgeDir = (faceEdgeEnd - faceEdgeStart).normalized;

				Vector3 linePoint1 = hitPoint;
				Vector3 lineVec1 = centPlaneEdgeIntersectRayDirection;
				Vector3 linePoint2 = faceEdgeStart;
				Vector3 lineVec2 = faceEdgeDir;

				Vector3 lineLineIntersectPoint;
				if (LineLineIntersection (out lineLineIntersectPoint, linePoint1, lineVec1, linePoint2, lineVec2)) {
					Vector3 lineLineIntersectPointDir = (lineLineIntersectPoint - faceCenter).normalized;
					float distanceToCenterForLineLineIntersection = (lineLineIntersectPoint - faceCenter).magnitude;
					//if (centPlaneEdgeIntersectDistance <= (distanceToCenterForLineLineIntersection + wiggleRoom)
					//    && Vector3.Dot (centPlaneEdgeIntersectRayDirection, lineLineIntersectPointDir) < -0.9) { // > 0
                    if(centPlaneEdgeIntersectDistance <= (distanceToCenterForLineLineIntersection + EdgeIntersectMargin)) { 
						continue;
					} else {
						liesWithinAllFaceEdges = false;
						break;
					}
				} else {
					liesWithinAllFaceEdges = false;
					break;
				}
			}

			if (liesWithinAllFaceEdges) {
				return true;
			}
		}

		return false;
	}
	
	private static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross (lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross (lineVec3, lineVec2);

		float planarFactor = Vector3.Dot (lineVec3, crossVec1and2);
		if (Mathf.Abs (planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
			float s = Vector3.Dot (crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = linePoint1 + (lineVec1 * s);
			return true;

		} else
		{
			intersection = Vector3.zero;
			return false;
		}
	}
	private List<KeyValuePair<Vector3, Vector3>> GetEdges(BoundCorners corners)
	{
		List<KeyValuePair<Vector3, Vector3>> edgeList = new List<KeyValuePair<Vector3, Vector3>> ();

		// Front face
		edgeList.Add (GetEdge (corners.FrontBottomLeft, corners.FrontTopLeft));
		edgeList.Add (GetEdge (corners.FrontTopLeft, corners.FrontTopRight));
		edgeList.Add (GetEdge (corners.FrontTopRight, corners.FrontBottomRight));
		edgeList.Add (GetEdge (corners.FrontBottomRight, corners.FrontBottomLeft));

		// Back face
		edgeList.Add (GetEdge (corners.BackBottomLeft, corners.BackTopLeft));
		edgeList.Add (GetEdge (corners.BackTopLeft, corners.BackTopRight));
		edgeList.Add (GetEdge (corners.BackTopRight, corners.BackBottomRight));
		edgeList.Add (GetEdge (corners.BackBottomRight, corners.BackBottomLeft));

		// Connectors
		edgeList.Add(GetEdge(corners.FrontBottomLeft, corners.BackBottomLeft));
		edgeList.Add (GetEdge (corners.FrontTopLeft, corners.BackTopLeft));
		edgeList.Add (GetEdge (corners.FrontTopRight, corners.BackTopRight));
		edgeList.Add (GetEdge (corners.FrontBottomRight, corners.BackBottomRight));

		return edgeList;
	}

	private KeyValuePair<Vector3, Vector3> GetEdge(Vector3 start, Vector3 end)
	{
		return new KeyValuePair<Vector3, Vector3> (start, end);
	}

    public bool IsGreaterThanPoint(Vector3 a, Vector3 b)
    {
        // is b greater than a?
        float xcon = b.x - a.x;
        float ycon = b.y - a.y;
        float zcon = b.z - a.z;

        //return (xcon >= 0) && (ycon >= 0) && (zcon >= 0);

        return (b.x >= a.x) && (b.y >= a.y) && (b.z >= a.z);
    }

    public bool IsLesserThanPoint(Vector3 a, Vector3 b)
    {
        // is b less than a?
        float xcon = b.x - a.x;
        float ycon = b.y - a.y;
        float zcon = b.z - a.z;

        //return (xcon <= 0) && (ycon <= 0) && (zcon <= 0);
        return (b.x <= a.x) && (b.y <= a.y) && (b.z <= a.z);
    }

    public bool CornersContains(Vector3 point, BoundCorners corners)
    {
        // Create a vector from max to min
        Vector3 min = corners.FrontBottomLeft;
        Vector3 max = corners.BackTopRight;
        
        // Determine whether it has a positive dot product with all three nearest sides
        // Repeat for the max side

        Vector3 v = point - min;
        Vector3 up = corners.FrontTopLeft - min;
        Vector3 back = corners.BackBottomLeft - min;
        Vector3 right = corners.FrontBottomRight - min;

        if(Vector3.Dot(v, up) >= 0
            && Vector3.Dot(v, back) >= 0
            && Vector3.Dot(v, right) >= 0)
        {
            v = point - max;

            up = corners.BackBottomRight - max;
            back = corners.FrontTopRight - max;
            right = corners.BackTopLeft - max;

            if(Vector3.Dot(v, up) >= 0
                && Vector3.Dot(v, back) >= 0
                && Vector3.Dot(v, right) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    public Vector3 RotateBoundingBoxPoint(Vector3 point, GameObject go)
    {
        Vector3 pivot = go.transform.position;
        Quaternion quat = go.transform.rotation;
        Vector3 dir = point - pivot;

        Vector3 rotatedDir = quat * dir;
        return pivot + rotatedDir;
    }
    #endregion
}