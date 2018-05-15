using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MeshSaverInterface : MonoBehaviour {
    public string FileName = "MyMeshes";
	public string Directory = "/Users/bhumi/Documents/Capstone/testFiles";
    public string FilePath = "";
    public GameObject MeshParent;
    public Color loadedMeshColor;

    private string fileExtension = ".txt";
    
    public void Update()
    {
        Directory = FixDirectory();
        FilePath = JoinDirectoryAndFilename();
    }

    private string FixDirectory()
    {
        //Debug.Log("Fixing directory string to use appropriate directory separators...");

        string directory = Directory;
        string[] directoryComponents = directory.Split(new char[1] { '/' });
        List<string> directoryComponentsCurated = new List<string>();

        // Add the double slashes after C: for windows operating system
        //directoryComponents[0] = directoryComponents[0] + '/';

		bool initSlashPassed = false;
        foreach(string comp in directoryComponents)
        {
			if (!initSlashPassed
			   && string.IsNullOrEmpty (comp)) {
				directoryComponentsCurated.Add (comp);
				initSlashPassed = true;
			}
			if (!string.IsNullOrEmpty(comp))
            {
                directoryComponentsCurated.Add(comp);
            }
        }

        directory = string.Join("/", directoryComponentsCurated.ToArray());

        //Debug.Log("Directory fixed to " + directory);

        return directory;
    }

    private string AppendFileExtensionToFileName()
    {
        return FileName + fileExtension;
    }

    private string JoinDirectoryAndFilename()
    {
        return string.Join("/", new string[2] { Directory, AppendFileExtensionToFileName()});
    }

    public void SaveMesh()
    {
        if (MeshParent != null
            && !string.IsNullOrEmpty(FilePath))
        {
            MeshSaver.SaveObjectMesh(MeshParent, FilePath);
        }
    }

    public void LoadMesh()
    {
        if (!string.IsNullOrEmpty(FilePath)
            && File.Exists(FilePath))
        {
            Mesh[] childMeshes = MeshSaver.LoadMesh(FilePath);

            GameObject go = new GameObject();
            go.name = childMeshes[0].name;

            for(int i = 0; i < childMeshes.Length; i++)
            {
                GameObject child = new GameObject();
                var mf = child.AddComponent<MeshFilter>();
                mf.mesh = childMeshes[i];
                var mr = child.AddComponent<MeshRenderer>();
                mr.material = new Material(Shader.Find("Standard"));
                mr.material.SetColor("_Color", loadedMeshColor);

                // Set name appropraitely
                child.name = childMeshes[i].name;

                // Set transform parent to be the empty gameobject generated
                child.transform.SetParent(go.transform);
            }
        }
    }
}
