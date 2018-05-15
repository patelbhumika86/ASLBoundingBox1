using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MeshSaverInterface))]
public class MeshSaverInterfaceEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshSaverInterface interfaceScript = (MeshSaverInterface)target;
        if(GUILayout.Button("Save Mesh"))
        {
            interfaceScript.SaveMesh();
        }
        if(GUILayout.Button("Load Mesh"))
        {
            interfaceScript.LoadMesh();
        }
    }
}
#endif