using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof(MeshFilter))]
public class RamSpline : MonoBehaviour
{
	public SplineProfile currentProfile;
	public SplineProfile oldProfile;
	
	public List<RamSpline> beginnigChildSplines = new List<RamSpline> ();
	public List<RamSpline> endingChildSplines = new List<RamSpline> ();
	public RamSpline beginningSpline;
	public RamSpline endingSpline;
	public int beginningConnectionID;
	public int endingConnectionID;
	public float beginningMinWidth = 0.5f;
	public float beginningMaxWidth = 1f;
	public float endingMinWidth = 0.5f;
	public float endingMaxWidth = 1f;

	public int toolbarInt = 0;

	public bool invertUVDirection = false;
	public bool uvRotation = true;

	public MeshFilter meshfilter;
	public List<Vector4> controlPoints = new List<Vector4> ();

	public List<Quaternion> controlPointsRotations = new List<Quaternion> ();

	public List<Quaternion> controlPointsOrientation = new List<Quaternion> ();

	public List<Vector3> controlPointsUp = new List<Vector3> ();
	public List<Vector3> controlPointsDown = new List<Vector3> ();
	public List<float> controlPointsSnap = new List<float> ();

	public AnimationCurve meshCurve = new AnimationCurve (new Keyframe[]{ new Keyframe (0, 0), new Keyframe (1, 0) });
	public List<AnimationCurve> controlPointsMeshCurves = new List<AnimationCurve> ();


	public AnimationCurve terrainCurve = new AnimationCurve (new Keyframe[]{ new Keyframe (0, 0), new Keyframe (1, 0) });
	public int detailTerrain = 100;
	public int detailTerrainForward = 100;


	public bool normalFromRaycast = false;
	public bool snapToTerrain = false;
	public LayerMask snapMask = 1;

	public List<Vector3> points = new List<Vector3> ();

	public List<Vector3> pointsUp = new List<Vector3> ();
	public List<Vector3> pointsDown = new List<Vector3> ();



	public List<Vector3> points2 = new List<Vector3> ();


	public List<Vector3> verticesBeginning = new List<Vector3> ();
	public List<Vector3> verticesEnding = new List<Vector3> ();

	public List<Vector3> normalsBeginning = new List<Vector3> ();
	public List<Vector3> normalsEnding = new List<Vector3> ();

	public List<float> widths = new List<float> ();
	public List<float> snaps = new List<float> ();
	public List<float> lerpValues = new List<float> ();
	public List<Quaternion> orientations = new List<Quaternion> ();
	public List<Vector3> tangents = new List<Vector3> ();
	public List<Vector3> normalsList = new List<Vector3> ();
	public Color[] colors;
	public List<Vector2> colorsFlowMap = new List<Vector2> ();

	public float minVal = 0.5f;
	public float maxVal = 0.5f;

	public float width = 4;

	public int vertsInShape = 3;
	public float traingleDensity = 0.2f;
	public float uvScale = 3;

	public Material oldMaterial;
	public bool showVertexColors;
	public bool showFlowMap;
	public bool overrideFlowMap = false;

	public bool drawOnMesh = false;
	public bool drawOnMeshFlowMap = false;
	public bool uvScaleOverride = false;
	public bool debug = false;

	public Color drawColor = Color.black;

	public float flowSpeed = 1f;
	public float flowDirection = 0f;
	public AnimationCurve flowFlat = new AnimationCurve (new Keyframe[] {
		new Keyframe (0, 0.025f),
		new Keyframe (0.5f, 0.05f),
		new Keyframe (1, 0.025f)
	});
	public AnimationCurve flowWaterfall = new AnimationCurve (new Keyframe[] {
		new Keyframe (0, 0.25f),
		new Keyframe (1, 0.25f)
	});

	public float opacity = 0.1f;
	public float drawSize = 1f;

	public float length = 0;
	public float fulllength = 0;

	public float minMaxWidth;
	public float uvWidth;

	public float uvBeginning;

	public void Start ()
	{
		GenerateSpline ();

	}

