using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "SplineProfile", menuName = "SplineProfile", order = 1)]
public class SplineProfile : ScriptableObject
{
	public Material splineMaterial;
	public AnimationCurve meshCurve = new AnimationCurve (new Keyframe[]{ new Keyframe (0, 0), new Keyframe (1, 0) });


	public float minVal = 0.5f;
	public float maxVal = 0.5f;

	public int vertsInShape = 3;
	public float traingleDensity = 0.2f;

	public float uvScale = 3;
	public bool uvRotation = true;


	public AnimationCurve flowFlat = new AnimationCurve (new Keyframe[] {
		new Keyframe (0, 0.025f),
		new Keyframe (0.5f, 0.05f),
		new Keyframe (1, 0.025f)
	});
	public AnimationCurve flowWaterfall = new AnimationCurve (new Keyframe[] {
		new Keyframe (0, 0.25f),
		new Keyframe (1, 0.25f)
	});
}
