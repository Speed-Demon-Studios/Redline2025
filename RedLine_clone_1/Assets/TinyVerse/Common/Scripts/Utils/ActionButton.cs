using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TinyVerse.Common
{
	public class ActionButton : MonoBehaviour
	{
		public Transform button;
		public UnityEvent OnPressEvent = new UnityEvent();

		float isPressedTimeout = 0.25f;
		float isPressedPassed = 0f;

		void Start()
		{
			OnPressEvent.AddListener(OnPressEventAction);
		}

		void OnPressEventAction()
		{
			isPressedPassed = 0f;
		}

		void Update()
		{
			if (isPressedPassed < isPressedTimeout)
			{
				var step = (1 / isPressedTimeout) * Time.deltaTime;
				button.localScale = Vector3.MoveTowards(button.localScale, new Vector3(1f, 0.25f, 1f), step);
				isPressedPassed += Time.deltaTime;
			}
			else
			{
				if (button.localScale != Vector3.one)
				{
					var step = (1 / isPressedTimeout) * Time.deltaTime;
					button.localScale = Vector3.MoveTowards(button.localScale, Vector3.one, step);
				}
			}
		}
	}
}
