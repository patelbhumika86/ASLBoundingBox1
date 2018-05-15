using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BoundingBoxVisualizer))]
public class BoundingBoxVisualizerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoundingBoxVisualizer visualizer = (BoundingBoxVisualizer)target;
        if (GUILayout.Button("Toggle Bounds For Game Object"))
        {
            //visualizer.VisualizeGameObjectBounds();
            visualizer.ToggleGameObjectBounds();
        }
        if (GUILayout.Button("Toggle Bounds From MinAndMax"))
        {
            //visualizer.VisualizeBoxFromMinAndMax();
            visualizer.ToggleMinAndMaxBounds();
        }
        if (GUILayout.Button("Toggle Bounds From Box Corners"))
        {
            //visualizer.VisualizeBoxFromBoxCorners();
            visualizer.ToggleBoxCornersBounds();
        }
    }
}
#endif