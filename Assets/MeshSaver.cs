using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class MeshSaver
{
    /// <summary>
    /// Sub-class used to hold constant strings for exception messages, 
    /// debug messages, and user messages.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Use when mesh array passed in from other source is invalid for use.
        /// </summary>
        public static string InvalidMeshArray = "Invalid mesh array. Mesh was null or had zero entries.";
        public static string InvalidMesh = "Invalid mesh found. Mesh was null.";
        public static string MeshFileNotFound = "Mesh file does not exist at given path!";
    }

    #region Constants
    public static string Separator = "\n";
    public static string MeshID = "o ";
    public static string VertexID = "v ";
    public static string VertexNormalID = "vn ";
    public static string TriangleID = "f ";

    public static string SupplementaryInfoSeparator = "===";
    public static string PositionID = "MeshPositions";
    public static string RotationID = "MeshRotations";
    #endregion

    #region Methods
    #region Main Methods

    #region Save Mesh
    public static void SaveObjectMesh(GameObject go, string filepath)
    {
        if (go != null)
        {
            //SaveMesh(go);
            GameObject[] goArray = new GameObject[go.transform.childCount]; // Doesn't include parent because I assume the parent is an empty GameObject used to group up all children
            for (int i = 0; i < go.transform.childCount; i++)
            {
				if (go.transform.GetChild(i).gameObject.activeInHierarchy) {
					goArray [i] = go.transform.GetChild (i).gameObject;
				}

				Debug.Log ("Adding info for child " + i);
            }

            SaveMesh(goArray, filepath);
        }
        else
        {
            Debug.Log("Gameobject does not have a mesh filter to retrieve a mesh from.");
        }
    }

    /// <summary>
    /// Saves a mesh without adjusting the vertices to show the absolute position 
    /// and orientation of the object (what it was doing before).
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="meshName"></param>
    /// <returns></returns>
    private static void SaveMesh(GameObject go, string filepath)
    {
        if (go != null)
        {
            string meshString = WriteMeshString(go);
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            File.WriteAllText(filepath, meshString);
        }
        else
        {
            Debug.Log(Messages.InvalidMesh);
        }
    }
        
    /// <summary>
    /// Iterates through an array of meshes to generate a text file 
    /// representing this mesh.
    /// </summary>
    /// <param name="meshes">
    /// A non-null array of meshes with vertices and triangles.
    /// </param>
    private static void SaveMesh(GameObject[] goArray, string filepath)
    {
        if (goArray != null && goArray.Length > 0)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < goArray.Length; i++)
            {
                sb.Append(WriteMeshString(goArray[i], i));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            File.WriteAllText(filepath, sb.ToString());

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        else
        {
            Debug.Log(Messages.InvalidMeshArray);
        }
    }

    private static string WriteMeshString(GameObject go)
    {
        return WriteMeshString(go, 0);
    }

    /// <summary>
    /// Logic for writing a mesh to a string (i.e. for writing to a text file for transferral of meshes).
    /// 
    /// 
    /// General structure:
    /// o MeshName
    /// v vertexX vertexY vertexZ
    /// 
    /// vn vertexNormalX vertexNormalY vertexNormalZ
    /// 
    /// f triangleVertex1//triangleVertex1 triangleVertex2//triangleVertex2 triangleVertex3//triangleVertex3
    /// 
    /// NOTE: Referenced from ExportOBJ file online @ http://wiki.unity3d.com/index.php?title=ExportOBJ
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private static string WriteMeshString(GameObject go, int meshIndexSuffix)
    {
        if (go != null
            && go.GetComponent<MeshFilter>() != null
            && go.GetComponent<MeshFilter>().mesh != null)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(MeshSaver.Separator);
            sb.Append(MeshID + go.name + "_" + meshIndexSuffix + '\n');

            Mesh mesh = go.GetComponent<MeshFilter>().mesh;

            // Convert mesh vertex positions to world coordinates (i.e. position and rotation baked in)
            Vector3[] meshVertices = mesh.vertices;
            for(int i = 0; i < meshVertices.Length; i++)
            {
                meshVertices[i] = go.transform.TransformPoint(meshVertices[i]);
            }
            foreach (Vector3 vertex in meshVertices)
            {
                sb.Append(GetVertexString(vertex));
            }
            sb.Append(Separator);
            foreach (Vector3 normal in mesh.normals)
            {
                sb.Append(GetVertexNormalString(normal));
            }
            sb.Append(Separator);
            // Append triangles (i.e. which vertices each triangle uses)
            for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
            {
                sb.Append(Separator);
                sb.Append("submesh" + submesh + "\n");
                int[] triangles = mesh.GetTriangles(submesh);
                for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex += 3)
                {
                    sb.Append(string.Format("f {0}//{0} {1}//{1} {2}//{2}\n", triangles[triangleIndex], triangles[triangleIndex + 1], triangles[triangleIndex + 2]));
                }
            }

            return sb.ToString();
        }
        else
        {
            Debug.Log(Messages.InvalidMesh);
            return "";
        }
    }
        
    /// <summary>
    /// Turns a vertex Vector3 into a readable vertex string of "v vertexX 
    /// vertexY vertexZ".
    /// </summary>
    /// <param name="vertex">
    /// A Vector3 representing a mesh vertex.
    /// </param>
    /// <returns>
    /// A readable string representing the vertex's entry in the mesh text 
    /// file.
    /// </returns>
    private static string GetVertexString(Vector3 vertex)
    {
        string vertexString = "";
        vertexString += VertexID;
        vertexString += vertex.x.ToString() + ' ';
        vertexString += vertex.y.ToString() + ' ';
        vertexString += vertex.z.ToString() + '\n';

        return vertexString;
    }

    /// <summary>
    /// Turns a vertexNormal Vector3 into a readable vertexNormal string of
    /// "vn vertexNormalX vertexNormalY vertexNormalZ".
    /// </summary>
    /// <param name="vertexNormal">
    /// A Vector3 representing a vertex normal (the normal pointing away 
    /// from the vertex that is calculated by the mesh when instantiated).
    /// </param>
    /// <returns>
    /// A readable string representing the vertex's entry in the mesh text file.
    /// </returns>
    private static string GetVertexNormalString(Vector3 vertexNormal)
    {
        string vertexNormalString = "";
        vertexNormalString += VertexNormalID;
        vertexNormalString += vertexNormal.x.ToString() + ' ';
        vertexNormalString += vertexNormal.y.ToString() + ' ';
        vertexNormalString += vertexNormal.z.ToString() + '\n';

        return vertexNormalString;
    }

    #endregion

    #region Load Mesh
    public static Mesh[] LoadMesh(string filepath)
    {
        if (File.Exists(filepath))
        {
            return LoadMesh(File.ReadAllLines(filepath));
        }
        else
        {
            Debug.Log(Messages.MeshFileNotFound);
            return null;
        }
    }

    public static Mesh[] LoadMesh(string[] fileContents)
    {
        // ID the markers at the beginning of each line
        string meshID = MeshID.TrimEnd();
        string vertexID = VertexID.TrimEnd();
        string vertexNormalID = VertexNormalID.TrimEnd();
        string triangleID = TriangleID.TrimEnd();

        // Initialize items used while reading the file
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        Mesh m = new Mesh();
        bool MeshRead = false;

        List<Mesh> meshesRead = new List<Mesh>();
            
        int lineIndex = 0;
        int meshIndex = 0;
        while (lineIndex < fileContents.Length)
        {
            string line = fileContents[lineIndex].Trim();
            string[] lineContents = line.Split(' ');
            if (lineContents.Length == 0)
            {
                // Ignore blank lines
                continue;
            }

            // ID the marker telling you what info the line contains
            string marker = lineContents[0];

            // marker = "o"
            if (marker.Equals(meshID))
            {
                // Demarcates a new mesh object -> create a new mesh to store info
                m = new Mesh();
                m.name = lineContents[1];
            }
            // marker = "v"
            else if (marker.Equals(vertexID))
            {
                // IDs a vertex to read in
                Vector3 vertex = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
                vertices.Add(vertex);
            }
            // marker = "vn"
            else if (marker.Equals(vertexNormalID))
            {
                // IDs a vertex normal to read in
                Vector3 normal = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
                normals.Add(normal);
            }
            // marker = "f"
            else if (marker.Equals(triangleID))
            {
                // IDs a set of vertices that make up a triangle
                do
                {
                    triangles.Add(int.Parse(lineContents[1].Split('/')[0]));
                    triangles.Add(int.Parse(lineContents[2].Split('/')[0]));
                    triangles.Add(int.Parse(lineContents[3].Split('/')[0]));

                    // Reset variables
                    ++lineIndex;
                    if (lineIndex < fileContents.Length)
                    {
                        line = fileContents[lineIndex];
                        lineContents = line.Split(' ');
                        marker = lineContents[0];
                    }
                    else
                    {
                        marker = "";
                    }
                } while (marker.Contains(triangleID));
                --lineIndex;

                MeshRead = true;
            }

            ++lineIndex;
            if (MeshRead)
            {
                //// Set appropriate values
                //m.SetVertices(vertices);
                //vertices = new List<Vector3>();
                //m.SetNormals(normals);
                //normals = new List<Vector3>();
                //m.SetTriangles(triangles.ToArray(), 0);
                //triangles = new List<int>();
                //m.RecalculateBounds();
                //m.RecalculateNormals();

                Mesh newMesh = InstantiateMesh(vertices, triangles.ToArray());
                vertices = new List<Vector3>();
                normals = new List<Vector3>();
                triangles = new List<int>();
                newMesh.name = m.name;

                meshesRead.Add(newMesh);
                
                MeshRead = false;
                ++meshIndex;
            }
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif

        return meshesRead.ToArray();
    }
        
    #endregion
    
        
    public static Mesh InstantiateMesh(List<Vector3> vertices, int[] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
    #endregion
    #endregion
}