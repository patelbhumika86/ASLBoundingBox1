using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualizationStyles
{
    All_WithBoxes,
    All_WithoutBoxes,
    RoomPieces_WithBoxes,
    RoomPieces_WithoutBoxes,
    QuerySpace
}

public class AllObjectIntersector : MonoBehaviour {

    public VisualizationStyles visualizationStyle;
    public Vector3[] MinAndMax = new Vector3[2];
    public GameObject Room;
    [HideInInspector]
    public BoundingBoxVisualizer visualizer;

    private Vector3 cachedMin = new Vector3();
    private Vector3 cachedMax = new Vector3();
    private BoundCorners minMaxCorners = new BoundCorners();

    #region Methods
    public void Start()
    {
        visualizer = new BoundingBoxVisualizer();
        cacheMinMax();
        minMaxCorners = visualizer.CalculateCorners(cachedMin, cachedMax);
    }

    public void Update()
    {
        updateMinMaxCorners();
        Visualize(visualizationStyle);
    }

    public void Visualize(VisualizationStyles visualizationStyle)
    {
        bool displayRoomBoundingBoxes = true;

        switch (visualizationStyle)
        {
            case VisualizationStyles.QuerySpace:
                visualizer.LineColor = visualizer.NotContainedColor;
                VisualizeQuerySpace();
                break;
            case VisualizationStyles.RoomPieces_WithBoxes:
                visualizer.LineColor = visualizer.NotContainedColor;
                displayRoomBoundingBoxes = true;
                VisualizeRoomPieces(displayRoomBoundingBoxes);
                break;
            case VisualizationStyles.RoomPieces_WithoutBoxes:
                visualizer.LineColor = visualizer.NotContainedColor;
                displayRoomBoundingBoxes = false;
                VisualizeRoomPieces(displayRoomBoundingBoxes);
                break;
            case VisualizationStyles.All_WithBoxes:
                visualizer.LineColor = visualizer.ContainedColor;
                VisualizeQuerySpace();
                displayRoomBoundingBoxes = true;
                VisualizeRoomPieces(displayRoomBoundingBoxes);
                break;
            case VisualizationStyles.All_WithoutBoxes:
                visualizer.LineColor = visualizer.ContainedColor;
                VisualizeQuerySpace();
                displayRoomBoundingBoxes = false;
                VisualizeRoomPieces(displayRoomBoundingBoxes);
                break;
            default:
                Debug.LogError("AllObjectIntersector: Unknown visualization style requested for visualization. Unable to visualize.");
                break;
        }
    }

    #region Caching Logic
    private void updateMinMaxCorners()
    {
        if (minMaxCornersNeedsUpdate())
        {
            cacheMinMax();
            minMaxCorners = visualizer.CalculateCorners(cachedMin, cachedMax);
        }
    }

    private bool minMaxCornersNeedsUpdate()
    {
        Vector3 min = MinAndMax[0];
        Vector3 max = MinAndMax[1];

        if(min != cachedMin
            || max != cachedMax)
        {
            return true;
        }

        return false;
    }

    private void cacheMinMax()
    {
        cachedMin = MinAndMax[0];
        cachedMax = MinAndMax[1];
    }
    #endregion

    #region Visualization
    public void VisualizeQuerySpace()
    {
        visualizer.VisualizeBox(minMaxCorners);
    }

    public void VisualizeRoomPieces(bool displayBoundingBox)
    {
        var goList = GetGameObjectsInHierarchy(Room);
        foreach(var go in goList)
        {
            VisualizeRoomPieceIfAppropriate(go, displayBoundingBox);
        }
    }

    public void VisualizeRoomPieceIfAppropriate(GameObject roomPiece, bool displayBoundingBox)
    {
        if (roomPiece != null)
        {
            Bounds b = visualizer.GetBoundsFor(roomPiece);
            BoundCorners corners = visualizer.CalculateCorners(b, roomPiece);
            bool shouldBeVisible = getVisibilityRecommendation(corners, minMaxCorners);

            if (shouldBeVisible)
            {
                roomPiece.SetActive(true);
                if (displayBoundingBox)
                {
                    visualizer.VisualizeBox(corners);
                }
            }
            else
            {
                roomPiece.SetActive(false);
            }
        }
    }

    private bool getVisibilityRecommendation(GameObject go, BoundCorners querySpaceCorners)
    {
        Bounds b = visualizer.GetBoundsFor(go);
        BoundCorners goCorners = visualizer.CalculateCorners(b, go);
        return getVisibilityRecommendation(goCorners, querySpaceCorners);
    }

    private bool getVisibilityRecommendation(BoundCorners goCorners, BoundCorners querySpaceCorners)
    {
        //bool MeshBBCornerInQuerySpaceBB = true;
        //bool fbl = visualizer.CornersContains(goCorners.FrontBottomLeft, querySpaceCorners);
        //if (!fbl)
        //{
        //    bool ftl = visualizer.CornersContains(goCorners.FrontTopLeft, querySpaceCorners);
        //    if (!ftl)
        //    {
        //        bool ftr = visualizer.CornersContains(goCorners.FrontTopRight, querySpaceCorners);
        //        if (!ftr)
        //        {
        //            bool fbr = visualizer.CornersContains(goCorners.FrontBottomRight, querySpaceCorners);
        //            if (!fbr)
        //            {
        //                bool bbl = visualizer.CornersContains(goCorners.BackBottomLeft, querySpaceCorners);
        //                if (!bbl)
        //                {
        //                    bool btl = visualizer.CornersContains(goCorners.BackTopLeft, querySpaceCorners);
        //                    if (!btl)
        //                    {
        //                        bool btr = visualizer.CornersContains(goCorners.BackTopRight, querySpaceCorners);
        //                        if (!btr)
        //                        {
        //                            bool bbr = visualizer.CornersContains(goCorners.BackBottomRight, querySpaceCorners);
        //                            if (!bbr)
        //                            {
        //                                MeshBBCornerInQuerySpaceBB = false;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //if (MeshBBCornerInQuerySpaceBB)
        //{
        //    return true;
        //}

        return visualizer.Intersects(goCorners, querySpaceCorners);
    }
    #endregion

    #region Grab Objects
    public List<GameObject> GetGameObjectsInHierarchy(GameObject parent)
    {
        List<GameObject> goList = new List<GameObject>();

        getChildGameObjects(parent, ref goList);

        return goList;
    }

    private void getChildGameObjects(GameObject parent, ref List<GameObject> goList)
    {
        if (parent != null)
        {
            var mf = parent.GetComponent<MeshFilter>();
            if(mf != null)
            {
                goList.Add(parent);
            }

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                getChildGameObjects(child, ref goList);
            }
        }
    }
    #endregion
    #endregion
}
