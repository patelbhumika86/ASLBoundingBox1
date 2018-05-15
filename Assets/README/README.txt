I've created a basic Unity project (version 5.6.4f in case any problems come up with compatibility) that lets you save and load up meshes dynamically in Unity to see how they look.

I've made some quick screenshots of the project to help as references. Wherever you see "REFERENCE", go to the Assets / README and look at the picture to get a better idea for what I mean.

In order to use the project, go to "Assets / Scenes" and open up the MeshSaver scene. (REFERENCE: MeshSaverScene.png)

You should see several items in the scene:

  1) Main Camera
  2) Directional Light
  3) MeshSaverInterface
  4) ExampleMeshParent
     a) Cube
     b) Cylinder

If you click on the MeshSaverInterface object, you'll see a MeshSaverInterface script pop up in the Inspector. (REFERENCE: MeshSaverInterfaceInInspector.png)

Adjust the filename in the MeshSaverInterface Inspector however you'd like (it doesn't expect a file extension like ".txt" though). You'll also probably want to update the directory to be the directory for the Assets folder of the project (it's currently set to my computer's directory for that). When you save the file, you should see it pop up immediately in whichever directory you save it to. (REFERENCE: MyMeshesTextFileSavedHere.png).

I make some assumptions for Windows platform file writing when fixing the directory path used, so it'll probably be a little different for the iMac. If the logic needs to be adjusted, you can click on the script in MeshSaverInterface to open up the script. (REFERENCE: MeshSaverInterfaceScript.png) From there, you'll probably want to adjust the FixDirectory method.

I assume for the mesh creation that there's an empty GameObject that serves as the parent. (That's how the room meshes were for the Hololens). The actual meshes are taken only from the children. In this case, ExampleMeshParent is the game object and Cube and Cylinder are the children. You'll have to drag ExampleMeshParent over to the "Mesh Parent" field in the Inspector for MeshSaverInterface. (REFERENCE: PlayButtonAndMeshParentExample.png) Once you're sure the directory, filename, and mesh parent are set correctly, you can press the Play button at the top, then the Save Mesh button in the Inspector. (REFERENCE: PlayButtonAndMeshParentExample.png) The filepath that the file will be saved to will show up after you press Play for the Unity Editor, so I'd double check it to make sure it's right.

The name grabbing for the generated mesh object parent is lazy, and the texturing just uses whichever color you pick for "Loaded Mesh Color" in MeshSaverInterface, but it should grab mesh vertices and triangles at their global positions and save them efficiently.

Good luck!