using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private BaseController controller;

	private float horizontal = 0;
	private float vertical = 0;
	private bool jump = false;
	private bool enter = false;
	private bool shift = false;

	void Start() {
		controller = GetComponent<BaseController>();
	}

	void Update() {
		horizontal = Input.GetAxisRaw("Horizontal");
		vertical = Input.GetAxisRaw("Vertical");
		jump = Input.GetKey(KeyCode.Space);
		enter = Input.GetKey(KeyCode.Return);
		shift = Input.GetKey(KeyCode.LeftShift);

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	void FixedUpdate() {
		controller.Move(horizontal * Time.fixedDeltaTime);
		controller.Crouch(vertical);

		if (enter) {
			controller.Attack();
		}
		if (jump) {
			controller.Jump();
		}
		if (shift) {
			controller.Slide();
		}
	}
}