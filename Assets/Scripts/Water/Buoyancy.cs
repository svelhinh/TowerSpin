using System.Collections;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
	public new Rigidbody rigidbody;
	public float UpwardForce = 38.16f;
	private bool isInWater = false;

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag != "ball")
		{
			isInWater = true;
			rigidbody.drag = 5f;
			rigidbody.angularDrag = 5f;
			rigidbody.useGravity = false;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag != "ball")
		{
			isInWater = false;
			rigidbody.drag = 0f;
			rigidbody.angularDrag = 0f;
			rigidbody.useGravity = true;
		}
	}

	void FixedUpdate()
	{
		if (isInWater)
		{
			Vector3 force = transform.up * UpwardForce;
			rigidbody.AddRelativeForce(force, ForceMode.Acceleration);
		}
	}
}
