using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyVerse.TinyMotion.Demo
{
	public class ForceField : MonoBehaviour
	{
		public float gravityRadius = 50f;
		public float gravityStrength = 10f;
		public Vector3 initialVelocity = Vector3.forward;
		public float initialVelocityStrength = 1000f;
		public LayerMask gravityMask = Physics.DefaultRaycastLayers;

		Rigidbody _rb;
		Rigidbody _hitBody;
		Collider[] overlapColliders;
		Vector3 _hitBodyPostition;
		Vector3 _directionToHitBody;
		Vector3 _forceStrength;

		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_rb.AddForce(transform.TransformDirection(initialVelocity) * initialVelocityStrength);
		}

		private void FixedUpdate()
		{
			Vector3 _position = transform.position;
			overlapColliders = Physics.OverlapSphere(_position, gravityRadius, gravityMask);

			foreach (Collider col in overlapColliders)
			{
				if (col.attachedRigidbody != null && col.attachedRigidbody != _rb)
				{
					_hitBody = col.attachedRigidbody;
					_hitBodyPostition = _hitBody.transform.position;
					_directionToHitBody = _position - _hitBodyPostition;
					_forceStrength = gravityStrength * _directionToHitBody.normalized * Mathf.Clamp(gravityRadius - _directionToHitBody.magnitude, 0, gravityRadius);

					Debug.DrawRay(_hitBodyPostition, _forceStrength, Color.green, 0.1f, false);

					_hitBody.AddForce(_forceStrength);
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, gravityRadius);
			Gizmos.DrawRay(transform.position, transform.TransformDirection(initialVelocity));
		}
	}
}
