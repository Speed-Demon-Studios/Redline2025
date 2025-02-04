using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

using TMPro;
using TinyVerse.Common;

namespace TinyVerse.TinyMotion.Demo
{
	public class Configurator : MonoBehaviour
	{
		public Volume volumeProfile;
		public Button sidePanelToggle;
		public Transform sidePanel;
		public Transform fixedSampleCountWrap;
		public Transform dynamicSampleCountWrap;

		[Header("Basic Options")]
		public Slider samplesSlider;
		public TextMeshProUGUI samplesValue;
		public Slider intensitySlider;
		public TextMeshProUGUI intensityValue;
		public Slider shutterSpeedSlider;
		public TextMeshProUGUI shutterSpeedValue;
		public Slider clampVelocitySlider;
		public TextMeshProUGUI clampVelocityValue;

		public Toggle toggleBlurModeSimple;
		public Toggle toggleBlurModeAdvanced;
		public Toggle toggleBlurModeAdvancedDepth;

		public Toggle toggleTextureQualityLow;
		public Toggle toggleTextureQualityMedium;
		public Toggle toggleTextureQualityHigh;

		public Toggle toggleDownsampleNone;
		public Toggle toggleDownsample2x;
		public Toggle toggleDownsample4x;
		public Toggle toggleDownsample6x;
		public Toggle toggleDownsample8x;

		[Header("Dynamic Samples")]
		public Toggle toggleFixedSamples;
		public Toggle toggleDynamicSamples;
		public Slider minSamplesSlider;
		public TextMeshProUGUI minSamplesValue;
		public Slider maxSamplesSlider;
		public TextMeshProUGUI maxSamplesValue;

		[Header("Depth Separation")]
		public Slider depthSeparationTresholdSlider;
		public TextMeshProUGUI depthSeparationTresholdValue;
		public Slider depthSeparationVelocityTresholdSlider;
		public TextMeshProUGUI depthSeparationVelocityTresholdValue;
		public Toggle toggleDepthSeparationVelocity;

		[Header("Debug Modes")]
		public Toggle toggleDebugModeNone;
		public Toggle toggleDebugModeVelocity;
		public Toggle toggleDebugModeVelocityBW;
		public Toggle toggleDebugModeNoiseTexture;
		public Toggle toggleDebugModeDynamicSampleCount;

		TinyMotionComponent _tinyMotion;
		bool _sidePanelOpen = true;
		float _animationTime = 0f;
		float _animationSpeed = 10f;

