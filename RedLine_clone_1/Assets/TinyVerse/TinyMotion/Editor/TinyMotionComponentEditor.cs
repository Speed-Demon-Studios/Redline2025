#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

using TinyVerse.TinyMotion;

namespace TinyVerse.TinyMotion.Editor
{
	[VolumeComponentEditor(typeof(TinyMotionComponent))]
	public class TinyMotionComponentEditor : VolumeComponentEditor
	{
		SerializedDataParameter active;
		SerializedDataParameter blurMode;
		SerializedDataParameter sampleCountMode;
		SerializedDataParameter samples;
		SerializedDataParameter maxSampleCount;
		SerializedDataParameter minSampleCount;
		SerializedDataParameter downsample;
		SerializedDataParameter skipSecondPass;
		SerializedDataParameter intensity;
		SerializedDataParameter intensityCamera;
		SerializedDataParameter threshold;
		SerializedDataParameter depthSeparationTreshold;
		SerializedDataParameter depthSeparationBothWays;
		SerializedDataParameter depthSeparationVelocity;
		SerializedDataParameter depthSeparationVelocityTreshold;
		SerializedDataParameter clampVelocity;
		SerializedDataParameter motionTextureQuality;
		SerializedDataParameter noiseMode;
		SerializedDataParameter animateNoise;
		SerializedDataParameter includeTransparentObjects;
		SerializedDataParameter shutterSpeed;
		SerializedDataParameter viewInEditor;
		SerializedDataParameter debugMode;

		GUIStyle boxStyle;
		GUIStyle labelStyle;

		public override void OnEnable()
		{
			PropertyFetcher<TinyMotionComponent> tinyMotion = new PropertyFetcher<TinyMotionComponent>(serializedObject);

			// PROPERTIES
			blurMode = Unpack(tinyMotion.Find(x => x.blurMode));
			noiseMode = Unpack(tinyMotion.Find(x => x.noiseMode));
			animateNoise = Unpack(tinyMotion.Find(x => x.animateNoise));

			sampleCountMode = Unpack(tinyMotion.Find(x => x.sampleCountMode));
			samples = Unpack(tinyMotion.Find(x => x.samples));
			minSampleCount = Unpack(tinyMotion.Find(x => x.minSampleCount));
			maxSampleCount = Unpack(tinyMotion.Find(x => x.maxSampleCount));

			downsample = Unpack(tinyMotion.Find(x => x.downsample));
			skipSecondPass = Unpack(tinyMotion.Find(x => x.skipSecondPass));
			motionTextureQuality = Unpack(tinyMotion.Find(x => x.motionTextureQuality));
			intensity = Unpack(tinyMotion.Find(x => x.intensity));
			intensityCamera = Unpack(tinyMotion.Find(x => x.intensityCamera));

			threshold = Unpack(tinyMotion.Find(x => x.threshold));
			depthSeparationTreshold = Unpack(tinyMotion.Find(x => x.depthSeparationTreshold));
			depthSeparationBothWays = Unpack(tinyMotion.Find(x => x.depthSeparationBothWays));
			depthSeparationVelocity = Unpack(tinyMotion.Find(x => x.depthSeparationVelocity));
			depthSeparationVelocityTreshold = Unpack(tinyMotion.Find(x => x.depthSeparationVelocityTreshold));
			clampVelocity = Unpack(tinyMotion.Find(x => x.clampVelocity));
			includeTransparentObjects = Unpack(tinyMotion.Find(x => x.includeTransparentObjects));
			shutterSpeed = Unpack(tinyMotion.Find(x => x.shutterSpeed));
			viewInEditor = Unpack(tinyMotion.Find(x => x.viewInEditor));
			debugMode = Unpack(tinyMotion.Find(x => x.debugMode));
		}