	public void GenerateBeginningParentBased ()
	{

		vertsInShape = (int)Mathf.Round ((beginningSpline.vertsInShape - 1) * (beginningMaxWidth - beginningMinWidth) + 1);
		if (vertsInShape < 1)
			vertsInShape = 1;

		beginningConnectionID = beginningSpline.points.Count - 1;
		Vector4 pos = beginningSpline.controlPoints [beginningSpline.controlPoints.Count - 1];
		float width = pos.w;
		width *= beginningMaxWidth - beginningMinWidth;
		pos = Vector3.Lerp (beginningSpline.pointsDown [beginningConnectionID], beginningSpline.pointsUp [beginningConnectionID], beginningMinWidth + (beginningMaxWidth - beginningMinWidth) * 0.5f)
		+ beginningSpline.transform.position - transform.position;
		pos.w = width;
		controlPoints [0] = pos;

		if (!uvScaleOverride)
			uvScale = beginningSpline.uvScale;
	}

	public void GenerateEndingParentBased ()
	{

		if (beginningSpline == null) {
			vertsInShape = (int)Mathf.Round ((endingSpline.vertsInShape - 1) * (endingMaxWidth - endingMinWidth) + 1);
			if (vertsInShape < 1)
				vertsInShape = 1;
		}
		
		endingConnectionID = 0;
		Vector4 pos = endingSpline.controlPoints [0];
		float width = pos.w;
		width *= endingMaxWidth - endingMinWidth;
		pos = Vector3.Lerp (endingSpline.pointsDown [endingConnectionID], endingSpline.pointsUp [endingConnectionID], endingMinWidth + (endingMaxWidth - endingMinWidth) * 0.5f) + endingSpline.transform.position - transform.position;
		pos.w = width;
		controlPoints [controlPoints.Count - 1] = pos;
	}


	public void GenerateSpline (List<RamSpline> generatedSplines = null)
	{
		generatedSplines = new List<RamSpline> ();

		if (beginningSpline) {
			GenerateBeginningParentBased ();
		}
		if (endingSpline) {
			GenerateEndingParentBased ();
		}

	
		

		List<Vector4> pointsChecked = new List<Vector4> ();
		for (int i = 0; i < controlPoints.Count; i++) {
			if (i > 0) {
				if (Vector3.Distance ((Vector3)controlPoints [i], (Vector3)controlPoints [i - 1]) > 0)
					pointsChecked.Add (controlPoints [i]);

			} else
				pointsChecked.Add (controlPoints [i]);
		}

		Mesh mesh = new Mesh ();
		meshfilter = GetComponent<MeshFilter> ();
		if (pointsChecked.Count < 2) {
			mesh.Clear ();
		
			meshfilter.mesh = mesh;
			return;

		}	

		controlPointsOrientation = new List<Quaternion> ();
		lerpValues.Clear ();
		snaps.Clear ();
		points.Clear ();
		pointsUp.Clear ();
		pointsDown.Clear ();
		orientations.Clear ();
		tangents.Clear ();
		normalsList.Clear ();
		widths.Clear ();
		controlPointsUp.Clear ();
		controlPointsDown.Clear ();
		verticesBeginning.Clear ();
		verticesEnding.Clear ();
		normalsBeginning.Clear ();
		normalsEnding.Clear ();

		if (beginningSpline != null && beginningSpline.controlPointsRotations.Count > 0)
			controlPointsRotations [0] = Quaternion.identity;
		if (endingSpline != null && endingSpline.controlPointsRotations.Count > 0)
			controlPointsRotations [controlPointsRotations.Count - 1] = Quaternion.identity;

		for (int i = 0; i < pointsChecked.Count; i++) {

			if (i > pointsChecked.Count - 2) {
				continue;
			}

			CalculateCatmullRomSideSplines (pointsChecked, i);
		}

		if (beginningSpline != null && beginningSpline.controlPointsRotations.Count > 0)
			controlPointsRotations [0] = Quaternion.Inverse (controlPointsOrientation [0]) * (beginningSpline.controlPointsOrientation [beginningSpline.controlPointsOrientation.Count - 1]);

		if (endingSpline != null && endingSpline.controlPointsRotations.Count > 0)
			controlPointsRotations [controlPointsRotations.Count - 1] = Quaternion.Inverse (controlPointsOrientation [controlPointsOrientation.Count - 1]) * (endingSpline.controlPointsOrientation [0]);// * endingSpline.controlPointsRotations [0]);

		controlPointsOrientation = new List<Quaternion> ();
		controlPointsUp.Clear ();
		controlPointsDown.Clear ();


		for (int i = 0; i < pointsChecked.Count; i++) {

			if (i > pointsChecked.Count - 2) {
				continue;
			}

			CalculateCatmullRomSideSplines (pointsChecked, i);
		}



					
		for (int i = 0; i < pointsChecked.Count; i++) {
			
			if (i > pointsChecked.Count - 2) {
				continue;
			}

			CalculateCatmullRomSplineParameters (pointsChecked, i);
		}

		for (int i = 0; i < controlPointsUp.Count; i++) {

			if (i > controlPointsUp.Count - 2) {
				continue;
			}

			CalculateCatmullRomSpline (controlPointsUp, i, ref pointsUp);
		}
		for (int i = 0; i < controlPointsDown.Count; i++) {

			if (i > controlPointsDown.Count - 2) {
				continue;
			}

			CalculateCatmullRomSpline (controlPointsDown, i, ref  pointsDown);
		}

		GenerateMesh (ref mesh);

		if (generatedSplines != null) {
			
			generatedSplines.Add (this);
			foreach (var item in beginnigChildSplines) {
				if (item != null && !generatedSplines.Contains (item)) {
					if (item.beginningSpline == this || item.endingSpline == this) {
						item.GenerateSpline (generatedSplines);
					}
				}
			}

			foreach (var item in endingChildSplines) {
				if (item != null && !generatedSplines.Contains (item)) {
					if (item.beginningSpline == this || item.endingSpline == this) {
						item.GenerateSpline (generatedSplines);
					}
				}
			}
		}
	}


