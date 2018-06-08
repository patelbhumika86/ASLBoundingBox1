using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestScript : MonoBehaviour {
	public string FileName = "MyMeshes0352.txt";
	public string Directory = "/Users/bhumi/Documents/Capstone/testFiles";
	string FilePath = "";
	Bounds GameObjBounds;
	Bounds BBBounds;

	Color loadedMeshColor = Color.grey;
	public Vector3[] MinAndMax = new Vector3[2];


	public Color LineColor = Color.red;

//	public GameObject MeshObjectToDisplayBoundsFor;
	[HideInInspector]
	public GameObject ParentRoom;

	void Start() {
		FilePath = string.Join("/", new string[2] { Directory,FileName });
		LoadMesh();
	}
	public void Update()
	{
		VisualizeBoxFromMinAndMax();

		GetGameObjectsInHierarchy (ParentRoom);
//		VisualizeGameObjectBounds();
//		CheckIntersection (ParentRoom);
		check();

	}
	public bool CheckIntersection (GameObject go){
		GameObjBounds = GetBoundsFor(go);
		if (BBBounds.Intersects (GameObjBounds)) {
//			Debug.Log (go.name);
			return true;
//			Debug.Log (go.name + "Bounds intersecting");
		} else {
			return false;
//			Debug.Log (go.name + "Not intersecting");
		}
	}
	public void LoadMesh()
	{
		if (!string.IsNullOrEmpty(FilePath)
			&& File.Exists(FilePath)){
//			Debug.Log (FilePath);
			Mesh[] childMeshes = MeshSaver.LoadMesh(FilePath);

			ParentRoom = new GameObject();
			ParentRoom.name = "ParentRoom";

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
				child.transform.SetParent(ParentRoom.transform);
				child.SetActive (false);
			}
		} else {
			Debug.Log ("Enter valid File path");
		}
	}
	public void VisualizeBoxFromMinAndMax()
	{
		BoundCorners corners = CalculateCorners(MinAndMax[0], MinAndMax[1]);
		VisualizeBox(corners);
	}
	public void VisualizeGameObjectBounds(GameObject go)
	{
		GameObjBounds = GetBoundsFor(go);
		BoundCorners corners = CalculateCorners(GameObjBounds, go);
		VisualizeBox(corners);

	}

	public BoundCorners CalculateCorners(Vector3 min, Vector3 max)
	{
		Vector3 size = max - min;
		Vector3 extents = size / 2;
		Vector3 center = min + extents;
//		Vector3 center = min + ((max - min) / 2);

		BBBounds = new Bounds(center, size);

		BoundCorners corners = CalculateCorners(BBBounds, null);
		return corners;
	}
	public void VisualizeBox(BoundCorners corners)//for bb
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

		BoundCorners corners = new BoundCorners(cornerMin, cornerFTL, cornerFTR, cornerFBR, cornerBBL, cornerBTL, cornerMax, cornerBBR);

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
//			Debug.Log (go.name+":  "+ b.min + ", " + b.max);

			return b;
		}
		else
		{
			Debug.LogError("Could not find MeshFilter component for GameObject. Unable to calculate bounds to draw them.");
			return new Bounds();
		}
	}
	public Vector3 RotateBoundingBoxPoint(Vector3 point, GameObject go)
	{
		Vector3 pivot = go.transform.position;
		Quaternion quat = go.transform.rotation;
		Vector3 dir = point - pivot;

		Vector3 rotatedDir = quat * dir;
		return pivot + rotatedDir;
	}
	public void check()
	{
		var goList = GetGameObjectsInHierarchy(ParentRoom);
		foreach(var go in goList)
		{
			if (CheckIntersection (go)) {
				go.SetActive (true);
				VisualizeGameObjectBounds (go);
			} 
		}
	}
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
//				CheckIntersection(child);
			}
		}
	}
	#endregion
}