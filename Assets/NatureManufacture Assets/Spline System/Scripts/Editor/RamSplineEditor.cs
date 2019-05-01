using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor (typeof(RamSpline)), CanEditMultipleObjects]
public class RamSplineEditor : Editor
{
	Vector2 scrollPos;


	RamSpline spline;

	Texture2D logo;
	int selectedPosition = -1;
	Vector3 pivotChange = Vector3.zero;


	public string[] toolbarStrings = new string[] {
		"Basic",
		"Points",
		"Vertex Color",
		"Flow Map\n[BETA]",
		"Terrain Shape\n[ALPHA]",
		"Tips"
	};
	//, "Debug" };

	//	/// <summary>
	//	/// The button editing style.
	//	/// </summary>
	//	GUIStyle buttonEditingStyle;

	[MenuItem ("GameObject/3D Object/Create River Spline")]
	static public void CreateSpline ()
	{
		GameObject gameobject = new GameObject ("RamSpline");
		gameobject.AddComponent<RamSpline> ();
		MeshRenderer meshRenderer = gameobject.AddComponent<MeshRenderer> ();
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	
		if (meshRenderer.sharedMaterial == null)
			meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material> ("Default-Diffuse.mat");
	

		Selection.activeGameObject = gameobject;
	}

	void CheckRotations ()
	{
		bool nan = false;
		if (spline.controlPointsRotations == null) {
			spline.controlPointsRotations = new List<Quaternion> ();
			nan = true;
		}
		if (spline.controlPoints.Count > spline.controlPointsRotations.Count) {
			nan = true;
			for (int i = 0; i < spline.controlPoints.Count - spline.controlPointsRotations.Count; i++) {
				spline.controlPointsRotations.Add (Quaternion.identity);
			}
		}
		for (int i = 0; i < spline.controlPointsRotations.Count; i++) {
			
			if (float.IsNaN (spline.controlPointsRotations [i].x) || float.IsNaN (spline.controlPointsRotations [i].y) || float.IsNaN (spline.controlPointsRotations [i].z) || float.IsNaN (spline.controlPointsRotations [i].w)) {
				spline.controlPointsRotations [i] = Quaternion.identity;
				nan = true;
			}
			if (spline.controlPointsRotations [i].x == 0 && spline.controlPointsRotations [i].y == 0 && spline.controlPointsRotations [i].z == 0 && spline.controlPointsRotations [i].w == 0) {
				
				spline.controlPointsRotations [i] = Quaternion.identity;
				nan = true;
			}
		
			spline.controlPointsRotations [i] = Quaternion.Euler (spline.controlPointsRotations [i].eulerAngles);		
		}



		if (nan)
			spline.GenerateSpline ();
	}

	public override void OnInspectorGUI ()
	{

	
		EditorGUILayout.Space ();
		logo = (Texture2D)Resources.Load ("logoRAM");



		Color baseCol = GUI.color;

		spline = (RamSpline)target;
	
		CheckRotations ();

		if (spline.controlPoints.Count > spline.controlPointsSnap.Count) {
			
			for (int i = 0; i < spline.controlPoints.Count - spline.controlPointsSnap.Count; i++) {
				spline.controlPointsSnap.Add (0);
			}
		}

		if (spline.controlPoints.Count > spline.controlPointsMeshCurves.Count) {

			for (int i = 0; i < spline.controlPoints.Count - spline.controlPointsMeshCurves.Count; i++) {
				spline.controlPointsMeshCurves.Add (new AnimationCurve (new Keyframe[]{ new Keyframe (0, 0), new Keyframe (1, 0) }));
			}
		}

	

		if (spline.controlPoints.Count > spline.controlPointsOrientation.Count)
			spline.GenerateSpline ();


		GUIContent btnTxt = new GUIContent (logo);

		var rt = GUILayoutUtility.GetRect (btnTxt, GUI.skin.label, GUILayout.ExpandWidth (false));
		rt.center = new Vector2 (EditorGUIUtility.currentViewWidth / 2, rt.center.y);

		GUI.Button (rt, btnTxt, GUI.skin.label);
		
		EditorGUI.BeginChangeCheck ();

		spline.toolbarInt = GUILayout.Toolbar (spline.toolbarInt, toolbarStrings);


		EditorGUILayout.Space ();


		spline.drawOnMesh = false;
		spline.drawOnMeshFlowMap = false;

	
		if (spline.showFlowMap) {
			if (spline.uvRotation)
				spline.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_RotateUV", 1);
			else
				spline.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_RotateUV", 0);
		}



		if (spline.toolbarInt == 0) {
	
			EditorGUILayout.HelpBox ("Add Point  - CTRL + Left Mouse Button Click", MessageType.Info);
			EditorGUI.indentLevel++;

			AddPointAtEnd ();

			EditorGUI.indentLevel--;
			EditorGUILayout.Space ();

			MeshSettings ();

		

			GUILayout.Label ("UV settings:", EditorStyles.boldLabel);
			if (spline.beginningSpline == null && spline.endingSpline == null) {
				spline.uvScale = EditorGUILayout.FloatField ("UV scale (texture tiling)", spline.uvScale);
			} else {

				spline.uvScaleOverride = EditorGUILayout.Toggle ("Parent UV scale override", spline.uvScaleOverride);
				if (!spline.uvScaleOverride) {
					if (spline.beginningSpline != null)
						spline.uvScale = spline.beginningSpline.uvScale;
					if (spline.endingSpline != null)
						spline.uvScale = spline.endingSpline.uvScale;
				

					GUI.enabled = false;
				}
				spline.uvScale = EditorGUILayout.FloatField ("UV scale (texture tiling)", spline.uvScale);
				GUI.enabled = true;


				
			}
			spline.invertUVDirection = EditorGUILayout.Toggle ("Invert UV direction", spline.invertUVDirection);

			spline.uvRotation = EditorGUILayout.Toggle ("Rotate UV", spline.uvRotation);


		

			//EditorGUILayout.Space ();

			//SetMaterials ();

		
			EditorGUILayout.Space ();
					
			ParentingSplineUI ();	
			EditorGUILayout.Space ();

	
			EditorGUILayout.Space ();

			GUILayout.Label ("Object settings:", EditorStyles.boldLabel);

			EditorGUILayout.Space ();
			if (GUILayout.Button ("Set object pivot to center")) {	
				Vector3 center = spline.meshfilter.sharedMesh.bounds.center;

				ChangePivot (center);

			}
			EditorGUILayout.BeginHorizontal ();
			{

				if (GUILayout.Button ("Set object pivot position")) {	
					ChangePivot (pivotChange - spline.transform.position);
				}
				pivotChange = EditorGUILayout.Vector3Field ("", pivotChange);



			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			if (GUILayout.Button (new GUIContent ("Regenerate spline", "Racalculates whole mesh"))) {	
				spline.GenerateSpline ();
			}

			if (GUILayout.Button ("Export as mesh")) {	

				string path = EditorUtility.SaveFilePanelInProject ("Save river mesh", "", "asset", "Save river mesh");


				if (path.Length != 0 && spline.meshfilter.sharedMesh != null) {

					AssetDatabase.CreateAsset (spline.meshfilter.sharedMesh, path);		

					AssetDatabase.Refresh ();
					spline.GenerateSpline ();
				}

			}

			EditorGUILayout.Space ();
			GUILayout.Label ("Debug Settings: ", EditorStyles.boldLabel);

			spline.debug = EditorGUILayout.Toggle ("Show debug gizmos", spline.debug);


		} else if (spline.toolbarInt == 1) {

			PointsUI ();

		} else if (spline.toolbarInt == 2) {
			
			DrawVertexColorsUI ();

		}
		if (spline.toolbarInt == 3) {

			DrawFlowColorsUI ();

		} else if (spline.toolbarInt == 4) {
			RiverChannel ();
		} else if (spline.toolbarInt == 5) {
			
			Tips ();
		} 
//		else if (spline.toolbarInt == 4) {
//
//			DebugOptions ();
//		}


		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (spline, "Spline changed");
			spline.GenerateSpline ();
		}

		EditorGUILayout.Space ();

		if (spline.beginningSpline) {
			if (!spline.beginningSpline.endingChildSplines.Contains (spline)) {
				spline.beginningSpline.endingChildSplines.Add (spline);

			}
		}

		if (spline.endingSpline) {
			if (!spline.endingSpline.beginnigChildSplines.Contains (spline)) {
				spline.endingSpline.beginnigChildSplines.Add (spline);

			}
		}
	}