	void CalculateCatmullRomSideSplines (List<Vector4> controlPoints, int pos)
	{
		Vector3 p0 = controlPoints [pos];
		Vector3 p1 = controlPoints [pos];
		Vector3 p2 = controlPoints [ClampListPos (pos + 1)];
		Vector3 p3 = controlPoints [ClampListPos (pos + 1)];

		if (pos > 0)
			p0 = controlPoints [ClampListPos (pos - 1)];

		if (pos < controlPoints.Count - 2)
			p3 = controlPoints [ClampListPos (pos + 2)];


		int tValueMax = 0;
		if (pos == controlPoints.Count - 2) {
			tValueMax = 1;
		}

		for (int tValue = 0; tValue <= tValueMax; tValue++) {			
		
			Vector3 newPos = GetCatmullRomPosition (tValue, p0, p1, p2, p3);
			Vector3 tangent = GetCatmullRomTangent (tValue, p0, p1, p2, p3).normalized;
			Vector3 normal = CalculateNormal (tangent, Vector3.up).normalized;

			Quaternion orientation;
			if (normal == tangent && normal == Vector3.zero)
				orientation = Quaternion.identity;
			else
				orientation = Quaternion.LookRotation (tangent, normal);

			orientation *= Quaternion.Lerp (controlPointsRotations [pos], controlPointsRotations [ClampListPos (pos + 1)], tValue);

//			if (beginningSpline && pos == 0) {
//				
//				int lastId = beginningSpline.controlPointsOrientation.Count - 1;
//				//orientation = beginningSpline.controlPointsOrientation [lastId];
//
//			} 
//		
//			if (endingSpline && pos == controlPoints.Count - 2 && tValue == 1) {
//				
//				//orientation = endingSpline.controlPointsOrientation [0];
//
//			}

			controlPointsOrientation.Add (orientation);

			Vector3 posUp = newPos + orientation * (0.5f * controlPoints [pos + tValue].w * Vector3.right);
			Vector3 posDown = newPos + orientation * (0.5f * controlPoints [pos + tValue].w * Vector3.left);

			controlPointsUp.Add (posUp);
			controlPointsDown.Add (posDown);
		}

	}


