using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTransformer : MonoBehaviour {

	public Material material;

	public float radiusOnSurface = 5000;
	public float sealevel = 0;

	[Min(.0001f)] public float alpha = 3;
	[Min(0)] public float beta = 1;
	[Min(0)] public float yOffset = 0;

	void Start() {
		material.SetFloat("_r", radiusOnSurface);
		material.SetFloat("_sealevel", sealevel);

	}

	void OnPreRender() {
		float y = -transform.position.y - yOffset;

		y = beta * Mathf.Max(y, 0);

		float r = radiusOnSurface * (1 - y / ((Mathf.Abs(y) + alpha)));
		material.SetFloat("_r", r);
	}

}