	void RiverChannel ()
	{
		spline.terrainCurve = EditorGUILayout.CurveField ("Terrain curve", spline.terrainCurve);
		//spline.detailTerrain = EditorGUILayout.IntSlider ("Terrain sampling transverse", spline.detailTerrain, 100, 100);
		//spline.detailTerrainForward = EditorGUILayout.IntSlider ("Terrain sampling", spline.detailTerrainForward, 100, 100);

		if (GUILayout.Button ("Shape terrain")) {
			RiverCurveTerrain ();

		}
	}

	void RiverCurveTerrain ()
	{
		Mesh mesh = spline.meshfilter.sharedMesh;
		Dictionary<Terrain, float[,]> oldterrainHeights = new Dictionary<Terrain, float[,]> ();
		Dictionary<Terrain, float[,]> terrainHeights = new Dictionary<Terrain, float[,]> ();
		RaycastHit hit;
		Vector3 rayPointDown;
		Vector3 rayPointUp;
		Vector3 point;
		float[,] heightmapData;
		float[,] oldHeightmapData;
		for (int i = 0; i < spline.pointsDown.Count - 1; i++) {
			for (int tf = 0; tf <= spline.detailTerrainForward; tf++) {
				for (int t = 0; t <= spline.detailTerrain; t++) {
					rayPointDown = Vector3.Lerp (spline.pointsDown [i], spline.pointsDown [i + 1], tf / (float)spline.detailTerrainForward);
					rayPointUp = Vector3.Lerp (spline.pointsUp [i], spline.pointsUp [i + 1], tf / (float)spline.detailTerrainForward);
					point = Vector3.Lerp (rayPointDown, rayPointUp, t / (float)spline.detailTerrain) + spline.transform.position;
					if (Physics.Raycast (point + Vector3.up * 50, Vector3.down, out hit)) {
						
						Terrain terrain = hit.collider.GetComponent<Terrain> ();/// Terrain.activeTerrain;
						 
						if (terrain != null) {
							TerrainData terrainData = terrain.terrainData;
							if (terrainHeights.ContainsKey (terrain)) {
								heightmapData = terrainHeights [terrain];
								oldHeightmapData = oldterrainHeights [terrain];
							} else {
								Undo.RegisterCompleteObjectUndo (terrainData, "River curve");
								heightmapData = terrainData.GetHeights (0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
								terrainHeights.Add (terrain, heightmapData);
								oldHeightmapData = terrainData.GetHeights (0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
								oldterrainHeights.Add (terrain, oldHeightmapData);
							}
							Vector3 heightmapPos = hit.point - terrain.transform.position;
							float terrainToheight = (1 / terrainData.size.x * terrain.terrainData.heightmapWidth);
							float terrainTowidth = (1 / terrainData.size.z * terrain.terrainData.heightmapHeight);
							heightmapPos.x = heightmapPos.x * terrainToheight;
							heightmapPos.z = heightmapPos.z * terrainTowidth;
							heightmapPos.x = Mathf.Clamp (heightmapPos.x, 0, terrain.terrainData.heightmapWidth - 1);
							heightmapPos.z = Mathf.Clamp (heightmapPos.z, 0, terrain.terrainData.heightmapHeight - 1);
							float height = point.y - terrain.transform.position.y + spline.terrainCurve.Evaluate (t / (float)spline.detailTerrain);
							heightmapData [(int)heightmapPos.z, (int)heightmapPos.x] = height / (float)terrainData.size.y;
						}
					}
				}
			}
		}
		foreach (var item in terrainHeights) {
			item.Key.terrainData.SetHeights (0, 0, item.Value);
		}

	}

	void DebugOptions ()
	{
		EditorGUILayout.LabelField ("splitParameter", spline.uvBeginning.ToString ());
		EditorGUILayout.LabelField ("beginningMinWidth", spline.beginningMinWidth.ToString ());
		EditorGUILayout.LabelField ("beginningMaxWidth", spline.beginningMaxWidth.ToString ());
		EditorGUILayout.LabelField ("minMaxWidth", spline.minMaxWidth.ToString ());
		EditorGUILayout.LabelField ("uvBeginning", spline.uvBeginning.ToString ());
		EditorGUILayout.LabelField ("uvWidth", spline.uvWidth.ToString ());
		if (GUILayout.Button (new GUIContent ("Regenerate spline", "Racalculates whole mesh"))) {	
			spline.GenerateSpline ();
		}
	}


	void AddPointAtEnd ()
	{
		if (GUILayout.Button ("Add point at end")) {
			
			int i = spline.controlPoints.Count - 1;
			Vector4 position = Vector3.zero;
			position.w = spline.width;
			if (i < spline.controlPoints.Count - 1 && spline.controlPoints.Count > i + 1) {
				position = spline.controlPoints [i];
				Vector4 positionSecond = spline.controlPoints [i + 1];
				if (Vector3.Distance ((Vector3)positionSecond, (Vector3)position) > 0)
					position = (position + positionSecond) * 0.5f;
				else
					position.x += 1;
			} else if (spline.controlPoints.Count > 1 && i == spline.controlPoints.Count - 1) {
				position = spline.controlPoints [i];
				Vector4 positionSecond = spline.controlPoints [i - 1];
				if (Vector3.Distance ((Vector3)positionSecond, (Vector3)position) > 0)
					position = position + (position - positionSecond);
				else
					position.x += 1;
			} else if (spline.controlPoints.Count > 0) {
				position = spline.controlPoints [i];
				position.x += 1;
			}
			spline.controlPointsRotations.Add (Quaternion.identity);
			spline.controlPoints.Add (position);
			spline.controlPointsSnap.Add (0);
			spline.controlPointsMeshCurves.Add (new AnimationCurve (new Keyframe[] {
				new Keyframe (0, 0),
				new Keyframe (1, 0)
			}));
			spline.GenerateSpline ();
		}
	}

	void MeshSettings ()
	{
		GUILayout.Label ("Mesh settings:", EditorStyles.boldLabel);
		EditorGUI.indentLevel++;

		spline.currentProfile = (SplineProfile)EditorGUILayout.ObjectField ("Spline profile", spline.currentProfile, typeof(SplineProfile), false);

		if (GUILayout.Button ("Create profile from settings")) {


			SplineProfile asset = ScriptableObject.CreateInstance<SplineProfile> ();


			asset.meshCurve = spline.meshCurve;

			MeshRenderer ren = spline.GetComponent<MeshRenderer> ();
			asset.splineMaterial = ren.sharedMaterial;

			asset.minVal = spline.minVal;
			asset.maxVal = spline.maxVal;


			asset.traingleDensity = spline.traingleDensity;
			asset.vertsInShape = spline.vertsInShape;

			asset.uvScale = spline.uvScale;

			asset.uvRotation = spline.uvRotation;

			asset.flowFlat = spline.flowFlat;
			asset.flowWaterfall = spline.flowWaterfall;

			string path = EditorUtility.SaveFilePanelInProject ("Save new spline profile", spline.gameObject.name + ".asset", "asset", "Please enter a file name to save the spline profile to");



			AssetDatabase.CreateAsset (asset, path);
			AssetDatabase.SaveAssets ();
			spline.currentProfile = asset;
		}
		if (GUILayout.Button ("Save profile from settings")) {
			
			spline.currentProfile.meshCurve = spline.meshCurve;

			MeshRenderer ren = spline.GetComponent<MeshRenderer> ();
			spline.currentProfile.splineMaterial = ren.sharedMaterial;

			spline.currentProfile.minVal = spline.minVal;
			spline.currentProfile.maxVal = spline.maxVal;


			spline.currentProfile.traingleDensity = spline.traingleDensity;
			spline.currentProfile.vertsInShape = spline.vertsInShape;

			spline.currentProfile.uvScale = spline.uvScale;

			spline.currentProfile.uvRotation = spline.uvRotation;

			spline.currentProfile.flowFlat = spline.flowFlat;
			spline.currentProfile.flowWaterfall = spline.flowWaterfall;

			AssetDatabase.SaveAssets ();
		}
	

		if (spline.currentProfile != null && spline.currentProfile != spline.oldProfile) {

			ResetToProfile ();
			EditorUtility.SetDirty (spline);

		}

		if (CheckProfileChange ())
			EditorGUILayout.HelpBox ("Profile data changed.", MessageType.Info);

		if (spline.currentProfile != null && GUILayout.Button ("Reset to profile")) {

			ResetToProfile ();
		}
		EditorGUILayout.Space ();


		string meshResolution = "Triangles density";
		if (spline.meshfilter != null && spline.meshfilter.sharedMesh != null) {
			float tris = spline.meshfilter.sharedMesh.triangles.Length / 3;
			meshResolution += " (" + tris + " tris)";
		} else if (spline.meshfilter != null && spline.meshfilter.sharedMesh == null) {
			spline.GenerateSpline ();
		}


		EditorGUILayout.LabelField (meshResolution);
		EditorGUI.indentLevel++;
		spline.traingleDensity = 1 / (float)EditorGUILayout.IntSlider ("U", (int)(1 / (float)spline.traingleDensity), 1, 100);

		if (spline.beginningSpline == null && spline.endingSpline == null) {
			
			spline.vertsInShape = EditorGUILayout.IntSlider ("V", spline.vertsInShape - 1, 1, 20) + 1;

		} else {
			GUI.enabled = false;
			if (spline.beginningSpline != null) {
				spline.vertsInShape = (int)Mathf.Round ((spline.beginningSpline.vertsInShape - 1) * (spline.beginningMaxWidth - spline.beginningMinWidth) + 1);
			} else if (spline.endingSpline != null)
				spline.vertsInShape = (int)Mathf.Round ((spline.endingSpline.vertsInShape - 1) * (spline.endingMaxWidth - spline.endingMinWidth) + 1);
			
			EditorGUILayout.IntSlider ("V", spline.vertsInShape - 1, 1, 20);
			GUI.enabled = true;

		}

		EditorGUI.indentLevel--;
		EditorGUILayout.Space ();


		EditorGUILayout.BeginHorizontal ();
		{
			spline.width = EditorGUILayout.FloatField ("River width", spline.width);
			if (GUILayout.Button ("Change width for whole river")) {
				if (spline.width > 0) {
					for (int i = 0; i < spline.controlPoints.Count; i++) {
						Vector4 point = spline.controlPoints [i];
						point.w = spline.width;
						spline.controlPoints [i] = point;
					}
					spline.GenerateSpline ();
				}
			}
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Space ();


		spline.meshCurve = EditorGUILayout.CurveField ("Mesh curve", spline.meshCurve);
		if (GUILayout.Button ("Set all mesh curves")) {
			for (int i = 0; i < spline.controlPointsMeshCurves.Count; i++) {
				spline.controlPointsMeshCurves [i] = new AnimationCurve (spline.meshCurve.keys);
			}
		}
		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ("Vertice distribution: " + spline.minVal.ToString () + " " + spline.maxVal.ToString ());
		EditorGUILayout.MinMaxSlider (ref spline.minVal, ref spline.maxVal, 0, 1);

		//Debug.Log (spline.minVal + " " + spline.maxVal);

		spline.minVal = (int)(spline.minVal * 100) * 0.01f;
		spline.maxVal = (int)(spline.maxVal * 100) * 0.01f;


		if (spline.minVal > 0.5f)
			spline.minVal = 0.5f;
		
		if (spline.minVal < 0.01f)
			spline.minVal = 0.01f;
		
		if (spline.maxVal < 0.5f)
			spline.maxVal = 0.5f;
		
		if (spline.maxVal > 0.99f)
			spline.maxVal = 0.99f;
		
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Snap/Unsnap mesh to terrain")) {
			spline.snapToTerrain = !spline.snapToTerrain;
			for (int i = 0; i < spline.controlPointsSnap.Count; i++) {
				spline.controlPointsSnap [i] = spline.snapToTerrain == true ? 1 : 0;
			}
		}
		///spline.snapMask = EditorGUILayout.MaskField ("Layers", spline.snapMask, InternalEditorUtility.layers);
		spline.snapMask = LayerMaskField ("Layers", spline.snapMask, true);

		spline.normalFromRaycast = EditorGUILayout.Toggle ("Take Normal from terrain", spline.normalFromRaycast);
		EditorGUILayout.Space ();
	}

	bool CheckProfileChange ()
	{
		if (spline.currentProfile == null)
			return false;
		//if (ren.material != spline.currentProfile.splineMaterial)
		//	return true;

		if (spline.minVal != spline.currentProfile.minVal)
			return true;
		if (spline.maxVal != spline.currentProfile.maxVal)
			return true;
				
		if (spline.traingleDensity != spline.currentProfile.traingleDensity)
			return true;
		if (spline.vertsInShape != spline.currentProfile.vertsInShape)
			return true;
						
		if (spline.uvScale != spline.currentProfile.uvScale)
			return true;
							
		if (spline.uvRotation != spline.currentProfile.uvRotation)
			return true;


		if (spline.flowFlat != spline.currentProfile.flowFlat)
			return true;

		if (spline.flowWaterfall != spline.currentProfile.flowWaterfall)
			return true;

		return false;
	}

	void ResetToProfile ()
	{
		spline.meshCurve = spline.currentProfile.meshCurve;
		for (int i = 0; i < spline.controlPointsMeshCurves.Count; i++) {
			spline.controlPointsMeshCurves [i] = new AnimationCurve (spline.meshCurve.keys);
		}
		MeshRenderer ren = spline.GetComponent<MeshRenderer> ();
		ren.sharedMaterial = spline.currentProfile.splineMaterial;

		spline.minVal = spline.currentProfile.minVal;
		spline.maxVal = spline.currentProfile.maxVal;


		spline.traingleDensity = spline.currentProfile.traingleDensity;
		spline.vertsInShape = spline.currentProfile.vertsInShape;

		spline.uvScale = spline.currentProfile.uvScale;

		spline.uvRotation = spline.currentProfile.uvRotation;

		spline.flowFlat = spline.currentProfile.flowFlat;
		spline.flowWaterfall = spline.currentProfile.flowWaterfall;

		spline.GenerateSpline ();
		spline.oldProfile = spline.currentProfile;
	}

	void DrawVertexColorsUI ()
	{
		spline.drawOnMesh = true;
		if (spline.drawOnMesh) {


			EditorGUILayout.HelpBox ("R - Slow Water G - Small Cascade B - Big Cascade A - Opacity", MessageType.Info);
			EditorGUILayout.Space ();
			spline.drawColor = EditorGUILayout.ColorField ("Draw color", spline.drawColor);

			spline.opacity = EditorGUILayout.FloatField ("Opacity", spline.opacity);
			spline.drawSize = EditorGUILayout.FloatField ("Size", spline.drawSize);
			if (spline.drawSize < 0) {
				spline.drawSize = 0;
			}
		}

		EditorGUILayout.Space ();
		if (!spline.showVertexColors) {
			if (GUILayout.Button ("Show vertex colors")) {
				
				if (!spline.showFlowMap && !spline.showVertexColors)
					spline.oldMaterial = spline.GetComponent<MeshRenderer> ().sharedMaterial;
				ResetMaterial ();
				spline.GetComponent<MeshRenderer> ().sharedMaterial = new Material (Shader.Find ("NatureManufacture Shaders/Debug/Vertex color"));
				spline.showVertexColors = true;
			}
		} else {
			if (GUILayout.Button ("Hide vertex colors")) {
				ResetMaterial ();
				spline.GetComponent<MeshRenderer> ().sharedMaterial = spline.oldMaterial;
				spline.showVertexColors = false;
			}
		}

		if (GUILayout.Button ("Reset vertex colors") && EditorUtility.DisplayDialog ("Reset vertex colors?",
			    "Are you sure you want to reset f vertex colors?", "Yes", "No")) {
			spline.colors = null;
			spline.GenerateSpline ();
		}
	}

	void DrawFlowColorsUI ()
	{
		spline.drawOnMeshFlowMap = true;
		if (spline.drawOnMeshFlowMap) {
			EditorGUILayout.HelpBox ("Sharp gradient could generate bugged effect. Keep flow changes smooth.", MessageType.Info);
			EditorGUILayout.Space ();
			spline.flowSpeed = EditorGUILayout.Slider ("Flow speed", spline.flowSpeed, -1, 1);
			spline.flowDirection = EditorGUILayout.Slider ("Flow direction", spline.flowDirection, -1, 1);
			spline.opacity = EditorGUILayout.FloatField ("Opacity", spline.opacity);
			spline.drawSize = EditorGUILayout.FloatField ("Size", spline.drawSize);
			if (spline.drawSize < 0) {
				spline.drawSize = 0;
			}
		}

		EditorGUILayout.Space ();
		if (!spline.showFlowMap) {
			


			if (GUILayout.Button ("Show flow directions")) {
				if (!spline.showFlowMap && !spline.showVertexColors)
					spline.oldMaterial = spline.GetComponent<MeshRenderer> ().sharedMaterial;
				ResetMaterial ();
				spline.GetComponent<MeshRenderer> ().sharedMaterial = new Material (Shader.Find ("NatureManufacture Shaders/Debug/Flowmap Direction"));
				spline.GetComponent<MeshRenderer> ().sharedMaterial.SetTexture ("_Direction", Resources.Load<Texture2D> ("Debug_Arrow"));



				spline.showFlowMap = true;
			}
			if (GUILayout.Button ("Show flow smoothness")) {
				if (!spline.showFlowMap && !spline.showVertexColors)
					spline.oldMaterial = spline.GetComponent<MeshRenderer> ().sharedMaterial;
				ResetMaterial ();
				spline.GetComponent<MeshRenderer> ().sharedMaterial = new Material (Shader.Find ("NatureManufacture Shaders/Debug/FlowMapUV4"));
				spline.showFlowMap = true;
			}
		} 

		if (spline.showFlowMap) {
			
			if (GUILayout.Button ("Hide flow")) {
				ResetMaterial ();
				spline.GetComponent<MeshRenderer> ().sharedMaterial = spline.oldMaterial;
			}
		}

		EditorGUILayout.Space ();

		spline.flowFlat = EditorGUILayout.CurveField ("Flow curve flat speed", spline.flowFlat);
		spline.flowWaterfall = EditorGUILayout.CurveField ("Flow curve waterfall speed", spline.flowWaterfall);

		if (GUILayout.Button ("Reset flow to automatic") && EditorUtility.DisplayDialog ("Reset flow to automatic?",
			    "Are you sure you want to reset flow to automatic?", "Yes", "No")) {
			spline.overrideFlowMap = false;
			spline.GenerateSpline ();
		}
	}

	void ResetMaterial ()
	{
		//if (spline.oldMaterial != null)
		//	spline.GetComponent<MeshRenderer> ().sharedMaterial = spline.oldMaterial;
		spline.showFlowMap = false;
		spline.showVertexColors = false;
	}

	static void Tips ()
	{
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox ("\nReflections - Use box projection in reflection probes to get proper render even at multiple river conection.\n", MessageType.Info);
		EditorGUILayout.HelpBox ("\nKeep resonable quasi- square vertex shapes at river mesh, " + "this will give better tesselation result. Don't worry about low amount of poly, tesselation will smooth shapes.\n", MessageType.Info);
		EditorGUILayout.HelpBox ("\nBy rotating point you could get simmilar effect as vertex color painting.\n" + "You could adjust waterfalls or add noise in the river. " + "Note that if rotation will be bigger then +/- 90 degree you could invert normals.\n", MessageType.Info);
		EditorGUILayout.HelpBox ("\nUse low resolution reflection probes, and only around the water. " + "\nFar clip planes also should be short, you probably only need colors from the surounding world.\n", MessageType.Info);
		EditorGUILayout.HelpBox ("\nPut reflection probes behind , in and after dark area (tunel, cave) so you will get exelent result in lighting and reflections.\n", MessageType.Info);
		EditorGUILayout.HelpBox ("\nTry to keep quite simmilar distance between spline points. Huge distance between them could create strange result.\n", MessageType.Info);
		EditorGUILayout.HelpBox ("\nWhen you use multiple connected rivers, you shoud put reflection probe at fork of the rivers to keep proper reflections\n", MessageType.Info);


		EditorGUILayout.Space ();
	}



	void ParentingSplineUI ()
	{
		GUILayout.Label ("Rivers connections", EditorStyles.boldLabel);


		spline.beginningSpline = (RamSpline)EditorGUILayout.ObjectField ("Beginning spline", spline.beginningSpline, typeof(RamSpline), true);

		if (spline.beginningSpline == spline)
			spline.beginningSpline = null;
		
		spline.endingSpline = (RamSpline)EditorGUILayout.ObjectField ("Ending spline", spline.endingSpline, typeof(RamSpline), true);
		if (spline.endingSpline == spline)
			spline.endingSpline = null;
		

		if (spline.beginningSpline != null) {
			if (spline.controlPoints.Count > 0 && spline.beginningSpline.points.Count > 0) {
				spline.beginningMinWidth = spline.beginningMinWidth * (spline.beginningSpline.vertsInShape - 1);
				spline.beginningMaxWidth = spline.beginningMaxWidth * (spline.beginningSpline.vertsInShape - 1);
				EditorGUILayout.MinMaxSlider ("Part parent", ref spline.beginningMinWidth, ref spline.beginningMaxWidth, 0, spline.beginningSpline.vertsInShape - 1);
				spline.beginningMinWidth = (int)spline.beginningMinWidth;
				spline.beginningMaxWidth = (int)spline.beginningMaxWidth;
				spline.beginningMinWidth = Mathf.Clamp (spline.beginningMinWidth, 0, spline.beginningSpline.vertsInShape - 1);
				spline.beginningMaxWidth = Mathf.Clamp (spline.beginningMaxWidth, 0, spline.beginningSpline.vertsInShape - 1);
				if (spline.beginningMinWidth == spline.beginningMaxWidth) {
					if (spline.beginningMinWidth > 0)
						spline.beginningMinWidth--;
					else
						spline.beginningMaxWidth++;
				}
				spline.vertsInShape = (int)(spline.beginningMaxWidth - spline.beginningMinWidth) + 1;
				spline.beginningMinWidth = spline.beginningMinWidth / (float)(spline.beginningSpline.vertsInShape - 1);
				spline.beginningMaxWidth = spline.beginningMaxWidth / (float)(spline.beginningSpline.vertsInShape - 1);

				spline.GenerateBeginningParentBased ();
			}
		} else {
			spline.beginningMaxWidth = 1;
			spline.beginningMinWidth = 0;
		}


		if (spline.endingSpline != null) {
			if (spline.controlPoints.Count > 1 && spline.endingSpline.points.Count > 0) {
				spline.endingMinWidth = spline.endingMinWidth * (spline.endingSpline.vertsInShape - 1);
				spline.endingMaxWidth = spline.endingMaxWidth * (spline.endingSpline.vertsInShape - 1);

				EditorGUILayout.MinMaxSlider ("Part parent", ref spline.endingMinWidth, ref spline.endingMaxWidth, 0, spline.endingSpline.vertsInShape - 1);

				spline.endingMinWidth = (int)spline.endingMinWidth;
				spline.endingMaxWidth = (int)spline.endingMaxWidth;
				spline.endingMinWidth = Mathf.Clamp (spline.endingMinWidth, 0, spline.endingSpline.vertsInShape - 1);
				spline.endingMaxWidth = Mathf.Clamp (spline.endingMaxWidth, 0, spline.endingSpline.vertsInShape - 1);
				if (spline.endingMinWidth == spline.endingMaxWidth) {
					if (spline.endingMinWidth > 0)
						spline.endingMinWidth--;
					else
						spline.endingMaxWidth++;
				}
				spline.vertsInShape = (int)(spline.endingMaxWidth - spline.endingMinWidth) + 1;
				spline.endingMinWidth = spline.endingMinWidth / (float)(spline.endingSpline.vertsInShape - 1);
				spline.endingMaxWidth = spline.endingMaxWidth / (float)(spline.endingSpline.vertsInShape - 1);

				spline.GenerateEndingParentBased ();
			}
		} else {
			spline.endingMaxWidth = 1;
			spline.endingMinWidth = 0;
		}

	}

	void PointsUI ()
	{
		for (int i = 0; i < spline.controlPoints.Count; i++) {

			GUILayout.Label ("Point: " + i.ToString (), EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			EditorGUILayout.BeginHorizontal ();
			spline.controlPoints [i] = EditorGUILayout.Vector4Field ("", spline.controlPoints [i]);
			if (spline.controlPoints [i].w <= 0) {
				Vector4 vec4 = spline.controlPoints [i];
				vec4.w = 1;
				spline.controlPoints [i] = vec4;
			}
			if (GUILayout.Button (new GUIContent ("A", "Add point after this point"), GUILayout.MaxWidth (20))) {
				
				Vector4 position = spline.controlPoints [i];
				if (i < spline.controlPoints.Count - 1 && spline.controlPoints.Count > i + 1) {
					Vector4 positionSecond = spline.controlPoints [i + 1];
					if (Vector3.Distance ((Vector3)positionSecond, (Vector3)position) > 0)
						position = (position + positionSecond) * 0.5f;
					else
						position.x += 1;
				} else if (spline.controlPoints.Count > 1 && i == spline.controlPoints.Count - 1) {
					Vector4 positionSecond = spline.controlPoints [i - 1];
					if (Vector3.Distance ((Vector3)positionSecond, (Vector3)position) > 0)
						position = position + (position - positionSecond);
					else
						position.x += 1;
				} else {
					position.x += 1;
				}
				spline.controlPoints.Insert (i + 1, position);
				spline.controlPointsRotations.Insert (i + 1, Quaternion.identity);
				spline.controlPointsSnap.Insert (i + 1, 0);
				spline.controlPointsMeshCurves.Insert (i + 1, new AnimationCurve (new Keyframe[] {
					new Keyframe (0, 0),
					new Keyframe (1, 0)
				}));
				spline.GenerateSpline ();
			}
			if (GUILayout.Button (new GUIContent ("R", "Remove this Point"), GUILayout.MaxWidth (20))) {
				
				spline.controlPoints.RemoveAt (i);
				spline.controlPointsRotations.RemoveAt (i);
				spline.controlPointsMeshCurves.RemoveAt (i);
				spline.controlPointsSnap.RemoveAt (i);
				spline.GenerateSpline ();
			}
			if (GUILayout.Toggle (selectedPosition == i, new GUIContent ("S", "Select point"), "Button", GUILayout.MaxWidth (20))) {
				selectedPosition = i;
			} else if (selectedPosition == i) {
				selectedPosition = -1;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (spline.controlPointsRotations.Count > i)
				spline.controlPointsRotations [i] = Quaternion.Euler (EditorGUILayout.Vector3Field ("", spline.controlPointsRotations [i].eulerAngles));
			if (GUILayout.Button (new GUIContent ("    Clear rotation    ", "Clear Rotation"))) {
				spline.controlPointsRotations [i] = Quaternion.identity;
				spline.GenerateSpline ();
			}
			EditorGUILayout.EndHorizontal ();

			if (spline.controlPointsSnap.Count > i)
				spline.controlPointsSnap [i] = EditorGUILayout.Toggle ("Snap to terrain", spline.controlPointsSnap [i] == 1 ? true : false) == true ? 1 : 0;
			if (spline.controlPointsMeshCurves.Count > i)
				spline.controlPointsMeshCurves [i] = EditorGUILayout.CurveField ("Mesh curve", spline.controlPointsMeshCurves [i]);
			EditorGUILayout.Space ();
			EditorGUI.indentLevel--;
		}
	}

	void SetMaterials ()
	{
		GUILayout.Label ("Set materials: ", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		{
			
			if (GUILayout.Button ("Basic", GUILayout.MinWidth (80))) {
				try {
					string materialName = "RAM_River_Material_Gamma";
					if (PlayerSettings.colorSpace == ColorSpace.Linear)
						materialName = "RAM_River_Material_Linear";
					Material riverMat = (Material)Resources.Load (materialName);
					if (riverMat != null) {
						spline.GetComponent<MeshRenderer> ().sharedMaterial = riverMat;
					}
				} catch {
				}
			}
			if (GUILayout.Button ("Vertex color", GUILayout.MinWidth (80))) {
				try {
					string materialName = "RAM_River_Material_Gamma_Vertex_Color";
					if (PlayerSettings.colorSpace == ColorSpace.Linear)
						materialName = "RAM_River_Material_Linear_Vertex_Color";
					Material riverMat = (Material)Resources.Load (materialName);
					if (riverMat != null) {
						spline.GetComponent<MeshRenderer> ().sharedMaterial = riverMat;
					}
				} catch {
				}
			}
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Basic tesseled", GUILayout.MinWidth (80))) {
				try {
					string materialName = "RAM_River_Material_Gamma_Tess";
					if (PlayerSettings.colorSpace == ColorSpace.Linear)
						materialName = "RAM_River_Material_Linear_Tess";
					Material riverMat = (Material)Resources.Load (materialName);
					if (riverMat != null) {
						spline.GetComponent<MeshRenderer> ().sharedMaterial = riverMat;
					}
				} catch {
				}
			}
		
			if (GUILayout.Button ("Basic tesseled - vertex color", GUILayout.MinWidth (80))) {
				try {
					string materialName = "RAM_River_Material_Gamma_Tess_Vertex_Color";
					if (PlayerSettings.colorSpace == ColorSpace.Linear)
						materialName = "RAM_River_Material_Linear_Tess_Vertex_Color";
					Material riverMat = (Material)Resources.Load (materialName);
					if (riverMat != null) {
						spline.GetComponent<MeshRenderer> ().sharedMaterial = riverMat;
					}
				} catch {
				}
			}
		}
		EditorGUILayout.EndHorizontal ();
	}

	void ChangePivot (Vector3 center)
	{
		Vector3 position = spline.transform.position;
		spline.transform.position += center;
		for (int i = 0; i < spline.controlPoints.Count; i++) {
			Vector4 vec = spline.controlPoints [i];
			vec.x -= center.x;
			vec.y -= center.y;
			vec.z -= center.z;
			spline.controlPoints [i] = vec;
		}
		spline.GenerateSpline ();
	}


	protected virtual void OnSceneGUI ()
	{
		if (spline == null)
			spline = (RamSpline)target;
		
		Color baseColor = Handles.color;
		int controlId = GUIUtility.GetControlID (FocusType.Passive);

		if (spline != null) {

//			//river points terrain
//			for (int i = 0; i < spline.pointsDown.Count - 1; i++) {
//
//				for (int tf = 0; tf <= spline.detailTerrainForward; tf++) {
//
//					for (int t = 0; t <= spline.detailTerrain; t++) {
//
//						Vector3 rayPointDown = Vector3.Lerp (spline.pointsDown [i], spline.pointsDown [i + 1], tf / (float)spline.detailTerrainForward);
//						Vector3 rayPointUp = Vector3.Lerp (spline.pointsUp [i], spline.pointsUp [i + 1], tf / (float)spline.detailTerrainForward);
//						Vector3 point = Vector3.Lerp (rayPointDown, rayPointUp, t / (float)spline.detailTerrain) + spline.transform.position;
//						Handles.DrawLine (point + Vector3.up, point + Vector3.up * -1);
//							
//
//
//					}
//				}
//			}



			CheckRotations ();

			if (spline.drawOnMesh || spline.drawOnMeshFlowMap) {
				Tools.current = Tool.None;
				if (spline.meshfilter != null) {
					Handles.color = Color.magenta; 
					Vector3[] vertices = spline.meshfilter.sharedMesh.vertices;
					Vector2[] uv4 = spline.meshfilter.sharedMesh.uv4;
					Vector3[] normals = spline.meshfilter.sharedMesh.normals;
					Quaternion up = Quaternion.Euler (90, 0, 0);
					for (int i = 0; i < vertices.Length; i += 5) {
						Vector3 item = vertices [i];
						Vector3 handlePos = spline.transform.TransformPoint (item);
						//Handles.CubeHandleCap (0, spline.transform.TransformPoint (item), Quaternion.identity, 0.05f, EventType.Repaint);
						if (spline.drawOnMesh)
							Handles.RectangleHandleCap (0, handlePos, up, 0.05f, EventType.Repaint);

						//float angle = Mathf.Acos (uv4 [i].y) * Mathf.Sign (uv4 [i].x);
						//Handles.ArrowHandleCap (0, handlePos, Quaternion.LookRotation (normals [i]) * up * Quaternion.Euler (0, angle * Mathf.Rad2Deg, 0), HandleUtility.GetHandleSize (handlePos) * 0.2f, EventType.Repaint);
					}

				}
				if (spline.drawOnMesh)
					DrawOnVertexColors ();
				else
					DrawOnFlowMap ();
				return;
			}

			if (Event.current.commandName == "UndoRedoPerformed") {

				spline.GenerateSpline ();
				return;
			}

			if (selectedPosition >= 0 && selectedPosition < spline.controlPoints.Count) {
				Handles.color = Color.red;
				Handles.SphereHandleCap (0, (Vector3)spline.controlPoints [selectedPosition] + spline.transform.position, Quaternion.identity, 1, EventType.Repaint);

			}

			if (spline.debug) {
				Vector3[] points = new Vector3[spline.controlPoints.Count];


				for (int i = 0; i < spline.controlPoints.Count; i++) {
					points [i] = (Vector3)spline.controlPoints [i] + spline.transform.position;
				}


				Handles.color = Color.white;    
				Handles.DrawPolyLine (points);

				Handles.color = new Color (1, 0, 0, 0.5f);   

				for (int i = 0; i < spline.pointsDown.Count; i++) {

					Vector3 handlePos = (Vector3)spline.pointsDown [i] + spline.transform.position;
					Vector3 handlePos2 = (Vector3)spline.pointsUp [i] + spline.transform.position;
					Handles.DrawLine (handlePos, handlePos2);
				}

				Handles.color = Color.green;    
				foreach (var item in spline.controlPointsDown) {
					Handles.SphereHandleCap (0, item + spline.transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
				}

				points = new Vector3[spline.pointsDown.Count];


				for (int i = 0; i < spline.pointsDown.Count; i++) {
					points [i] = (Vector3)spline.pointsDown [i] + spline.transform.position;
				}
			  
				Handles.DrawPolyLine (points);



				Handles.color = Color.blue;    
				foreach (var item in spline.controlPointsUp) {
					Handles.SphereHandleCap (0, item + spline.transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
				}

				points = new Vector3[spline.pointsUp.Count];

				for (int i = 0; i < spline.pointsUp.Count; i++) {
					points [i] = (Vector3)spline.pointsUp [i] + spline.transform.position;
				}

				Handles.DrawPolyLine (points);




				Handles.color = Color.red;    

				points = spline.points.ToArray ();
				for (int i = 0; i < points.Length; i++) {



					points [i] += spline.transform.position;
					Handles.DrawLine (points [i], points [i] + spline.normalsList [i]);
				}
				Handles.DrawPolyLine (points);
			}
		

			for (int j = 0; j < spline.controlPoints.Count; j++) {

			

				EditorGUI.BeginChangeCheck ();



				Vector3 handlePos = (Vector3)spline.controlPoints [j] + spline.transform.position;




				GUIStyle style = new GUIStyle ();
				style.normal.textColor = Color.red; 

				Vector3 screenPoint = Camera.current.WorldToScreenPoint (handlePos);

				if (screenPoint.z > 0) { 

					Handles.Label (handlePos + Vector3.up * HandleUtility.GetHandleSize (handlePos), "Point: " + j.ToString (), style);

				}

				float width = spline.controlPoints [j].w;
				if (Tools.current == Tool.Move) {
					
					float size = 0.6f;
					size = HandleUtility.GetHandleSize (handlePos) * size;

					Handles.color = Handles.xAxisColor;   
					Vector4 pos = Handles.Slider ((Vector3)spline.controlPoints [j] + spline.transform.position, Vector3.right, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;
					Handles.color = Handles.yAxisColor;   
					pos = Handles.Slider ((Vector3)pos + spline.transform.position, Vector3.up, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;
					Handles.color = Handles.zAxisColor;  
					pos = Handles.Slider ((Vector3)pos + spline.transform.position, Vector3.forward, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;

					Vector3 halfPos = (Vector3.right + Vector3.forward) * size * 0.3f;
					Handles.color = Handles.yAxisColor;
					pos = Handles.Slider2D ((Vector3)pos + spline.transform.position + halfPos, Vector3.up, Vector3.right, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
					halfPos = (Vector3.right + Vector3.up) * size * 0.3f;
					Handles.color = Handles.zAxisColor;  
					pos = Handles.Slider2D ((Vector3)pos + spline.transform.position + halfPos, Vector3.forward, Vector3.right, Vector3.up, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
					halfPos = (Vector3.up + Vector3.forward) * size * 0.3f;
					Handles.color = Handles.xAxisColor;  
					pos = Handles.Slider2D ((Vector3)pos + spline.transform.position + halfPos, Vector3.right, Vector3.up, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
				
					pos.w = width;
					spline.controlPoints [j] = pos;


				} else if (Tools.current == Tool.Rotate) {
					
					if (spline.controlPointsRotations.Count > j && spline.controlPointsOrientation.Count > j) {

						if (!((spline.beginningSpline && j == 0) || (spline.endingSpline && j == spline.controlPoints.Count - 1))) {
							float size = 0.6f;
							size = HandleUtility.GetHandleSize (handlePos) * size;

							Handles.color = Handles.zAxisColor;    
							Quaternion rotation = Handles.Disc (spline.controlPointsOrientation [j], handlePos, spline.controlPointsOrientation [j] * new Vector3 (0, 0, 1), size, true, 0.1f);

							Handles.color = Handles.yAxisColor;   
							rotation = Handles.Disc (rotation, handlePos, rotation * new Vector3 (0, 1, 0), size, true, 0.1f);

							Handles.color = Handles.xAxisColor;   
							rotation = Handles.Disc (rotation, handlePos, rotation * new Vector3 (1, 0, 0), size, true, 0.1f);
						


							spline.controlPointsRotations [j] *= (Quaternion.Inverse (spline.controlPointsOrientation [j]) * rotation);

							if (float.IsNaN (spline.controlPointsRotations [j].x) || float.IsNaN (spline.controlPointsRotations [j].y) || float.IsNaN (spline.controlPointsRotations [j].z) || float.IsNaN (spline.controlPointsRotations [j].w)) {
								spline.controlPointsRotations [j] = Quaternion.identity;
								spline.GenerateSpline ();
							}
							Handles.color = baseColor;   
							Handles.FreeRotateHandle (Quaternion.identity, handlePos, size);

							Handles.CubeHandleCap (0, handlePos, spline.controlPointsOrientation [j], size * 0.3f, EventType.Repaint);

							Handles.DrawLine (spline.controlPointsUp [j] + spline.transform.position, spline.controlPointsDown [j] + spline.transform.position);
						} 
					

					}

				} else if (Tools.current == Tool.Scale) {
					
					Handles.color = Handles.xAxisColor;     
					//Vector3 handlePos = (Vector3)spline.controlPoints [j] + spline.transform.position;
					      
					width = Handles.ScaleSlider (spline.controlPoints [j].w, (Vector3)spline.controlPoints [j] + spline.transform.position, new Vector3 (0, 0.5f, 0), 
						Quaternion.Euler (-90, 0, 0), HandleUtility.GetHandleSize (handlePos), 0.01f);
					
					Vector4 pos = spline.controlPoints [j];
					pos.w = width;
					spline.controlPoints [j] = pos;

				}

			
				
				if (EditorGUI.EndChangeCheck ()) {
					
					CheckRotations ();
					Undo.RecordObject (spline, "Change Position");
					spline.GenerateSpline ();

				}

			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control) {
				

				Vector3 screenPosition = Event.current.mousePosition;
				screenPosition.y = Camera.current.pixelHeight - screenPosition.y;
				Ray ray = Camera.current.ScreenPointToRay (screenPosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit)) {
					Undo.RecordObject (spline, "Add point");

					Vector4 position = hit.point - spline.transform.position;
					if (spline.controlPoints.Count > 0)
						position.w = spline.controlPoints [spline.controlPoints.Count - 1].w;
					else
						position.w = spline.width;


					spline.controlPointsRotations.Add (Quaternion.identity);
					spline.controlPoints.Add (position);
					spline.controlPointsSnap.Add (0);
					spline.controlPointsMeshCurves.Add (new AnimationCurve (new Keyframe[]{ new Keyframe (0, 0), new Keyframe (1, 0) }));

					spline.GenerateSpline ();

					GUIUtility.hotControl = controlId;
					Event.current.Use ();
					HandleUtility.Repaint ();
				}
			}
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.control) {
				GUIUtility.hotControl = 0;

			}


		}


	}

	void DrawOnVertexColors ()
	{

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

	
	
		Camera sceneCamera = SceneView.lastActiveSceneView.camera;
		Vector2 mousePos = Event.current.mousePosition;
		mousePos.y = Screen.height - mousePos.y - 40;
		Ray ray = sceneCamera.ScreenPointToRay (mousePos);
		MeshCollider meshCollider = spline.gameObject.AddComponent<MeshCollider> ();
		RaycastHit[] hits = Physics.RaycastAll (ray, Mathf.Infinity);

		GameObject go = null;
		Vector3 hitPosition = Vector3.zero;
		Vector3 hitNormal = Vector3.zero;
		if (hits.Length > 0) {

			foreach (var hit in hits) {
				if (hit.collider is MeshCollider) {
					go = hit.collider.gameObject;
					if (go == spline.gameObject) {
						
						hitPosition = hit.point;
						hitNormal = hit.normal;
						break;
					} else
						go = null;
				}
			}
		
		}

		DestroyImmediate (meshCollider);


		if (go != null) {
			
			Handles.color = new Color (spline.drawColor.r, spline.drawColor.g, spline.drawColor.b, 1);
			Handles.DrawLine (hitPosition, hitPosition + hitNormal * 2);
			Handles.CircleHandleCap (
				0,
				hitPosition,
				Quaternion.LookRotation (hitNormal),
				spline.drawSize,
				EventType.Repaint
			);
			Handles.color = Color.black;
			Handles.CircleHandleCap (
				0,
				hitPosition,
				Quaternion.LookRotation (hitNormal),
				spline.drawSize - 0.1f,
				EventType.Repaint
			);

			if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
				return;
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
			}

			MeshFilter meshFilter = spline.GetComponent<MeshFilter> ();
			if (meshFilter.sharedMesh != null) {
				Mesh mesh = meshFilter.sharedMesh;
				if (spline.colors.Length == 0)
					spline.colors = new Color[mesh.vertices.Length];

				int length = mesh.vertices.Length;
				float dist = 0;
				hitPosition -= spline.transform.position;
				Vector3[] vertices = mesh.vertices;
				Color[] colors = spline.colors;

				for (int i = 0; i < length; i++) {
					dist = Vector3.Distance (hitPosition, vertices [i]);

					if (dist < spline.drawSize) {
						
						if (Event.current.shift)
							colors [i] = Color.Lerp (colors [i], Color.white, spline.opacity);
						else
							colors [i] = Color.Lerp (colors [i], spline.drawColor, spline.opacity);
						mesh.colors = colors;
						meshFilter.sharedMesh = mesh;
					}
				}

			
			}
		}
	}

	void DrawOnFlowMap ()
	{

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));



		Camera sceneCamera = SceneView.lastActiveSceneView.camera;
		Vector2 mousePos = Event.current.mousePosition;
		mousePos.y = Screen.height - mousePos.y - 40;
		Ray ray = sceneCamera.ScreenPointToRay (mousePos);
		MeshCollider meshCollider = spline.gameObject.AddComponent<MeshCollider> ();
		RaycastHit[] hits = Physics.RaycastAll (ray, Mathf.Infinity);

		GameObject go = null;
		Vector3 hitPosition = Vector3.zero;
		Vector3 hitNormal = Vector3.zero;
		if (hits.Length > 0) {

			foreach (var hit in hits) {
				if (hit.collider is MeshCollider) {
					go = hit.collider.gameObject;
					if (go == spline.gameObject) {

						hitPosition = hit.point;
						hitNormal = hit.normal;
						break;
					} else
						go = null;
				}
			}

		}

		DestroyImmediate (meshCollider);


		if (go != null) {

			Handles.color = new Color (spline.flowDirection, spline.flowSpeed, 0, 1);
			Handles.DrawLine (hitPosition, hitPosition + hitNormal * 2);
			Handles.CircleHandleCap (
				0,
				hitPosition,
				Quaternion.LookRotation (hitNormal),
				spline.drawSize,
				EventType.Repaint
			);
			Handles.color = Color.black;
			Handles.CircleHandleCap (
				0,
				hitPosition,
				Quaternion.LookRotation (hitNormal),
				spline.drawSize - 0.1f,
				EventType.Repaint
			);

			if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
				return;
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
			}

			spline.overrideFlowMap = true;
			MeshFilter meshFilter = spline.GetComponent<MeshFilter> ();

		
			if (meshFilter.sharedMesh != null) {
				Mesh mesh = meshFilter.sharedMesh;



				List<Vector2> colorsFlowMap = spline.colorsFlowMap;
				int length = mesh.vertices.Length;
				float dist = 0;
				float distValue = 0;
				hitPosition -= spline.transform.position;
				Vector3[] vertices = mesh.vertices;

				for (int i = 0; i < length; i++) {
					dist = Vector3.Distance (hitPosition, vertices [i]);
					if (dist < spline.drawSize) {
						distValue = (spline.drawSize - dist) / (float)spline.drawSize;
						if (Event.current.shift) {
							colorsFlowMap [i] = Vector2.Lerp (colorsFlowMap [i], new Vector2 (0, 0), spline.opacity);

						} else {
							colorsFlowMap [i] = Vector2.Lerp (colorsFlowMap [i], new Vector2 (spline.flowDirection, spline.flowSpeed), spline.opacity * distValue);
					
						}

					}
				}

				mesh.uv4 = colorsFlowMap.ToArray ();
				spline.colorsFlowMap = colorsFlowMap;
				meshFilter.sharedMesh = mesh;

			}
		}
	}

	public static LayerMask LayerMaskField (string label, LayerMask selected, bool showSpecial)
	{

		List<string> layers = new List<string> ();
		List<int> layerNumbers = new List<int> ();

		string selectedLayers = "";

		for (int i = 0; i < 32; i++) {
			string layerName = LayerMask.LayerToName (i);
			if (layerName != "") {
				if (selected == (selected | (1 << i))) {
					if (selectedLayers == "") {
						selectedLayers = layerName;
					} else {
						selectedLayers = "Mixed";
					}
				}
			}
		}

		EventType lastEvent = Event.current.type;

		if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand) {
			if (selected.value == 0) {
				layers.Add ("Nothing");
			} else if (selected.value == -1) {
				layers.Add ("Everything");
			} else {
				layers.Add (selectedLayers);
			}
			layerNumbers.Add (-1);
		}

		if (showSpecial) {
			layers.Add ((selected.value == 0 ? "[X] " : "      ") + "Nothing");
			layerNumbers.Add (-2);

			layers.Add ((selected.value == -1 ? "[X] " : "      ") + "Everything");
			layerNumbers.Add (-3);
		}

		for (int i = 0; i < 32; i++) {

			string layerName = LayerMask.LayerToName (i);

			if (layerName != "") {
				if (selected == (selected | (1 << i))) {
					layers.Add ("[X] " + i + ": " + layerName);
				} else {
					layers.Add ("     " + i + ": " + layerName);
				}
				layerNumbers.Add (i);
			}
		}

		bool preChange = GUI.changed;

		GUI.changed = false;

		int newSelected = 0;

		if (Event.current.type == EventType.MouseDown) {
			newSelected = -1;
		}

		newSelected = EditorGUILayout.Popup (label, newSelected, layers.ToArray (), EditorStyles.layerMaskField);

		if (GUI.changed && newSelected >= 0) {
			if (showSpecial && newSelected == 0) {
				selected = 0;
			} else if (showSpecial && newSelected == 1) {
				selected = -1;
			} else {

				if (selected == (selected | (1 << layerNumbers [newSelected]))) {
					selected &= ~(1 << layerNumbers [newSelected]);
				} else {
					selected = selected | (1 << layerNumbers [newSelected]);
				}
			}
		} else {
			GUI.changed = preChange;
		}

		return selected;
	}

}
