using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private Character2DController controller;

	private float horizontal = 0;
	private bool jump = false;

	void Start() {
		controller = GetComponent<Character2DController>();
	}

	void Update() {
		horizontal = Input.GetAxisRaw("Horizontal");
		jump = Input.GetKey("space");

		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	void FixedUpdate() {
		controller.Move(horizontal * Time.fixedDeltaTime);
		if (jump) {
			controller.Jump();
		}
	}
}