	void CalculateCatmullRomSplineParameters (List<Vector4> controlPoints, int pos, bool initialPoints = false)
	{
		

		Vector3 p0 = controlPoints [pos];
		Vector3 p1 = controlPoints [pos];
		Vector3 p2 = controlPoints [ClampListPos (pos + 1)];
		Vector3 p3 = controlPoints [ClampListPos (pos + 1)];

		if (pos > 0)
			p0 = controlPoints [ClampListPos (pos - 1)];
	
		if (pos < controlPoints.Count - 2)
			p3 = controlPoints [ClampListPos (pos + 2)];
	

		int loops = Mathf.FloorToInt (1f / traingleDensity);

		float i = 1;

		float start = 0;
		if (pos > 0)
			start = 1;	

		for (i = start; i <= loops; i++) {
			float t = i * traingleDensity;
			CalculatePointParameters (controlPoints, pos, p0, p1, p2, p3, t);
		}

		if (i < loops) {
			i = loops;
			float t = i * traingleDensity;
			CalculatePointParameters (controlPoints, pos, p0, p1, p2, p3, t);
		}

	}

	void CalculateCatmullRomSpline (List<Vector3> controlPoints, int pos, ref List<Vector3> points)
	{


		Vector3 p0 = controlPoints [pos];
		Vector3 p1 = controlPoints [pos];
		Vector3 p2 = controlPoints [ClampListPos (pos + 1)];
		Vector3 p3 = controlPoints [ClampListPos (pos + 1)];

		if (pos > 0)
			p0 = controlPoints [ClampListPos (pos - 1)];

		if (pos < controlPoints.Count - 2)
			p3 = controlPoints [ClampListPos (pos + 2)];

		int loops = Mathf.FloorToInt (1f / traingleDensity);

		float i = 1;

		float start = 0;
		if (pos > 0)
			start = 1;	

		for (i = start; i <= loops; i++) {
			float t = i * traingleDensity;
			CalculatePointPosition (controlPoints, pos, p0, p1, p2, p3, t, ref points);
		}

		if (i < loops) {
			i = loops;
			float t = i * traingleDensity;
			CalculatePointPosition (controlPoints, pos, p0, p1, p2, p3, t, ref points);
		}

	}

	void CalculatePointPosition (List<Vector3> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, ref List<Vector3> points)
	{

		Vector3 newPos = GetCatmullRomPosition (t, p0, p1, p2, p3);
		points.Add (newPos);

		Vector3 tangent = GetCatmullRomTangent (t, p0, p1, p2, p3).normalized;
		Vector3 normal = CalculateNormal (tangent, Vector3.up).normalized;

	}

