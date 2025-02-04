using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyVerse.Common
{
	public static class VectorExtensions
	{
		/// <summary>
		///     Inverts Vector direction.
		/// </summary>
		public static Vector3 Inverse(this Vector3 a)
		{
			return new Vector3(1 / a.x, 1 / a.y, 1 / a.z);
		}

		/// <summary>
		///     Subtracts b values from a.
		/// </summary>
		public static Vector3 Subtract(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		/// <summary>
		///     Adds b values to a.
		/// </summary>
		public static Vector3 Add(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		/// <summary>
		///     Multiplies a values by b.
		/// </summary>
		public static Vector3 Multiply(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		/// <summary>
		///     Sets Y component of Vector3 to 0.
		/// </summary>
		public static Vector3 Flat(this Vector3 vector)
		{
			vector.y = 0;
			return vector;
		}

		public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
		{
			vector.x = Mathf.Clamp(vector.x, min.x, max.x);
			vector.y = Mathf.Clamp(vector.y, min.y, max.y);
			vector.z = Mathf.Clamp(vector.z, min.z, max.z);
			return vector;
		}

		public static Vector3 Clamp(this Vector3 vector, float min, float max)
		{
			vector.x = Mathf.Clamp(vector.x, min, max);
			vector.y = Mathf.Clamp(vector.y, min, max);
			vector.z = Mathf.Clamp(vector.z, min, max);
			return vector;
		}

		/// <summary>
		///     Calculates average collision normal from a list of contact points.
		/// </summary>
		public static Vector3 AverageCollisionNormal(ContactPoint[] contacts)
		{
			Vector3[] points = new Vector3[contacts.Length];
			int n = contacts.Length;
			for (int i = 0; i < n; i++)
			{
				points[i] = contacts[i].normal;
			}
			return AveragePoint(points);
		}

		/// <summary>
		///     Calculates average collision point from a list of contact points.
		/// </summary>
		public static Vector3 AverageCollisionPoint(ContactPoint[] contacts)
		{
			Vector3[] points = new Vector3[contacts.Length];
			int n = contacts.Length;
			for (int i = 0; i < n; i++)
			{
				points[i] = contacts[i].point;
			}

			return AveragePoint(points);
		}

		/// <summary>
		///     Calculates average from multiple vectors.
		/// </summary>
		private static Vector3 AveragePoint(Vector3[] points)
		{
			Vector3 sum = Vector3.zero;
			int n = points.Length;
			for (int i = 0; i < n; i++)
			{
				sum += points[i];
			}
			return sum / points.Length;
		}

		/// <summary>
		///     Converts eulerAngles to Vector3.
		/// </summary>
		public static Vector3 AnglesToVector(this Vector3 Angles)
		{
			if (Angles.x > 180)
				Angles.x = -360 + Angles.x;
			if (Angles.y > 180)
				Angles.y = -360 + Angles.y;
			if (Angles.z > 180)
				Angles.z = -360 + Angles.z;
			return Angles;
		}
	}
}
