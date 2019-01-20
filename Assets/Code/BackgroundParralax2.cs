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
		camera = GetComponent<Camera>().transform;


		for (int i = 0; i < backgrounds.Length; i++) {
			GameObject cloneBackground = Object.Instantiate(backgrounds[i].gameObject, backgrounds[i].parent);
			backgroundSwaps[i] = cloneBackground.transform;

			backgroundWidths[i] = cloneBackground.GetComponent<SpriteRenderer>().bounds.size.x;
			backgroundY[i] = backgrounds[i].position.y;
		}

		lastCameraPosition = camera.position;
		firstCameraPosition = camera.position;
	}

	// Update is called once per frame
	void OnPreRender() {
		Vector2 cameraDelta = (camera.position - lastCameraPosition) * Time.deltaTime;

		for (int i = 0; i < backgrounds.Length; i++) {
			float parallaxScale = 60f / (backgrounds[i].position.z + 1);

			//set background to y = 0
			Vector3 pos = backgrounds[i].position;
			pos.y = backgroundY[i] - camera.position.y / parallaxScale ;
			backgrounds[i].position = pos;

			//calculate parallax
			backgrounds[i].Translate(-cameraDelta.x * parallaxScale, 0, 0);

			//offset direction
			Vector3 localPosition = backgrounds[i].localPosition;
			float direction = -Mathf.Sign(localPosition.x);

			//background swap offset
			pos.x += direction * backgroundWidths[i];
			backgroundSwaps[i].position = pos;

			//swap background with backgroundswap
			if (Mathf.Abs(localPosition.x) >= backgroundWidths[i] / 2f) {
				Transform tempBackground = backgrounds[i];
				backgrounds[i] = backgroundSwaps[i];
				backgroundSwaps[i] = tempBackground;

			}

		}

		lastCameraPosition = camera.position;
	}
}
