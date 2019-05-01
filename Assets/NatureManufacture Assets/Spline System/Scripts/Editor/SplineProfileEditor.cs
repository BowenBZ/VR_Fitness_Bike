using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(SplineProfile)), CanEditMultipleObjects]
public class SplineProfileEditor : Editor
{
	

	public override void OnInspectorGUI ()
	{
		SplineProfile spline = (SplineProfile)target;

		spline.splineMaterial = (Material)EditorGUILayout.ObjectField ("Material", spline.splineMaterial, typeof(Material), false);
		
		spline.meshCurve = EditorGUILayout.CurveField ("Mesh curve", spline.meshCurve);

		EditorGUILayout.LabelField ("Vertice distribution: " + spline.minVal.ToString () + " " + spline.maxVal.ToString ());
		EditorGUILayout.MinMaxSlider (ref spline.minVal, ref spline.maxVal, 0, 1);
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

		spline.traingleDensity = 1 / (float)EditorGUILayout.IntSlider ("U", (int)(1 / (float)spline.traingleDensity), 1, 100);
		spline.vertsInShape = EditorGUILayout.IntSlider ("V", spline.vertsInShape - 1, 1, 20) + 1;

		spline.uvScale = EditorGUILayout.FloatField ("UV scale (texture tiling)", spline.uvScale);
		spline.uvRotation = EditorGUILayout.Toggle ("Rotate UV", spline.uvRotation);

		spline.flowFlat = EditorGUILayout.CurveField ("Flow curve flat speed", spline.flowFlat);
		spline.flowWaterfall = EditorGUILayout.CurveField ("Flow curve waterfall speed", spline.flowWaterfall);


		EditorUtility.SetDirty (target);

	}
}
