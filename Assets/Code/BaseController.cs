using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour {
	public abstract void Move(float direction);
	public abstract void Attack();
	public abstract void Crouch(float vertical);
	public abstract void Jump();
	public abstract void Slide();

}
