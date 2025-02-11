// Rotate an object around its Y (upward) axis in response to
// left/right controls.
using System.Collections;
using UnityEngine;

namespace TinyVerse.TinyMotion.Demo
{
	public class SetCOM : MonoBehaviour
	{
		public Vector3 COM = new Vector3(0, 0, 0);

		Rigidbody rb;

		void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		void FixedUpdate()
		{
			rb.centerOfMass = COM;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.TransformPoint(GetComponent<Rigidbody>().centerOfMass), 0.25f);
		}
	}
}
