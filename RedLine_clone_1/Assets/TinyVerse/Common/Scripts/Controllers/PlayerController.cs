using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace TinyVerse.Common
{
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class PlayerController : MonoBehaviour
	{
		public Camera playerCamera;
		public Transform playerTransform;
		public Rigidbody playerRigidBody;
		public bool isEnabled = true;

		[Header("Movement")]
		public LayerMask groundMask;
		public float mouseSensitivity = 0.25f;
		public float moveSpeed = 10f;
		public float maxAcceleration = 30f;
		public float jumpForce = 10f;
		public float groundCheckDistance = 0.1f;
		public float cameraSmoothTime = 0.025f;

		[System.NonSerialized]
		public bool isGrounded = true;

		[Header("Shooting")]
		public LayerMask shootMask;
		private float delayBetweenShots = 0.1f;
		public AudioClip shotClip;
		public AudioClip shotHitClip;
		public GameObject shotHitEffect;

		[Header("Grabbing")]
		public LayerMask grabMask;
		public float grabForce = 50f;
		public float grabDamping = 10f;
		public LineRenderer lineRenderer;
		public float throwForce = 20f;

		[Header("Debug")]
		public bool debugGizmos = false;

		[Header("Events")]
		[Space(10f)]
		public UnityEvent OnOptionsEvent = new UnityEvent();
		public UnityEvent OnShootEvent = new UnityEvent();
		public UnityEvent<RaycastHit> OnShootHitEvent = new UnityEvent<RaycastHit>();
		public UnityEvent<RaycastHit> OnGrabEvent = new UnityEvent<RaycastHit>();

		private bool jumpInput = false;
		private bool fireInput = false;
		private bool grabInput = false;
		private Vector2 lookInput = Vector2.zero;
		private Vector3 moveInput = Vector3.zero;
		private Vector3 targetMoveDirection = Vector3.zero;
		private Vector3 currentVelocity = Vector3.zero;
		private Vector3 cameraLookAngle = Vector3.zero;
		private Vector3 playerLookAngle = Vector3.zero;
		private Vector3 rayOrigin = Vector3.zero;

		public HeldObject heldObject = new HeldObject();
		public bool isHoldingObject
		{
			get { return heldObject.transform != null; }
			set { }
		}

		[System.NonSerialized]
		public RaycastHit raycastHit;

		[System.NonSerialized]
		public Vector3 lookDirection;

		private Vector3 xAxis;
		private Vector3 zAxis;
		private Vector3 upAxis;
		private CapsuleCollider _collider;
		private float _shotDelayTicker = 0f;
		private float _grabDelayTicker = 2f;
		private float cameraLookAngleX = 0f;
		private float cameraLookAngleY = 0f;
		private Vector3 playerCameraTransformVelocity = Vector3.zero;
		private Vector3 playerCameraTransformOffset = Vector3.zero;

		private float _fdt;

		public virtual void Start()
		{
			_collider = playerTransform.GetComponent<CapsuleCollider>();
			xAxis = playerTransform.right;
			zAxis = playerTransform.forward;
			upAxis = playerTransform.up;

			playerCameraTransformOffset = playerCamera.transform.localPosition;
			GameController.Instance.AddPlayer(this);
			Cursor.lockState = CursorLockMode.Locked;

#if !ENABLE_INPUT_SYSTEM
			Debug.LogWarning("TinyVerse: For the PlayerController to work, please install and enable the New InputSystem.");
#endif
		}

		public virtual void OnDestroy()
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.RemovePlayer(this);
			}
		}

		public virtual void FixedUpdate()
		{
			if (isEnabled)
			{
				_fdt = Time.fixedDeltaTime;
				lookDirection = playerCamera.transform.TransformDirection(Vector3.forward);
				CheckGround();
				ProcessActions();
				ProcessMovement();
				UpdateGrabLine();
			}
		}

		public virtual void Update()
		{
		}

		public virtual void LateUpdate()
		{
			ProcessRotation();
		}

		public virtual void OnShootEventAction() { }

		public virtual void OnShootHitEventAction(RaycastHit hit) { }

		public virtual void OnGrabEventAction(RaycastHit hit) { }

		void CheckGround()
		{
			rayOrigin = playerTransform.position;
			rayOrigin.y -= (_collider.height / 2) - _collider.radius + groundCheckDistance;
			isGrounded = Physics.CheckSphere(rayOrigin, _collider.radius, groundMask);
		}

		void ProcessRotation()
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				cameraLookAngleX = cameraLookAngle.x - lookInput.y * mouseSensitivity;
				cameraLookAngleX = Mathf.Clamp(cameraLookAngle.x - lookInput.y * mouseSensitivity, -89.8f, 89.8f);
				cameraLookAngleY = cameraLookAngle.y + lookInput.x * mouseSensitivity;
				cameraLookAngle = Vector3.Lerp(cameraLookAngle, new Vector3(cameraLookAngleX, cameraLookAngleY, 0), 0.5f);
				playerLookAngle = new Vector3(0, cameraLookAngle.y, 0);
				//
				playerTransform.localEulerAngles = playerLookAngle;
				playerCamera.transform.localEulerAngles = cameraLookAngle;
			}

			playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, playerTransform.position + playerCameraTransformOffset, ref playerCameraTransformVelocity, cameraSmoothTime);
		}

		void ProcessMovement()
		{
			currentVelocity = playerRigidBody.velocity;
			// Debug.DrawRay(playerTransform.position, currentVelocity, Color.magenta, 1, false);

			targetMoveDirection = playerTransform.TransformDirection(moveInput);
			// Debug.DrawRay(playerTransform.position, targetMoveDirection, Color.red, 1, false);

			Vector3 adjustment;
			adjustment.x = targetMoveDirection.x * moveSpeed - Vector3.Dot(currentVelocity, xAxis);
			adjustment.z = targetMoveDirection.z * moveSpeed - Vector3.Dot(currentVelocity, zAxis);
			adjustment.y = 0f;
			adjustment = Vector3.ClampMagnitude(adjustment, maxAcceleration * Time.deltaTime);
			// Debug.DrawRay(playerTransform.position, adjustment, Color.green, 1, false);

			if (isGrounded)
			{
				if (jumpInput)
				{
					adjustment.y = jumpForce;
					jumpInput = false;
				}
			}
			else
			{
				adjustment *= 0.25f;
			}

			playerRigidBody.velocity += xAxis * adjustment.x + zAxis * adjustment.z + upAxis * adjustment.y;
		}

		Vector3 _pinPosition;
		Vector3 _pinPositionVelocity;
		Vector3 _grabPosition;
		Vector3 _grabForce;

		ActionButton ab;

		public virtual void ProcessActions()
		{
			bool canShoot = _shotDelayTicker > delayBetweenShots;
			bool canGrab = _grabDelayTicker > 2f;

			if (grabInput && canGrab)
			{
				if (!isHoldingObject)
				{
					if (Physics.Raycast(playerCamera.transform.position, lookDirection, out raycastHit, Mathf.Infinity, grabMask))
					{
						if (raycastHit.rigidbody != null)
						{
							heldObject.transform = raycastHit.rigidbody.transform;
							heldObject.rigidBody = raycastHit.rigidbody;
							heldObject.distance = raycastHit.distance;
							heldObject.pinPosition = raycastHit.rigidbody.transform.InverseTransformPoint(raycastHit.point);
							//
							_pinPosition = raycastHit.point;
							//
							OnGrabEvent.Invoke(raycastHit);
						}
					}
				}

				if (isHoldingObject)
				{
					_grabPosition = playerCamera.transform.position + lookDirection * heldObject.distance;

					Vector3 _newPinPosition = heldObject.transform.TransformPoint(heldObject.pinPosition);
					_pinPositionVelocity = (_pinPosition - _newPinPosition) / _fdt;
					_pinPosition = _newPinPosition;
					_grabForce = (((_grabPosition - _pinPosition) * grabForce) + (_pinPositionVelocity * grabDamping)) * _fdt;

					heldObject.rigidBody.AddForceAtPosition(_grabForce, _pinPosition, ForceMode.VelocityChange);
				}
			}
			else if (isHoldingObject)
			{
				heldObject.transform = null;
				heldObject.rigidBody = null;
				heldObject.distance = 0f;
				_shotDelayTicker = 0f;
			}
			else if (!grabInput)
			{
				_grabDelayTicker = 2f;
			}

			if (fireInput && canShoot)
			{
				if (Cursor.lockState == CursorLockMode.Locked)
				{
					if (isHoldingObject)
					{
						heldObject.rigidBody.velocity += playerCamera.transform.forward * throwForce;
						heldObject.transform = null;
						heldObject.rigidBody = null;
						heldObject.distance = 0f;
						_shotDelayTicker = 0f;
						_grabDelayTicker = 0f;
					}
					else
					{
						if (Physics.Raycast(playerCamera.transform.position, lookDirection, out raycastHit, Mathf.Infinity, shootMask))
						{
							if (raycastHit.transform.TryGetComponent<ActionButton>(out ab))
							{
								ab.OnPressEvent.Invoke();
							}
							else
							{
								OnShootHitEvent.Invoke(raycastHit);
							}
						}
						OnShootEvent.Invoke();
						_shotDelayTicker = 0f;
					}
				}
			}

			_shotDelayTicker += _fdt;
			_grabDelayTicker += _fdt;
		}

		void UpdateGrabLine()
		{
			if (null != lineRenderer)
			{
				if (isHoldingObject)
				{
					Vector3[] _positions = new Vector3[2];
					_positions[0] = _grabPosition;
					_positions[1] = _pinPosition;
					lineRenderer.positionCount = _positions.Length;
					lineRenderer.SetPositions(_positions);
					lineRenderer.enabled = true;
				}
				else
				{
					lineRenderer.enabled = false;
				}
			}
		}

		public void ToggleCursorLock()
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		void OnDrawGizmos()
		{
			if (debugGizmos)
			{
				Gizmos.color = Color.white;

				CapsuleCollider __collider = playerTransform.GetComponent<CapsuleCollider>();
				Vector3 _rayOrigin = playerTransform.position;
				_rayOrigin.y -= (__collider.height / 2) - __collider.radius + groundCheckDistance;
				Gizmos.DrawWireSphere(_rayOrigin, __collider.radius);

				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere(_pinPosition, 0.2f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(_grabPosition, 0.1f);
				Gizmos.color = Color.red;
				Gizmos.DrawLine(_pinPosition, _grabPosition);

				Gizmos.color = Color.green;
				Gizmos.DrawRay(_pinPosition, _grabForce);

			}
		}

		Vector3 Between(Vector3 v1, Vector3 v2, float percentage)
		{
			return (v2 - v1) * percentage + v1;
		}

		[System.Serializable]
		public struct HeldObject
		{
			public Transform transform;
			public Rigidbody rigidBody;
			public Vector3 pinPosition;
			public float distance;
		}

		[System.Serializable]
		public partial class GrabEvent : UnityEvent<RaycastHit> { }

		[System.Serializable]
		public partial class ShootHitEvent : UnityEvent<RaycastHit> { }

#if ENABLE_INPUT_SYSTEM
		void OnMove(InputValue value)
		{
			moveInput.x = value.Get<Vector2>().x;
			moveInput.z = value.Get<Vector2>().y;
		}

		void OnLook(InputValue value)
		{
			lookInput = value.Get<Vector2>();
		}

		void OnJump(InputValue value)
		{
			jumpInput = value.isPressed;
		}

		void OnFire(InputValue value)
		{
			fireInput = value.isPressed;
		}

		void OnGrab(InputValue value)
		{
			grabInput = value.isPressed;
		}

		void OnCursorLock(InputValue value)
		{
			ToggleCursorLock();
		}

		void OnOptions(InputValue value)
		{
			OnOptionsEvent.Invoke();
		}
#endif
	}
}
