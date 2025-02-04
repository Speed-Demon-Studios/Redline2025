using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyVerse.Common
{
	public static class MathExtentions
	{
		public static float Quad(this float t)
		{
			return t < 0.5f ? (t * 2).InQuad() / 2 : (t * 2 - 1).OutQuad() / 2 + 0.5f;
		}

		public static float InQuad(this float t)
		{
			return t * t;
		}

		public static float OutQuad(this float t)
		{
			return 1 - (1 - t).InQuad();
		}

		public static float EaseInOutQuad(this float t)
		{
			return (t < 0.5 ? 2 * t * t : 1 - ((-2 * t + 2) * (-2 * t + 2)) / 2);
		}

		public static float Quart(this float t)
		{
			return t < 0.5f ? (t * 2).InQuart() / 2 : (t * 2 - 1).OutQuart() / 2 + 0.5f;
		}

		public static float InQuart(this float t)
		{
			return t * t * t * t;
		}

		public static float OutQuart(this float t)
		{
			return 1 - (1 - t).InQuart();
		}

		public static float EaseSine(this float t)
		{
			return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
		}

		public static float EaseOutBounce(this float t)
		{
			float n1 = 7.5625f;
			float d1 = 2.75f;

			if (t < 1f / d1)
			{
				return n1 * t * t;
			}
			else if (t < 2f / d1)
			{
				return n1 * (t -= 1.5f / d1) * t + 0.75f;
			}
			else if (t < 2.5f / d1)
			{
				return n1 * (t -= 2.25f / d1) * t + 0.9375f;
			}
			else
			{
				return n1 * (t -= 2.625f / d1) * t + 0.984375f;
			}
		}

		public static float EaseInBack(this float t)
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1f;
			return c3 * t * t * t - c1 * t * t;
		}

		public static float EaseOutBack(this float t)
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1f;
			return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
		}

		public static float EaseInOutBack(this float t)
		{
			float c1 = 1.70158f;
			float c2 = c1 * 1.525f;

			return t < 0.5f
				? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f
				: (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
		}

		public static float SmootherStep(this float x, float edge0, float edge1)
		{
			// Scale, and clamp x to 0..1 range
			x = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
			return x * x * x * (x * (6.0f * x - 15.0f) + 10.0f);
		}
	}
}
