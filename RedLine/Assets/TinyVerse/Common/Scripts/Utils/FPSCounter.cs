using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TinyVerse.Common
{
	public class FPSCounter : MonoBehaviour
	{
		public TextMeshProUGUI Text;
		public float UpdateTime = 0.3f;

		int FpsCount = 0;
		float Timer = 0;

		void LateUpdate()
		{
			if (Timer >= UpdateTime)
			{
				Text.text = ((int)(FpsCount / Timer)).ToString();
				Timer = FpsCount = 0;
			}
			else
			{
				Timer += Time.deltaTime;
				FpsCount++;
			}
		}
	}
}