	void CalculatePointParameters (List<Vector4> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{		
		
		Vector3 newPos = GetCatmullRomPosition (t, p0, p1, p2, p3);



		widths.Add (Mathf.Lerp (controlPoints [pos].w, controlPoints [ClampListPos (pos + 1)].w, t));

		if (controlPointsSnap.Count > pos + 1)
			snaps.Add (Mathf.Lerp (controlPointsSnap [pos], controlPointsSnap [ClampListPos (pos + 1)], t));
		else
			snaps.Add (0);

		lerpValues.Add (pos + t);


		points.Add (newPos);

		Vector3 tangent = GetCatmullRomTangent (t, p0, p1, p2, p3).normalized;
		Vector3 normal = CalculateNormal (tangent, Vector3.up).normalized;

		Quaternion orientation;
		if (normal == tangent && normal == Vector3.zero)
			orientation = Quaternion.identity;
		else
			orientation = Quaternion.LookRotation (tangent, normal);
		
		orientation *= Quaternion.Lerp (controlPointsRotations [pos], controlPointsRotations [ClampListPos (pos + 1)], t);
		orientations.Add (orientation);

		tangents.Add (tangent);
		if (normalsList.Count > 0 && Vector3.Angle (normalsList [normalsList.Count - 1], normal) > 90) {
			normal *= -1;
		}

		normalsList.Add (normal);
	
	}

	int ClampListPos (int pos)
	{
		if (pos < 0) {
			pos = controlPoints.Count - 1;
		}

		if (pos > controlPoints.Count) {
			pos = 1;
		} else if (pos > controlPoints.Count - 1) {
			pos = 0;
		}

		return pos;
	}

	Vector3 GetCatmullRomPosition (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

		Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}

	Vector3 GetCatmullRomTangent (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{		
		return  0.5f * ((-p0 + p2) + 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t + 3f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t);
	}

	Vector3 CalculateNormal (Vector3 tangent, Vector3 up)
	{
		Vector3 binormal = Vector3.Cross (up, tangent);
		return Vector3.Cross (tangent, binormal);
	}


	void GenerateMesh (ref Mesh mesh)
	{
		int segments = points.Count - 1;
		int edgeLoops = points.Count;
		int vertCount = vertsInShape * edgeLoops;

		List<int> triangleIndices = new List<int> ();
		Vector3[] vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector2[] uvs = new Vector2[vertCount];
		Vector2[] uvs3 = new Vector2[vertCount];
		Vector2[] uvs4 = new Vector2[vertCount];

		if (colors == null || colors.Length != vertCount) {
			colors = new Color[vertCount];
			for (int i = 0; i < colors.Length; i++) {
				colors [i] = Color.black;
			}
		}

		if (colorsFlowMap.Count != vertCount)
			colorsFlowMap.Clear ();


		length = 0;
		fulllength = 0;

		if (beginningSpline != null)
			length = beginningSpline.length;

		minMaxWidth = 1;
		uvWidth = 1;
		uvBeginning = 0;

		if (beginningSpline != null) {

			minMaxWidth = beginningMaxWidth - beginningMinWidth;


			uvWidth = minMaxWidth * beginningSpline.uvWidth;

			uvBeginning = beginningSpline.uvWidth * beginningMinWidth + beginningSpline.uvBeginning;

		} else if (endingSpline != null) {
			
			minMaxWidth = endingMaxWidth - endingMinWidth;

			uvWidth = minMaxWidth * endingSpline.uvWidth;

			uvBeginning = endingSpline.uvWidth * endingMinWidth + endingSpline.uvBeginning;
		}
			


		for (int i = 0; i < pointsDown.Count; i++) {
			float width = widths [i];
			if (i > 0)
				fulllength += uvWidth * Vector3.Distance (pointsDown [i], pointsDown [i - 1]) / (float)(uvScale * width);
		}



		float roundEnding = Mathf.Round (fulllength);

		for (int i = 0; i < pointsDown.Count; i++) {
			
			float width = widths [i];

			int offset = i * vertsInShape;

			if (i > 0) {
				length += (uvWidth * Vector3.Distance (pointsDown [i], pointsDown [i - 1]) / (float)(uvScale * width)) / fulllength * roundEnding;
			}

			float u = 0;
			float u3 = 0;

	




			for (int j = 0; j < vertsInShape; j++) {
				int id = offset + j;

				//VERTICES
				float pos = j / (float)(vertsInShape - 1);

				if (pos < 0.5f)
					pos *= minVal * 2;
				else
					pos = ((pos - 0.5f) * (1 - maxVal) + 0.5f * maxVal) * 2;


						
				if (i == 0 && beginningSpline != null && beginningSpline.verticesEnding != null && beginningSpline.normalsEnding != null) {
					
				
					int pos2 = (int)(beginningSpline.vertsInShape * beginningMinWidth);

					vertices [id] = beginningSpline.verticesEnding [Mathf.Clamp (j + pos2, 0, beginningSpline.verticesEnding.Count - 1)] + beginningSpline.transform.position - transform.position;
					//if (beginningSpline.normalsEnding.Count > 0)
					//	normals [id] = beginningSpline.normalsEnding [Mathf.Clamp (j + pos2, 0, beginningSpline.normalsEnding.Count - 1)];

				} else if (i == pointsDown.Count - 1 && endingSpline != null && endingSpline.verticesBeginning != null && endingSpline.normalsBeginning != null) {

					int pos2 = (int)(endingSpline.vertsInShape * endingMinWidth);

					vertices [id] = endingSpline.verticesBeginning [Mathf.Clamp (j + pos2, 0, endingSpline.verticesBeginning.Count - 1)] + endingSpline.transform.position - transform.position;
					//if (endingSpline.normalsBeginning.Count > 0)
					//	normals [id] = endingSpline.normalsBeginning [Mathf.Clamp (j + pos2, 0, endingSpline.normalsBeginning.Count - 1)];
					

				} else {
					vertices [id] = Vector3.Lerp (pointsDown [i], pointsUp [i], pos);
				
					RaycastHit hit;
					if (Physics.Raycast (vertices [id] + transform.position + Vector3.up * 5, Vector3.down, out hit, 1000, snapMask.value)) {
						
						vertices [id] = Vector3.Lerp (vertices [id], hit.point - transform.position + new Vector3 (0, 0.1f, 0), (Mathf.Sin (Mathf.PI * snaps [i] - Mathf.PI * 0.5f) + 1) * 0.5f);

					}


					if (normalFromRaycast) {
						RaycastHit hit2;
						if (Physics.Raycast (points [i] + transform.position + Vector3.up * 5, Vector3.down, out hit2, 1000, snapMask.value)) {
							normals [id] = hit2.normal;
						}
					}

									
					vertices [id].y += Mathf.Lerp (controlPointsMeshCurves [Mathf.FloorToInt (lerpValues [i])].Evaluate (pos),
						controlPointsMeshCurves [Mathf.CeilToInt (lerpValues [i])].Evaluate (pos),
						lerpValues [i] - Mathf.Floor (lerpValues [i]));
								
				}

				if (i > 0 && i < 5 && beginningSpline != null && beginningSpline.verticesEnding != null) {
					vertices [id].y = (vertices [id].y + vertices [id - vertsInShape].y) * 0.5f;
				}

				if (i == pointsDown.Count - 1 && endingSpline != null && endingSpline.verticesBeginning != null) {
					for (int k = 1; k < 5; k++) {
						vertices [id - vertsInShape * k].y = (vertices [id - vertsInShape * (k - 1)].y + vertices [id - vertsInShape * k].y) * 0.5f;
					}

				}


				if (i == 0)
					verticesBeginning.Add (vertices [id]);
				
				if (i == pointsDown.Count - 1)
					verticesEnding.Add (vertices [id]);
				
				
				//NORMALS

			

				if (!normalFromRaycast)
//				if ((i > 0 || beginningSpline == null) && (i < pointsDown.Count - 1 || endingSpline == null))
					normals [id] = orientations [i] * Vector3.up;
//				
//				if (beginningSpline != null && i == 1)
//					normals [id] = (normals [id] + normals [id - vertsInShape]) * 0.5f;
//
//				if (i == pointsDown.Count - 2 && endingSpline != null && endingSpline.normalsBeginning != null && endingSpline.normalsBeginning.Count > 0) {
//
//					int pos2 = (int)(endingSpline.vertsInShape * endingMinWidth);
//					normals [id] = (normals [id] + endingSpline.normalsBeginning [Mathf.Clamp (j + pos2, 0, endingSpline.normalsBeginning.Count - 1)]) * 0.5f;
//
//				}


			

				if (i == 0)
					normalsBeginning.Add (normals [id]);

				if (i == pointsDown.Count - 1)
					normalsEnding.Add (normals [id]);
			


				//UVS
				if (j > 0) {
					u = (pos) * uvWidth;
					u3 = pos;
				} 



				if (beginningSpline != null || endingSpline != null) {
					u += uvBeginning;
				}
				u = u / uvScale;



				float uv4u = FlowCalculate (u3, normals [id].y);
			


				int lerpDistance = 10;

				if (beginnigChildSplines.Count > 0 && i <= lerpDistance) {

					float lerpUv4u = 0;
					foreach (var item in beginnigChildSplines) {
						if (Mathf.CeilToInt (item.endingMaxWidth * (vertsInShape - 1)) >= j && j >= Mathf.CeilToInt (item.endingMinWidth * (vertsInShape - 1))) {		

							lerpUv4u = (j - Mathf.CeilToInt (item.endingMinWidth * (vertsInShape - 1)))
							/ (float)(Mathf.CeilToInt (item.endingMaxWidth * (vertsInShape - 1)) - Mathf.CeilToInt (item.endingMinWidth * (vertsInShape - 1)));
							
							lerpUv4u = FlowCalculate (lerpUv4u, normals [id].y);

						}
					}
					if (i > 0)
						uv4u =	Mathf.Lerp (uv4u, lerpUv4u, 1 - (i / (float)lerpDistance));
					else
						uv4u =	lerpUv4u;

				}


				if (i >= pointsDown.Count - lerpDistance - 1 && endingChildSplines.Count > 0) {

					float lerpUv4u = 0;

					foreach (var item in endingChildSplines) {		
						
						if (Mathf.CeilToInt (item.beginningMaxWidth * (vertsInShape - 1)) >= j && j >= Mathf.CeilToInt (item.beginningMinWidth * (vertsInShape - 1))) {		

							lerpUv4u = (j - Mathf.CeilToInt (item.beginningMinWidth * (vertsInShape - 1)))
							/ (float)(Mathf.CeilToInt (item.beginningMaxWidth * (vertsInShape - 1)) - Mathf.CeilToInt (item.beginningMinWidth * (vertsInShape - 1)));

							lerpUv4u = FlowCalculate (lerpUv4u, normals [id].y);

						}

					}
					if (i < pointsDown.Count - 1)
						uv4u =	Mathf.Lerp (uv4u, lerpUv4u, (i - (pointsDown.Count - lerpDistance - 1)) / (float)lerpDistance);
					else
						uv4u = lerpUv4u;

				}

				float uv4v = -(u3 - 0.5f) * 0.01f;




				if (uvRotation) {

					if (!invertUVDirection) {

						uvs [id] = new Vector2 (1 - length, u);
						uvs3 [id] = new Vector2 (1 - length / (float)fulllength, u3);
						uvs4 [id] = new Vector2 (uv4u, uv4v);

					} else {

						uvs [id] = new Vector2 (1 + length, u);
						uvs3 [id] = new Vector2 (1 + length / (float)fulllength, u3);				
						uvs4 [id] = new Vector2 (uv4u, uv4v);

					}
				} else {
					if (!invertUVDirection) {

						uvs [id] = new Vector2 (u, 1 - length);
						uvs3 [id] = new Vector2 (u3, 1 - length / (float)fulllength);
						uvs4 [id] = new Vector2 (uv4v, uv4u);
					} else {

						uvs [id] = new Vector2 (u, 1 + length);
						uvs3 [id] = new Vector2 (u3, 1 + length / (float)fulllength);
						uvs4 [id] = new Vector2 (uv4v, uv4u);

					}
				}

				if (colorsFlowMap.Count <= id)
					colorsFlowMap.Add (uvs4 [id]);
				else if (!overrideFlowMap)
					colorsFlowMap [id] = uvs4 [id];
			
			}

		}

		//TRIANGLES
		for (int i = 0; i < segments; i++) {
			int offset = i * vertsInShape;
			for (int l = 0; l < vertsInShape - 1; l += 1) {
				int a = offset + l;
				int b = offset + l + vertsInShape;
				int c = offset + l + 1 + vertsInShape;
				int d = offset + l + 1;
				triangleIndices.Add (a);
				triangleIndices.Add (b);
				triangleIndices.Add (c);
				triangleIndices.Add (c);
				triangleIndices.Add (d);
				triangleIndices.Add (a);
			}
		}

		mesh = new Mesh ();
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.uv3 = uvs3;
		mesh.uv4 = colorsFlowMap.ToArray ();
		
		mesh.triangles = triangleIndices.ToArray ();
		mesh.colors = colors;
		mesh.RecalculateTangents ();
		meshfilter.mesh = mesh;
	}

	float FlowCalculate (float u, float normalY)
	{
		return Mathf.Lerp (flowWaterfall.Evaluate (u), flowFlat.Evaluate (u), Mathf.Clamp (normalY, 0, 1));
	}


}