		private void Start()
		{

			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 120;

			volumeProfile.sharedProfile.TryGet<TinyMotionComponent>(out _tinyMotion);

			samplesSlider.onValueChanged.AddListener(
				delegate
				{
					SetMotionBlurSamples();
				}
			);
			intensitySlider.onValueChanged.AddListener(
				delegate
				{
					SetMotionBlurIntensity();
				}
			);
			shutterSpeedSlider.onValueChanged.AddListener(
				delegate
				{
					SetMotionBlurShutterSpeed();
				}
			);
			clampVelocitySlider.onValueChanged.AddListener(
				delegate
				{
					SetMotionBlurClamping();
				}
			);

			maxSamplesSlider.onValueChanged.AddListener(
				delegate
				{
					SetMotionBlurMaxSamples();
				}
			);
			minSamplesSlider.onValueChanged.AddListener(
				delegate
				{
					SetMotionBlurMinSamples();
				}
			);

			depthSeparationTresholdSlider.onValueChanged.AddListener(
				delegate
				{
					SetDepthSeparationTreshold();
				}
			);
			depthSeparationVelocityTresholdSlider.onValueChanged.AddListener(
				delegate
				{
					SetDepthSeparationVelocityTreshold();
				}
			);

			SetMotionBlurSamples();
			SetMotionBlurMinSamples();
			SetMotionBlurMaxSamples();
			SetMotionBlurIntensity();
			SetMotionBlurShutterSpeed();
			SetMotionBlurClamping();

			toggleBlurModeSimple.onValueChanged.AddListener(
				delegate
				{
					if (toggleBlurModeSimple.isOn)
						SetBlurMode(1);
				}
			);
			toggleBlurModeAdvanced.onValueChanged.AddListener(
				delegate
				{
					if (toggleBlurModeAdvanced.isOn)
						SetBlurMode(2);
				}
			);
			toggleBlurModeAdvancedDepth.onValueChanged.AddListener(
				delegate
				{
					if (toggleBlurModeAdvancedDepth.isOn)
						SetBlurMode(3);
				}
			);

			toggleTextureQualityLow.onValueChanged.AddListener(
				delegate
				{
					if (toggleTextureQualityLow.isOn)
						SetMotionTextureQuality(1);
				}
			);
			toggleTextureQualityMedium.onValueChanged.AddListener(
				delegate
				{
					if (toggleTextureQualityMedium.isOn)
						SetMotionTextureQuality(2);
				}
			);
			toggleTextureQualityHigh.onValueChanged.AddListener(
				delegate
				{
					if (toggleTextureQualityHigh.isOn)
						SetMotionTextureQuality(3);
				}
			);

			toggleDownsampleNone.onValueChanged.AddListener(
				delegate
				{
					if (toggleDownsampleNone.isOn)
						SetMotionTextureDownsample(0);
				}
			);
			toggleDownsample2x.onValueChanged.AddListener(
				delegate
				{
					if (toggleDownsample2x.isOn)
						SetMotionTextureDownsample(2);
				}
			);
			toggleDownsample4x.onValueChanged.AddListener(
				delegate
				{
					if (toggleDownsample4x.isOn)
						SetMotionTextureDownsample(4);
				}
			);
			toggleDownsample6x.onValueChanged.AddListener(
				delegate
				{
					if (toggleDownsample6x.isOn)
						SetMotionTextureDownsample(6);
				}
			);
			toggleDownsample8x.onValueChanged.AddListener(
				delegate
				{
					if (toggleDownsample8x.isOn)
						SetMotionTextureDownsample(8);
				}
			);

			toggleDepthSeparationVelocity.onValueChanged.AddListener(
				delegate
				{
					SetDepthSeparationVelocityState(toggleDepthSeparationVelocity.isOn);
				}
			);
			toggleFixedSamples.onValueChanged.AddListener(
				delegate
				{
					if (toggleFixedSamples.isOn)
						SetSamplecountMode(1);
				}
			);
			toggleDynamicSamples.onValueChanged.AddListener(
				delegate
				{
					if (toggleDynamicSamples.isOn)
						SetSamplecountMode(2);
				}
			);

			toggleDebugModeNone.onValueChanged.AddListener(
				delegate
				{
					if (toggleDebugModeNone.isOn)
						SetDebugMode(0);
				}
			);
			toggleDebugModeVelocity.onValueChanged.AddListener(
				delegate
				{
					if (toggleDebugModeVelocity.isOn)
						SetDebugMode(1);
				}
			);
			toggleDebugModeVelocityBW.onValueChanged.AddListener(
				delegate
				{
					if (toggleDebugModeVelocityBW.isOn)
						SetDebugMode(2);
				}
			);
			toggleDebugModeNoiseTexture.onValueChanged.AddListener(
				delegate
				{
					if (toggleDebugModeNoiseTexture.isOn)
						SetDebugMode(4);
				}
			);
			toggleDebugModeDynamicSampleCount.onValueChanged.AddListener(
				delegate
				{
					if (toggleDebugModeDynamicSampleCount.isOn)
						SetDebugMode(5);
				}
			);

			SetMotionTextureQuality(0);
			SetMotionTextureDownsample(0);
			SetDepthSeparationVelocityState(toggleDepthSeparationVelocity.isOn);
			SetSamplecountMode(0);
			SetDebugMode(0);
			SetBlurMode(0);

			sidePanelToggle.onClick.AddListener(
				delegate
				{
					TogglePanel();
				}
			);

			_sidePanelOpen = false;
			_animationTime = 0f;
		}

