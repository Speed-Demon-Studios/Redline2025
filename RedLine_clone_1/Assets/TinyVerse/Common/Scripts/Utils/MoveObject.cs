using System.Collections;
using UnityEngine;

namespace TinyVerse.Common
{
	public class MoveObject : MonoBehaviour
	{
		/// <summary>
		/// Distance to move in each direction
		/// </summary>
		[Tooltip("Distance to move in each direction")]
		public float moveDistance = 1f;

		/// <summary>
		/// Speed of movement
		/// </summary>
		[Tooltip("Speed of movement")]
		public float moveSpeed = 2f;

		/// <summary>
		/// Whether to move on the x-axis or y-axis
		/// </summary>
		[Tooltip("Whether to move on the x-axis or y-axis")]
		public bool moveOnXAxis = true;

		private Vector3 startPos;
		private float moveAxis;
		private float direction = 1f;
		private bool moveInPositiveDirection = true;

		void Start()
		{
			// store the starting position of the object
			startPos = transform.position;

			// set the move axis based on whether to move on the x-axis or y-axis
			if (moveOnXAxis)
			{
				moveAxis = startPos.x;
			}
			else
			{
				moveAxis = startPos.y;
			}

			// set the direction based on whether to move in the positive direction or negative direction
			if (!moveInPositiveDirection)
			{
				direction = -1f;
			}
		}

		void Update()
		{
			// calculate the new position of the object based on the current direction
			Vector3 newPos = transform.position;

			if (moveOnXAxis)
			{
				newPos.x = moveAxis + (moveDistance * direction);
			}
			else
			{
				newPos.y = moveAxis + (moveDistance * direction);
			}

			// smoothly move the object to the new position over time
			transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveSpeed);

			// if the object has reached the end of its movement range, switch direction
			if (Mathf.Abs((moveOnXAxis ? transform.position.x : transform.position.y) - moveAxis) >= moveDistance - 0.01f)
			{
				direction *= -1;
			}
		}
	}
}
