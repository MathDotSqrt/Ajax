using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character2DController : MonoBehaviour {
	public float jumpForce = 100f;
	public float moveSpeed = 10f;
	[Range(0f, 1f)] public float smoothedTime = .5f;
	[Range(0f, 1f)] public float crouchSpeed = .3f;

	public LayerMask whatIsGround;

	public Transform groundCheck = null;
	public Transform ceilingCheck = null;

	public float groundRadius = .1f;
	public float ceilingRadius = .1f;


	private Transform playerTransform;
	private Rigidbody2D playerRigidBody;
	private Animator playerAnimator;



	private bool isOnGround = false;
	private float playerDirection = 1;
	private Vector2 smoothedVel = new Vector2();

	void Start() {
		playerTransform = GetComponent<Transform>();
		playerRigidBody = GetComponent<Rigidbody2D>();
		playerAnimator = GetComponent<Animator>();

	}

	void FixedUpdate() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, whatIsGround);

		isOnGround = false;

		foreach (Collider2D c in colliders) {
			if (c.gameObject != base.gameObject) {
				isOnGround = true;
				break;
			}
		}

		playerAnimator.SetFloat("Speed", Mathf.Abs(playerRigidBody.velocity.x));
		playerAnimator.SetFloat("VerticalVelocity", playerRigidBody.velocity.y);
		playerAnimator.SetBool("IsOnGround", isOnGround);

	}

	public void Move(float direction) {
		if (direction != 0 && Mathf.Sign(direction) != Mathf.Sign(playerDirection))
			Flip();

		Vector2 currentVel = playerRigidBody.velocity;
		Vector2 targetVel = new Vector2(direction * moveSpeed, currentVel.y);

		playerRigidBody.velocity = Vector2.SmoothDamp(currentVel, targetVel, ref smoothedVel, smoothedTime);
	}

	public void Jump() {
		if (isOnGround) {
			isOnGround = false;
			Vector2 currentVel = playerRigidBody.velocity;
			currentVel.y = jumpForce;
			playerRigidBody.velocity = currentVel;
		}
	}

	public void Flip() {
		playerDirection = -playerDirection;
		playerTransform.Rotate(0, playerDirection * 180 + playerTransform.rotation.y, 0);
	}
}
