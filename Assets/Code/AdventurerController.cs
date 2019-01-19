using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class AdventurerController : BaseController {

	[Header("Movement")]
	public float maxRunSpeed = 10;
	[Range(0, 1)]
	public float floorSmoothing = .01f;
	[Range(0, 1)]
	public float airSmoothing = .05f;

	public float jumpForce = 20f;
	public float maxFallSpeed = 20f;

	public float slideDuration = 10f;
	public float slideSpeed = 10f;
	public float slideCoolDown = 1f;

	[Header("Collision")]
	public LayerMask collisionLayer;

	public Transform ceilingCheck = null;
	public Transform floorCheck = null;

	public float ceilingRadius = .1f;
	public float floorRadius = .1f;


	private SpriteRenderer sprite;
	private Rigidbody2D body;
	private Animator animator;

	private float facingDirection = 1;
	private bool isOnGround = false;
	private bool isCrouching = false;
	private bool isSliding = false;
	private bool isMoving = false;
	private Vector2 smoothedVel;

	private float slidingTimer = 0f;
	private float slideCoolDownTimer = 0f;

	void Start() {
		sprite = GetComponent<SpriteRenderer>();
		body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		smoothedVel = new Vector2();
	}

	void FixedUpdate() {
		Collider2D[] collisions = Physics2D.OverlapCircleAll(floorCheck.position, floorRadius, collisionLayer);

		isOnGround = collisions.Length > 0;

		if (isSliding) {
			body.velocity = new Vector2(facingDirection * slideSpeed, 0);
		}
	}

	void Update() {
		isMoving = Mathf.Abs(body.velocity.x) > .1f;

		animator.SetFloat("xSpeed", Mathf.Abs(body.velocity.x));
		animator.SetFloat("yVel", body.velocity.y);
		animator.SetBool("isOnGround", isOnGround);
		animator.SetBool("isCrouching", isCrouching);
		animator.SetBool("isMoving", isMoving);
		animator.SetBool("isSliding", isSliding);

		if (isSliding) {
			if (slidingTimer >= slideDuration) {
				isSliding = false;
				body.constraints = RigidbodyConstraints2D.FreezeRotation;

			}
			slidingTimer += Time.fixedDeltaTime;
		}
		else if (slideCoolDownTimer < slideCoolDown) {
			slideCoolDownTimer += Time.deltaTime;
		}
	}

	//ceiling y 0.1147
	//ceiling y 0.044
	public override void Move(float direction) {
		if (isSliding)
			return;

		float dir = Mathf.Sign(direction);
		if (direction != 0 && dir != facingDirection) {
			Flip();
		}

		Vector2 currentVel = body.velocity;
		Vector2 targetVel = new Vector2(direction * maxRunSpeed * 60, currentVel.y);

		float smoothing = isOnGround ? floorSmoothing : airSmoothing;
		Vector2 velocity = Vector2.SmoothDamp(currentVel, targetVel, ref smoothedVel, smoothing);
		body.velocity = velocity;
	}

	public override void Attack() {

	}

	public override void Crouch(float vertical) {
		isCrouching = vertical < 0 && isOnGround;
	}

	public override void Jump() {
		if (isOnGround) {
			isOnGround = false;

			Vector2 currentVel = body.velocity;
			currentVel.y = jumpForce;
			body.velocity = currentVel;
		}
	}

	public override void Slide() {
		if (slideCoolDownTimer >= slideCoolDown) {
			isSliding = true;

			slideCoolDownTimer = 0f;
			slidingTimer = 0f;
			body.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

		}
	}


	private void Flip() {
		facingDirection *= -1;
		sprite.flipX = !sprite.flipX;
	}
}
