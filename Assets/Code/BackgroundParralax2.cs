using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParralax2 : MonoBehaviour {
	public Transform[] backgrounds;
	public Vector2[] parallax;

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

			parallax[i].x = 1f / parallax[i].x;
			parallax[i].y = 1f / parallax[i].y;
		}

		lastCameraPosition = camera.position;
		firstCameraPosition = camera.position;

	}

	private Vector3 vel = Vector3.zero;
	// Update is called once per frame
	void OnPreRender() {
		Vector3 delta = (camera.position - lastCameraPosition);
		for (int i = 0; i < backgrounds.Length; i++) {

			Vector3 position = backgrounds[i].position;
			position.x -= delta.x * parallax[i].x;
			position.y -= delta.y * parallax[i].y;

			backgrounds[i].position = position;
			backgroundSwaps[i].position = position;

			Vector3 localPosition = backgrounds[i].localPosition;
			float direction = -Mathf.Sign(localPosition.x);
			backgroundSwaps[i].Translate(direction * backgroundWidths[i], 0, 0);


			if (Mathf.Abs(localPosition.x) > backgroundWidths[i] / 2f) {
				Transform tempBackground = backgrounds[i];
				backgrounds[i] = backgroundSwaps[i];
				backgroundSwaps[i] = tempBackground;
			}
		}

		lastCameraPosition = camera.position;
	}
}