		private void FixedUpdate()
		{
			AnimatePanel();
		}

		void AnimatePanel()
		{
			if (!_sidePanelOpen && !Mathf.Approximately(sidePanel.localScale.x, 0))
			{
				_animationTime += Time.fixedDeltaTime * _animationSpeed;
				Vector3 scale = sidePanel.localScale;
				scale.x = Mathf.Lerp(scale.x, 0, _animationTime.EaseSine());
				sidePanel.localScale = scale;
			}
			else if (_sidePanelOpen && !Mathf.Approximately(sidePanel.localScale.x, 1))
			{
				_animationTime += Time.fixedDeltaTime * _animationSpeed;
				Vector3 scale = sidePanel.localScale;
				scale.x = Mathf.Lerp(scale.x, 1, _animationTime.EaseSine());
				sidePanel.localScale = scale;
			}
			else
			{
				_animationTime = 0f;
			}
		}

		// TODO: Add check if current platform supports selected texture quality
		public void SetBlurMode(int blurModeValue)
		{
			switch (blurModeValue)
			{
				case 1:
					_tinyMotion.blurMode.SetValue(new BlurModeParameter(BlurMode.Simple));
					break;
				case 2:
					_tinyMotion.blurMode.SetValue(new BlurModeParameter(BlurMode.Advanced));
					break;
				case 3:
					_tinyMotion.blurMode.SetValue(new BlurModeParameter(BlurMode.AdvancedWithDepthSeparation));
					break;
				default:
					_tinyMotion.blurMode.SetValue(new BlurModeParameter(BlurMode.Advanced));
					break;
			}
		}

		public void SetMotionTextureQuality(int qualityValue)
		{
			switch (qualityValue)
			{
				case 1:
					_tinyMotion.motionTextureQuality.SetValue(new MotionTextureQualityParameter(MotionTextureQuality.Low));
					break;
				case 2:
					_tinyMotion.motionTextureQuality.SetValue(new MotionTextureQualityParameter(MotionTextureQuality.Medium));
					break;
				case 3:
					_tinyMotion.motionTextureQuality.SetValue(new MotionTextureQualityParameter(MotionTextureQuality.High));
					break;
				default:
					_tinyMotion.motionTextureQuality.SetValue(new MotionTextureQualityParameter(MotionTextureQuality.Medium));
					break;
			}
		}

		public void SetMotionTextureDownsample(int downsampleValue)
		{
			switch (downsampleValue)
			{
				case 2:
					_tinyMotion.downsample.SetValue(new DownsampleParameter(Downsample.x2));
					break;
				case 4:
					_tinyMotion.downsample.SetValue(new DownsampleParameter(Downsample.x4));
					break;
				case 6:
					_tinyMotion.downsample.SetValue(new DownsampleParameter(Downsample.x6));
					break;
				case 8:
					_tinyMotion.downsample.SetValue(new DownsampleParameter(Downsample.x8));
					break;
				default:
					_tinyMotion.downsample.SetValue(new DownsampleParameter(Downsample.None));
					break;
			}
		}

		public void SetDebugMode(int debugModeIntValue)
		{
			switch (debugModeIntValue)
			{
				case 1:
					_tinyMotion.debugMode.SetValue(new DebugModeParameter(DebugMode.Velocity));
					break;
				case 2:
					_tinyMotion.debugMode.SetValue(new DebugModeParameter(DebugMode.VelocityBW));
					break;
				case 3:
					_tinyMotion.debugMode.SetValue(new DebugModeParameter(DebugMode.VectorMotionTexture));
					break;
				case 4:
					_tinyMotion.debugMode.SetValue(new DebugModeParameter(DebugMode.NoiseTexture));
					break;
				case 5:
					_tinyMotion.debugMode.SetValue(new DebugModeParameter(DebugMode.DynamicSampleCount));
					break;
				default:
					_tinyMotion.debugMode.SetValue(new DebugModeParameter(DebugMode.None));
					break;
			}
		}

