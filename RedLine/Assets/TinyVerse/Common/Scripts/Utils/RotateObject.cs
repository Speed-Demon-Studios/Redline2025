using System.Collections;
using UnityEngine;

namespace TinyVerse.Common
{
	public class RotateObject : MonoBehaviour
	{
		public bool constantlyApplyForce = true;
		public float speedMultiplier = 1f;
		public bool randomizeAxis = false;
		public Vector3 rotationAxis = new Vector3(0, 1, 0);
		public bool continuouslyRandomizeAxis = false;
		public float continuouslyRandomizeAxisTimeout = 1f;

		[System.NonSerialized]
		private Transform _tr;

		[System.NonSerialized]
		private Rigidbody _rb;

		[System.NonSerialized]
		private Vector3 _rotationAxis;

		[System.NonSerialized]
		private Vector3 _worldRotationAxis;

		[System.NonSerialized]
		private float _continuouslyRandomizeAxisTimeout;

		void OnEnable()
		{
			_tr = GetComponent<Transform>();
			_rb = GetComponent<Rigidbody>();
		}

		void Start()
		{
			if (randomizeAxis)
			{
				rotationAxis = UnityEngine.Random.onUnitSphere;
			}
			_rotationAxis = rotationAxis;
			_worldRotationAxis = transform.TransformDirection(_rotationAxis);
			_continuouslyRandomizeAxisTimeout = 0f;
		}

		void FixedUpdate()
		{
			if (continuouslyRandomizeAxis)
			{
				if (_continuouslyRandomizeAxisTimeout > continuouslyRandomizeAxisTimeout)
				{
					rotationAxis = UnityEngine.Random.onUnitSphere;
					_continuouslyRandomizeAxisTimeout = 0f;
				}
				else
				{
					_continuouslyRandomizeAxisTimeout += Time.fixedDeltaTime;
				}
			}

			if (_rotationAxis != rotationAxis)
			{
				_rotationAxis = rotationAxis;
				_worldRotationAxis = transform.TransformDirection(_rotationAxis);
			}

			if (constantlyApplyForce)
			{
				if (_rb != null)
				{
					_rb.AddRelativeTorque(_rotationAxis * speedMultiplier * Time.deltaTime);
				}
				else
				{
					_tr.RotateAround(transform.position, _worldRotationAxis, speedMultiplier * Time.deltaTime);
				}
			}
		}

		public void AddExtraForce(float force)
		{
			if (_rb != null)
			{
				_rb.AddRelativeTorque(_rotationAxis * speedMultiplier * force * Time.deltaTime);
			}
			else
			{
				_tr.RotateAround(transform.position, _worldRotationAxis, speedMultiplier * force * Time.deltaTime);
			}
		}
	}
}
