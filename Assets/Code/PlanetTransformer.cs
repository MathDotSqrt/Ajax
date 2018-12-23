using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTransformer : MonoBehaviour {

	public Material material;

	public float radiusOnSurface = 5000;
	public float sealevel = 0;
	public float smoothing = 3;

	void Start() {
		material.SetFloat("_r", radiusOnSurface);
		material.SetFloat("_sealevel", sealevel);

	}

	void Update() {
		if (Input.GetKey("r")) {
			material.SetFloat("_r", material.GetFloat("_r") * .99f);
		}
		if (Input.GetKey("t")) {
			material.SetFloat("_r", material.GetFloat("_r") * 1.0f / .99f);
		}
	}

	void OnPreRender() {
		float y = -transform.position.y;
		y = Mathf.Max(y, 0);

		float r = radiusOnSurface * (1 - y / ((Mathf.Abs(y) + smoothing)));
		material.SetFloat("_r", r);
	}

}
