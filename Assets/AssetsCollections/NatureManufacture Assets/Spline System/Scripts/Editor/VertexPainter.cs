using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class VertexPainter : EditorWindow
{
	bool drawing = false;
	bool showVertexColors = false;
	GameObject currentDrawObject;

	List<MeshFilter> meshFilters = new List<MeshFilter> ();
	List<Material> oldMaterials = new List<Material> ();


	Color drawColor = Color.white;
	float drawSize = 0.5f;
	float opacity = 1f;

	[MenuItem ("Tools/Nature Manufacture/Vertex Painter")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		VertexPainter window = EditorWindow.GetWindow<VertexPainter> ("VertexPainter");
		window.Show ();
	}

	void OnGUI ()
	{
		
		if (!drawing) {
			if (GUILayout.Button ("Start Drawing")) {
				if (Selection.activeGameObject != null) {
					EditorSceneManager.MarkSceneDirty (SceneManager.GetActiveScene ());
					meshFilters.Clear ();
					MeshFilter[] filters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter> ();

					foreach (var item in filters) {
						currentDrawObject = Selection.activeGameObject;
						meshFilters.Add (item);

						drawing = true;
						Tools.current = Tool.None;

						Undo.RecordObject (item.sharedMesh, "Start draw vertex");
					}

				}
			}
		}

		if (drawing) {

			if (Selection.activeGameObject != currentDrawObject) {
				StopDrawing ();
			}

			if (GUILayout.Button ("End Drawing")) {
				StopDrawing ();


			}

			EditorGUILayout.Space ();
			if (!showVertexColors) {
				if (GUILayout.Button ("Show vertex colors")) {
					oldMaterials.Clear ();

					for (int i = 0; i < meshFilters.Count; i++) {
						oldMaterials.Add (meshFilters [i].GetComponent<MeshRenderer> ().sharedMaterial);
						meshFilters [i].GetComponent<MeshRenderer> ().sharedMaterial = new Material (Shader.Find ("NatureManufacture Shaders/Debug/Vertex color"));
					}

					showVertexColors = true;
				}
			} else {
				if (GUILayout.Button ("Hide vertex colors")) {
					for (int i = 0; i < meshFilters.Count; i++) {
						meshFilters [i].GetComponent<MeshRenderer> ().sharedMaterial = oldMaterials [i];
					}

					showVertexColors = false;
				}
			}

			if (GUILayout.Button ("Reset colors")) {
				RestartColor ();
			}


			EditorGUILayout.HelpBox ("River Auto Material -> R Wetness", MessageType.Info);
		}	

		drawColor = EditorGUILayout.ColorField ("Draw color", drawColor);
		opacity = EditorGUILayout.FloatField ("Opacity", opacity);
		drawSize = EditorGUILayout.FloatField ("Size", drawSize);
		if (drawSize < 0) {
			drawSize = 0;
		}
		EditorGUILayout.HelpBox ("R - Emission G- Bottom Cover B - Top Cover", MessageType.Info);
		EditorGUILayout.Space ();
	
	}

	void StopDrawing ()
	{
		meshFilters.Clear ();
		if (showVertexColors) {
			for (int i = 0; i < meshFilters.Count; i++) {
				meshFilters [i].GetComponent<MeshRenderer> ().sharedMaterial = oldMaterials [i];
			}
			showVertexColors = false;
		}
		currentDrawObject = null;

		drawing = false;


	}

	protected virtual void OnSceneGUI (SceneView sceneView)
	{
		
		if (Selection.activeGameObject != currentDrawObject && drawing) {
			StopDrawing ();
		}
		
		Color baseColor = Handles.color;


		if (currentDrawObject != null) {


			if (drawing) {

				DrawOnVertexColors ();
			}
		}
	}

	void RestartColor ()
	{
		foreach (var item in meshFilters) {
			Mesh mesh = item.sharedMesh; 
			if (!string.IsNullOrEmpty (AssetDatabase.GetAssetPath (mesh))) {
				mesh = Instantiate<Mesh> (item.sharedMesh);
				item.sharedMesh = mesh;

			}

			mesh.colors = null;

		}
	}

	void DrawOnVertexColors ()
	{

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));



		Camera sceneCamera = SceneView.lastActiveSceneView.camera;
		Vector2 mousePos = Event.current.mousePosition;
		mousePos.y = Screen.height - mousePos.y - 40;
		Ray ray = sceneCamera.ScreenPointToRay (mousePos);

		List<MeshCollider> meshColliders = new List<MeshCollider> ();
		foreach (var item in meshFilters) {
			meshColliders.Add (item.gameObject.AddComponent<MeshCollider> ());
		}

		RaycastHit[] hits = Physics.RaycastAll (ray, Mathf.Infinity);

		Vector3 hitPosition = Vector3.zero;
		Vector3 hitNormal = Vector3.zero;
		if (hits.Length > 0) {

			foreach (var hit in hits) {
				
				if (hit.collider is MeshCollider) {
					MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter> ();

					hitPosition = hit.point;
					hitNormal = hit.normal;

					Handles.color = new Color (drawColor.r, drawColor.g, drawColor.b, 1);
					Handles.DrawLine (hitPosition, hitPosition + hitNormal * 2);
					Handles.CircleHandleCap (
						GUIUtility.GetControlID (FocusType.Passive),
						hitPosition,
						Quaternion.LookRotation (hitNormal),
						drawSize,
						EventType.Repaint
					);
					Handles.color = Color.black;
					Handles.CircleHandleCap (
						GUIUtility.GetControlID (FocusType.Passive),
						hitPosition,
						Quaternion.LookRotation (hitNormal),
						drawSize * 0.95f,
						EventType.Repaint
					);

					foreach (var currentMeshFilter in meshFilters) {
											
						if (meshFilter == currentMeshFilter) {
							

						

							if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
								continue;


							if (meshFilter.sharedMesh != null) {
								


								Mesh mesh = meshFilter.sharedMesh; 
								if (!string.IsNullOrEmpty (AssetDatabase.GetAssetPath (mesh))) {
									mesh = Instantiate<Mesh> (meshFilter.sharedMesh);
									meshFilter.sharedMesh = mesh;

								}

								int vertLength = mesh.vertices.Length;
								Vector3[] vertices = mesh.vertices;
								Color[] colors = mesh.colors;
								Transform transform = meshFilter.transform;
								if (colors.Length == 0) {
									colors = new Color[vertLength];
									for (int i = 0; i < colors.Length; i++) {
										colors [i] = Color.white;
									}

								}



								for (int i = 0; i < vertLength; i++) {
									
									float dist = Vector3.Distance (hitPosition, transform.TransformPoint (vertices [i]));

									if (dist < drawSize) {
										
										if (Event.current.shift)
											colors [i] = Color.Lerp (colors [i], Color.white, opacity);
										else
											colors [i] = Color.Lerp (colors [i], drawColor, opacity);					

									}
								}


								mesh.colors = colors;

							}
						} 
					}
				}
			}
		}

		foreach (var item in meshColliders) {
			DestroyImmediate (item);
		}

	}


	void OnFocus ()
	{
		
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDestroy ()
	{
		
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		if (drawing)
			StopDrawing ();
		
	}
}
