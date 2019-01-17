using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour {

	public Transform[] backgrounds;

	private Transform[] backgroundSwaps;
	private float[] parallaxScale;
	private float[] backgroundWidths;
	private float[] panelCounts;
	private new Transform camera;
	private Vector3 lastCameraPosition;

	public void Awake() {

	}

	// Start is called before the first frame update
	void Start() {
		backgroundSwaps = new Transform[backgrounds.Length];
		parallaxScale = new float[backgrounds.Length];
		backgroundWidths = new float[backgrounds.Length];
		panelCounts = new float[backgrounds.Length];

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

		//Grabs main camera transform in the scene 
		camera = Camera.main.transform;
		lastCameraPosition = camera.position;
		
	}

	// Update is called once per frame
	void Update() {
		float cameraDelta = camera.position.x - lastCameraPosition.x;

		for (int i = 0; i < backgrounds.Length; i++) {
			//translates background sprite by camera delta multiplied by the parallax scale
			backgrounds[i].Translate(-cameraDelta * parallaxScale[i], 0, 0);

			//sets background swap to the same position as background sprite 
			//this is done to offset the background swap in front or behing the background sprite
			//depending on where the camera is
			backgroundSwaps[i].position = backgrounds[i].position;

			//computs the delta of camera with the position of the background sprite
			//lots of transformations are needed to be done here to calculate the correct delta
			float cameraBackgroundDelta = parallaxScale[i] * camera.position.x - backgrounds[i].position.x - backgroundWidths[i] * panelCounts[i];

			//if cameraBackgroundDelta is positive translate the background swap to be infront
			//if cameraBackgroundDelta is negative translate the background swap to be behind
			float sign = Mathf.Sign(cameraBackgroundDelta);
			backgroundSwaps[i].Translate(sign * backgroundWidths[i], 0, 0);

			//tiny epsilon to prefent infinent swapping if camera is perfectly on the swap border
			const float epsilon = .05f;
			
			//if the magnitude of cameraBackgroundDelta is larger than the width of the sprite
			//swap the current background with the background swap
			if (Mathf.Abs(cameraBackgroundDelta) > backgroundWidths[i] + epsilon) {
				Transform tempBackground = backgroundSwaps[i];
				backgroundSwaps[i] = backgrounds[i];
				backgrounds[i] = tempBackground;

				//increment or decrement the panelCount depending on if the camera swaped twoard the left or the right
				panelCounts[i] += sign;
			}
		}

		lastCameraPosition = camera.position;
	}

	
}