		public override void OnInspectorGUI()
		{
			// STYLES
			boxStyle = new GUIStyle(EditorStyles.helpBox);
			boxStyle.padding = new RectOffset(5, 5, 5, 5);
			labelStyle = new GUIStyle(EditorStyles.boldLabel);
			labelStyle.padding = new RectOffset(5, 5, 0, 0);

			if (intensity.value.floatValue == 0)
			{
				EditorGUILayout.HelpBox("To see the effect please set intensity greater than 0.", MessageType.None);
			}
			else
			{
				EditorGUILayout.HelpBox("TinyMotion Blur is Enabled and Active.", MessageType.None);
			}

			DrawQualitySettings();
			DrawAdvancedSettings();
			// DrawExperimentalSettings();
			DrawDebugSettings();

			void DrawSeprator()
			{
				EditorGUILayout.Space(2);
				EditorGUILayout.Separator();
				EditorGUILayout.Space(2);
			}

			void DrawQualitySettings()
			{
				EditorGUILayout.BeginVertical(boxStyle);
				EditorGUILayout.LabelField(new GUIContent("• Basic"), labelStyle);
				EditorGUILayout.Space(2);

				PropertyField(blurMode, new GUIContent("Blur Mode"));
				PropertyField(intensity, new GUIContent("Intensity"));
				PropertyField(intensityCamera, new GUIContent("Camera Intensity"));

				PropertyField(shutterSpeed, new GUIContent("Shutter Speed"));

				bool guiPrevEnabled = GUI.enabled;
				GUI.enabled = guiPrevEnabled && shutterSpeed.overrideState.boolValue;
				string suffix = "• 1/" + shutterSpeed.value.intValue.ToString() + "th of a second";
				Rect rect = GUILayoutUtility.GetLastRect();
				Rect suffixRect = new Rect(rect.x + 26, rect.y + rect.height * 0.85f, 150, rect.height);
				EditorGUI.LabelField(suffixRect, suffix);
				EditorGUILayout.Space(rect.height * 16);
				GUI.enabled = guiPrevEnabled;

				PropertyField(threshold, new GUIContent("Velocity Treshold"));

				DrawSeprator();

				PropertyField(noiseMode, new GUIContent("Noise Mode"));
				PropertyField(animateNoise, new GUIContent("Animate Noise"));

				PropertyField(motionTextureQuality, new GUIContent("Motion Texture Quality"));
				PropertyField(downsample, new GUIContent("Downsample"));
				if (downsample.overrideState.boolValue && downsample.value.intValue != 1)
				{
					EditorGUILayout.HelpBox("Large downsampling can cause blocky motion blur edges.", MessageType.None);
				}
				PropertyField(skipSecondPass, new GUIContent("Skip Second Pass"));

				EditorGUILayout.Space(2);
				EditorGUILayout.EndVertical();
			}

			void DrawAdvancedSettings()
			{
				if (blurMode.value.intValue >= 1)
				{
					bool guiPrevEnabled = GUI.enabled;

					EditorGUILayout.BeginVertical(boxStyle);
					EditorGUILayout.LabelField(new GUIContent("• Advanced"), labelStyle);
					EditorGUILayout.Space(2);

					PropertyField(sampleCountMode, new GUIContent("Sample Count Mode"));
					if (sampleCountMode.value.intValue == 0)
					{
						PropertyField(samples, new GUIContent("Blur Samples"));
					}
					else
					{
						PropertyField(minSampleCount, new GUIContent("Min Sample Count"));
						PropertyField(maxSampleCount, new GUIContent("Max Sample Count"));
					}

					DrawSeprator();

					bool enableDepthSeparation = blurMode.value.intValue == 2;

					GUI.enabled = guiPrevEnabled && enableDepthSeparation;
					PropertyField(depthSeparationTreshold, new GUIContent("Depth Separation Treshold"));
					PropertyField(depthSeparationBothWays, new GUIContent("Depth Separation Both Ways"));
					PropertyField(depthSeparationVelocity, new GUIContent("Depth Separation Use Velocity"));
					PropertyField(depthSeparationVelocityTreshold, new GUIContent("Depth Separation Velocity Treshold"));
					GUI.enabled = guiPrevEnabled;

					DrawSeprator();

					PropertyField(clampVelocity, new GUIContent("Clamp Velocity"));
					PropertyField(includeTransparentObjects, new GUIContent("Include Transparent Objects"));
					if (includeTransparentObjects.overrideState.boolValue && includeTransparentObjects.value.boolValue)
					{
						EditorGUILayout.HelpBox("This causes objects behind other trasparent objects not being blurred.", MessageType.Warning);
					}

					EditorGUILayout.Space(2);
					EditorGUILayout.EndVertical();
				}
			}

			// void DrawExperimentalSettings()
			// {
			// 	EditorGUILayout.BeginVertical(boxStyle);
			// 	EditorGUILayout.LabelField(new GUIContent("• Experimental"), labelStyle);
			// 	EditorGUILayout.Space(2);

			// 	EditorGUILayout.Space(2);
			// 	EditorGUILayout.EndVertical();
			// }

			void DrawDebugSettings()
			{
				EditorGUILayout.BeginVertical(boxStyle);
				EditorGUILayout.LabelField(new GUIContent("• Debug"), labelStyle);
				EditorGUILayout.Space(2);

				EditorGUILayout.HelpBox("View In Editor works only while in Playmode.", MessageType.None);
				PropertyField(viewInEditor);
				PropertyField(debugMode);
				if (debugMode.value.intValue == 6)
				{
					int samples_80p = (int)((float)maxSampleCount.value.intValue * 0.80f);
					int samples_50p = (int)((float)maxSampleCount.value.intValue * 0.50f);
					int samples_20p = (int)((float)maxSampleCount.value.intValue * 0.20f);
					EditorGUILayout.HelpBox(
						"Colors represent percentage of current Blur Samples being used for each pixel."
							+ "\nRed: > 80% ("
							+ samples_80p.ToString()
							+ ")\nGreen: > 50% ("
							+ samples_50p.ToString()
							+ ")\nBlue: > 20% ("
							+ samples_20p.ToString()
							+ ")\nWhite: 1-"
							+ samples_20p.ToString()
							+ " samples\nNo color: 0 samples",
						MessageType.None
					);
				}

				EditorGUILayout.Space(2);
				EditorGUILayout.EndVertical();
			}
		}
	}
}
#endif
