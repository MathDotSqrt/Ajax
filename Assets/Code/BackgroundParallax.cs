using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour {

	public Transform[] backgrounds;

	private Transform[] backgroundSwaps;
	private float[] backgroundWidths;
	private float[] parallaxScale;
	private new Transform camera;
	private Vector3 lastCameraPosition;

	public void Awake() {

	}

	// Start is called before the first frame update
	void Start() {
		parallaxScale = new float[backgrounds.Length];
		backgroundSwaps = new Transform[backgrounds.Length];
		backgroundWidths = new float[backgrounds.Length];

		for (int i = 0; i < backgrounds.Length; i++) {
			Transform background = backgrounds[i];

			//the larger the z the smaler the parallaxScale
			//z + 1 because we want the backround to have a parallaxScale of 1 at z = 0
			parallaxScale[i] = 1f / (background.position.z + 1);

			//set the world space widths of the sprite backgrounds 
			float width = background.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
			backgroundWidths[i] = width;

			//creates copy of background gameobject to do the background swapping for the infinite parallax
			GameObject copyBackground = Object.Instantiate(background.gameObject, background.parent);
			backgroundSwaps[i] = copyBackground.transform;
			backgroundSwaps[i].Translate(width, 0, 0);


		}

		camera = Camera.main.transform;
		lastCameraPosition = camera.position;
		
	}

	// Update is called once per frame
	void Update() {
		float deltaX = camera.position.x - lastCameraPosition.x;

		for (int i = 0; i < backgrounds.Length; i++) {
			Transform background = backgrounds[i];

			background.Translate(-deltaX * parallaxScale[i], 0, 0);
			backgroundSwaps[i].Translate(-deltaX * parallaxScale[i], 0, 0);
		}

		lastCameraPosition = camera.position;

	}

	
}
