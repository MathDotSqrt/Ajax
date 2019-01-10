using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectCameraResolution : MonoBehaviour {

	public Camera reflectionCamera;
	public int divisor = 1;


	private RenderTexture reflectionTexture;
	private Material reflectionMaterial;

	private Transform quadTransform;
	private Transform reflectCameraTransform;

	// Start is called before the first frame update
	void Start() {
		quadTransform = GetComponent<Transform>();
		reflectCameraTransform = reflectionCamera.transform;

		Vector2 resolution = new Vector2(reflectionCamera.pixelWidth, reflectionCamera.pixelHeight);

		reflectionTexture = new RenderTexture((int)resolution.x / divisor, (int)resolution.y / divisor, 16, RenderTextureFormat.ARGB32);
		reflectionTexture.autoGenerateMips = false;
		reflectionTexture.antiAliasing = 1;
		reflectionTexture.filterMode = FilterMode.Point;
		reflectionTexture.wrapMode = TextureWrapMode.Mirror;
		reflectionTexture.Create();


		reflectionCamera.targetTexture = reflectionTexture;

		reflectionMaterial = GetComponent<MeshRenderer>().material;
		reflectionMaterial.SetTexture("ReflectionTexture", reflectionTexture);
	}

	// Update is called once per frame
	void Update() {
		float quadY = quadTransform.position.y + (quadTransform.localScale.y) / 2f;
		reflectionMaterial.SetFloat("quadY", quadY);
	}
}