		public void SetSamplecountMode(int sampleCountModeIntValue)
		{
			switch (sampleCountModeIntValue)
			{
				case 1:
					_tinyMotion.sampleCountMode.SetValue(new SampleCountModeParameter(SampleCountMode.Fixed));
					fixedSampleCountWrap.gameObject.SetActive(true);
					dynamicSampleCountWrap.gameObject.SetActive(false);
					break;
				case 2:
					_tinyMotion.sampleCountMode.SetValue(new SampleCountModeParameter(SampleCountMode.Dynamic));
					fixedSampleCountWrap.gameObject.SetActive(false);
					dynamicSampleCountWrap.gameObject.SetActive(true);
					break;
				default:
					_tinyMotion.sampleCountMode.SetValue(new SampleCountModeParameter(SampleCountMode.Dynamic));
					fixedSampleCountWrap.gameObject.SetActive(false);
					dynamicSampleCountWrap.gameObject.SetActive(true);
					break;
			}
		}

		public void SetMotionBlurSamples()
		{
			_tinyMotion.samples.SetValue(new ClampedIntParameter((int)samplesSlider.value, 1, 12, true));
			samplesValue.text = samplesSlider.value.ToString();
		}

		public void SetMotionBlurMinSamples()
		{
			_tinyMotion.minSampleCount.SetValue(new ClampedIntParameter((int)minSamplesSlider.value, 1, 12, true));
			minSamplesValue.text = minSamplesSlider.value.ToString();
		}

		public void SetMotionBlurMaxSamples()
		{
			_tinyMotion.maxSampleCount.SetValue(new ClampedIntParameter((int)maxSamplesSlider.value, 1, 12, true));
			maxSamplesValue.text = maxSamplesSlider.value.ToString();
		}

		public void SetMotionBlurIntensity()
		{
			_tinyMotion.intensity.SetValue(new ClampedFloatParameter(intensitySlider.value, 0f, 2f, true));
			intensityValue.text = intensitySlider.value.ToString("F");
		}

		public void SetMotionBlurShutterSpeed()
		{
			_tinyMotion.shutterSpeed.SetValue(new ClampedIntParameter((int)shutterSpeedSlider.value, 15, 1000, true));
			shutterSpeedValue.text = shutterSpeedSlider.value.ToString();
		}

		public void SetMotionBlurClamping()
		{
			_tinyMotion.clampVelocity.SetValue(new ClampedFloatParameter(clampVelocitySlider.value, 0f, 1f, true));
			clampVelocityValue.text = clampVelocitySlider.value.ToString("F");
		}

		public void SetDepthSeparationTreshold()
		{
			_tinyMotion.depthSeparationTreshold.SetValue(new ClampedFloatParameter(depthSeparationTresholdSlider.value, 0f, 1f, true));
			depthSeparationTresholdValue.text = depthSeparationTresholdSlider.value.ToString("F");
		}

		public void SetDepthSeparationVelocityTreshold()
		{
			_tinyMotion.depthSeparationVelocityTreshold.SetValue(new ClampedFloatParameter(depthSeparationVelocityTresholdSlider.value, 0f, 1f, true));
			depthSeparationVelocityTresholdValue.text = depthSeparationVelocityTresholdSlider.value.ToString("F");
		}

		public void SetDepthSeparationVelocityState(bool state)
		{
			_tinyMotion.depthSeparationVelocity.SetValue(new BoolParameter(state));
		}

		public void TogglePanel()
		{
			_sidePanelOpen = !_sidePanelOpen;
			_animationTime = 0f;
		}
	}
}
