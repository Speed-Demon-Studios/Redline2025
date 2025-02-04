using UnityEngine;

namespace TinyVerse.TinyMotion
{
	internal sealed class TinyMotionData
	{
		bool _firstFrame;
		int _lastFrameCount;
		Matrix4x4 _CurrentViewProjectionMatrix;
		Matrix4x4 _PreviousViewProjectionMatrix;

		internal TinyMotionData()
		{
			_firstFrame = true;
			_lastFrameCount = -1;
			_CurrentViewProjectionMatrix = Matrix4x4.identity;
			_PreviousViewProjectionMatrix = Matrix4x4.identity;
		}

		internal bool firstFrame
		{
			get => _firstFrame;
			set => _firstFrame = value;
		}

		internal int lastFrameCount
		{
			get => _lastFrameCount;
			set => _lastFrameCount = value;
		}

		internal Matrix4x4 currentViewProjectionMatrix
		{
			get => _CurrentViewProjectionMatrix;
			set => _CurrentViewProjectionMatrix = value;
		}

		internal Matrix4x4 previousViewProjectionMatrix
		{
			get => _PreviousViewProjectionMatrix;
			set => _PreviousViewProjectionMatrix = value;
		}
	}
}
