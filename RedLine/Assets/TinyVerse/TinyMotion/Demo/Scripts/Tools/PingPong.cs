using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TinyVerse.Common;

namespace TinyVerse.TinyMotion.Demo
{
	public class PingPong : MonoBehaviour
	{
		public float speed = 1f;
		public Vector3 OffsetStart;
		public Vector3 OffsetEnd;

		private Transform _tr;
		private float _timeOffset;
		private float _timeOffsetQuart;
		private float _timeOffsetSign = 1f;
		private float _speedFract
		{
			get { return 1 / speed; }
		}

		void Start()
		{
			_tr = transform;
		}

		void FixedUpdate()
		{
			Vector3 pos = _tr.position;

			if (_timeOffset > 1f || _timeOffset < -1)
			{
				_timeOffsetSign *= -1;
			}

			_timeOffset += Time.fixedDeltaTime * _speedFract * _timeOffsetSign;
			_timeOffsetQuart = _timeOffset.EaseSine();

			if (OffsetStart.x != OffsetEnd.x)
				pos.x = Mathf.Lerp(OffsetStart.x, OffsetEnd.x, _timeOffsetQuart);
			if (OffsetStart.y != OffsetEnd.y)
				pos.y = Mathf.Lerp(OffsetStart.y, OffsetEnd.y, _timeOffsetQuart);
			if (OffsetStart.z != OffsetEnd.z)
				pos.z = Mathf.Lerp(OffsetStart.z, OffsetEnd.z, _timeOffsetQuart);

			_tr.position = pos;
		}
	}
}
