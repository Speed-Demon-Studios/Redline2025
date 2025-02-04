using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyVerse.Common
{
	public class DestroyIfNotVisible : MonoBehaviour
	{
		public float NotVisibleTimeout = 60f;

		private Renderer _renderer;
		private float _isNotVisibleCounter = 0;

		void Start()
		{
			if (null == _renderer)
			{
				GetLargestRenderer();
			}
		}

		void GetLargestRenderer()
		{
			Vector3 _largestExtents = Vector3.zero;
			MeshFilter[] _meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
			MeshFilter _largestMeshFilter = null;
			MeshFilter _meshFilter;
			//
			for (int i = 0; i < _meshFilters.Length; i++)
			{
				_meshFilter = _meshFilters[i];
				if (_meshFilter.mesh.bounds.extents.sqrMagnitude > _largestExtents.sqrMagnitude)
				{
					_largestExtents = _meshFilter.mesh.bounds.extents;
					_largestMeshFilter = _meshFilter;
				}
			}
			//
			if (null != _largestMeshFilter)
			{
				_renderer = _largestMeshFilter.GetComponent<Renderer>();
			}
		}

		void Update()
		{
			if (null != _renderer)
			{
				if (!_renderer.isVisible)
				{
					if (_isNotVisibleCounter > NotVisibleTimeout)
					{
						Debug.Log($"Destroy invisible renderer: {this.name}");
						Destroy(gameObject);
					}
					else
					{
						_isNotVisibleCounter += Time.deltaTime;
					}
				}
				else
				{
					_isNotVisibleCounter = 0f;
				}
			}
		}
	}
}
