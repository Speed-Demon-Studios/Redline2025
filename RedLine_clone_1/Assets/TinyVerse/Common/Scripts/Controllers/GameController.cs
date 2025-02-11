using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace TinyVerse.Common
{
	public class GameController : Singleton<GameController>
	{
		// Prevent non-singleton constructor use.
		protected GameController() { }

		public bool manageSplitScreen = false;

		public List<PlayerController> players = new List<PlayerController>();
#if ENABLE_INPUT_SYSTEM
		public List<PlayerInput> playerInputs = new List<PlayerInput>();
#endif

		protected override void OnAwake()
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;
		}

#if ENABLE_INPUT_SYSTEM
		public void OnPlayerJoined(PlayerInput playerInput)
		{
			playerInputs.Add(playerInput);
		}
#endif

#if ENABLE_INPUT_SYSTEM
		public void OnPlayerLeft(PlayerInput playerInput)
		{
			playerInputs.Remove(playerInput);
		}
#endif

		public void AddPlayer(PlayerController _pc)
		{
			if (!players.Contains(_pc))
			{
				players.Add(_pc);
				if (manageSplitScreen)
				{
					UpdateCameras();
				}
			}
		}

		public void RemovePlayer(PlayerController _pc)
		{
			if (players.Contains(_pc))
			{
				players.Remove(_pc);
				if (manageSplitScreen)
				{
					UpdateCameras();
				}
			}
		}

		public void UpdateCameras()
		{
			//
			Rect[] rects = new Rect[4];
			rects[0] = new Rect(0, 0, 1f, 1f);
			//
			if (players.Count == 2)
			{
				rects[0] = new Rect(0f, 0f, 0.5f, 1f);
				rects[1] = new Rect(0.5f, 0f, 0.5f, 1f);
			}
			else if (players.Count == 3)
			{
				rects[0] = new Rect(0f, 0.5f, 0.5f, 0.5f);
				rects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
				rects[2] = new Rect(0.0f, 0f, 1f, 0.5f);
			}
			else if (players.Count == 4)
			{
				rects[0] = new Rect(0f, 0.5f, 0.5f, 0.5f);
				rects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
				rects[2] = new Rect(0f, 0f, 0.5f, 0.5f);
				rects[3] = new Rect(0f, 0.5f, 0.5f, 0.5f);
			}

			for (int i = 0; i < players.Count; i++)
			{
				players[i].playerCamera.rect = rects[i];
			}

			if (null != Camera.main)
			{
				if (players.Count > 0)
				{
					Camera.main.gameObject.SetActive(false);
				}
				else
				{
					Camera.main.gameObject.SetActive(true);
				}
			}
		}
	}
}
