using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParralax2 : MonoBehaviour {
	public Transform[] backgrounds;

	private Transform[] backgroundSwaps;
	private float[] backgroundWidths;
	private float[] backgroundY;

	private Transform camera;
	private Vector3 lastCameraPosition;
	private Vector3 firstCameraPosition;

	// Start is called before the first frame update
	void Start() {
		backgroundSwaps = new Transform[backgrounds.Length];
		backgroundWidths = new float[backgrounds.Length];
		backgroundY = new float[backgrounds.Length];
		camera = Camera.main.transform;


		for (int i = 0; i < backgrounds.Length; i++) {
			GameObject cloneBackground = Object.Instantiate(backgrounds[i].gameObject, backgrounds[i].parent);
			backgroundSwaps[i] = cloneBackground.transform;

			backgroundWidths[i] = cloneBackground.GetComponent<SpriteRenderer>().bounds.size.x;
			backgroundY[i] = backgrounds[i].position.y;
		}

		lastCameraPosition = camera.position;
		firstCameraPosition = camera.position;

	}

	private Vector3 vel = Vector3.zero;
	// Update is called once per frame
	void OnPreRender() {
		Vector3 delta = (camera.position - lastCameraPosition);
		for (int i = 0; i < backgrounds.Length; i++) {
			float parallaxScale = 1f / (backgrounds[i].position.z + 1);

			Vector3 position = backgrounds[i].position;
			position.x -= delta.x * (parallaxScale);
			position.y -= delta.y * (parallaxScale);

			backgrounds[i].position = position;
			backgroundSwaps[i].position = position;

			Vector3 localPosition = backgrounds[i].localPosition;
			float direction = -Mathf.Sign(localPosition.x);
			backgroundSwaps[i].Translate(direction * backgroundWidths[i], 0, 0);
			if (Mathf.Abs(localPosition.x) > backgroundWidths[i] / 2f) {
				Debug.Log(backgrounds[i].position + " : " + backgroundSwaps[i].position);

				Transform tempBackground = backgrounds[i];
				backgrounds[i] = backgroundSwaps[i];
				backgroundSwaps[i] = tempBackground;
				Debug.Log(backgrounds[i].position + " : " + backgroundSwaps[i].position);

			}
		}

		lastCameraPosition = camera.position;
	}
}
