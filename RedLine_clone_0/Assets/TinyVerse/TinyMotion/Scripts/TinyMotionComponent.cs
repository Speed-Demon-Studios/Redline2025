using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TinyVerse.TinyMotion
{
	public enum BlurMode
	{
		Simple,
		Advanced,
		AdvancedWithDepthSeparation,
	}

	[Serializable]
	public sealed class BlurModeParameter : VolumeParameter<BlurMode>
	{
		public BlurModeParameter(BlurMode value, bool overrideState = false)
			: base(value, overrideState) { }
	}

	public enum SampleCountMode
	{
		Fixed,
		Dynamic
	}

	[Serializable]
	public sealed class SampleCountModeParameter : VolumeParameter<SampleCountMode>
	{
		public SampleCountModeParameter(SampleCountMode value, bool overrideState = false)
			: base(value, overrideState) { }
	}

	public enum NoiseMode
	{
		InterleavedGradient,
		PseudoRandom,
		None
	}

	[Serializable]
	public sealed class NoiseModeParameter : VolumeParameter<NoiseMode>
	{
		public NoiseModeParameter(NoiseMode value, bool overrideState = false)
			: base(value, overrideState) { }
	}

	public enum MotionTextureQuality
	{
		High, // RGFloat
		Medium, // RGHalf
		Low // RG16
	}

	[Serializable]
	public sealed class MotionTextureQualityParameter : VolumeParameter<MotionTextureQuality>
	{
		public MotionTextureQualityParameter(MotionTextureQuality value, bool overrideState = false)
			: base(value, overrideState) { }
	}

	public enum Downsample
	{
		None = 1,
		x2 = 2,
		x4 = 4,
		x6 = 6,
		x8 = 8,
	}

	[Serializable]
	public sealed class DownsampleParameter : VolumeParameter<Downsample>
	{
		public DownsampleParameter(Downsample value, bool overrideState = false)
			: base(value, overrideState) { }
	}

	public enum DebugMode
	{
		None,
		Velocity,
		VelocityBW,
		VectorMotionTexture,
		NoiseTexture,
		DepthTexture,
		DynamicSampleCount,
	}

	[Serializable]
	public sealed class DebugModeParameter : VolumeParameter<DebugMode>
	{
		public DebugModeParameter(DebugMode value, bool overrideState = false)
			: base(value, overrideState) { }
	}

	[Serializable, VolumeComponentMenu("TinyMotion")]
	public sealed class TinyMotionComponent : VolumeComponent, IPostProcessComponent
	{
		/// <summary>
		/// Toggle between Simple/Advanced motion blur.
		/// </summary>
		[Tooltip(
			"Toggle between Simple/Advanced motion blur.\n- Simple: Fixed number of samples, advanced blur effects disabled\n- Advanced: Variable number of samples, all features of motion blur configurable (this is more demanding)"
		)]
		public BlurModeParameter blurMode = new BlurModeParameter(BlurMode.Advanced);

		/// <summary>
		/// Toggle between Fixed/Dynamic sample count.
		/// - Fixed: Fixed number of samples set by Blur Samples parametter
		/// - Dynamic: Variable number of samples calculated automatically from the velocity, capped with Min and Max Blur Samples
		/// </summary>
		[Tooltip(
			"Toggle between Fixed/Dynamic sample count.\n- Fixed: Fixed number of samples set by Blur Samples parametter\n- Dynamic: Variable number of samples calculated automatically from the velocity, capped with Min and Max Blur Samples"
		)]
		public SampleCountModeParameter sampleCountMode = new SampleCountModeParameter(SampleCountMode.Dynamic);

		/// <summary>
		/// Number of samples for motion blur. Choose lower samples for better performace.
		/// </summary>
		[Tooltip("Number of samples for motion blur. Choose lower samples for better performace.")]
		public ClampedIntParameter samples = new ClampedIntParameter(6, 1, 24);

		/// <summary>
		/// Minimum Sample Count used for Dynamic Sample Count Mode.
		/// </summary>
		[Tooltip("Minimum Sample Count used for Dynamic Sample Count Mode.")]
		public ClampedIntParameter minSampleCount = new ClampedIntParameter(2, 0, 24);

		/// <summary>
		/// Maximum Sample Count used for Dynamic Sample Count Mode.
		/// </summary>
		[Tooltip("Maximum Sample Count used for Dynamic Sample Count Mode.")]
		public ClampedIntParameter maxSampleCount = new ClampedIntParameter(6, 2, 24);

		/// <summary>
		/// Choose texture format for storing motion blur data.
		/// High: RGFloat
		/// Medium: RGHalf
		/// Low: RG16
		/// Documentation: https://docs.unity3d.com/ScriptReference/RenderTextureFormat.html
		/// </summary>
		[Tooltip("Choose texture format for storing motion blur data.\nHigh: RGFloat\nMedium: RGHalf\nLow: RG16")]
		public MotionTextureQualityParameter motionTextureQuality = new MotionTextureQualityParameter(MotionTextureQuality.Medium);

		/// <summary>
		/// Downsaples motion texture.
		/// </summary>
		[Tooltip("Downsaples motion texture.")]
		public DownsampleParameter downsample = new DownsampleParameter(Downsample.None);

		/// <summary>
		/// Select noise used to dither motion vectors.
		/// </summary>
		[Tooltip("Select noise used to dither motion vectors.")]
		public NoiseModeParameter noiseMode = new NoiseModeParameter(NoiseMode.InterleavedGradient);

		/// <summary>
		/// Animates noise with time.
		/// </summary>
		[Tooltip("Animates noise with time.")]
		public BoolParameter animateNoise = new BoolParameter(false);

		/// <summary>
		/// Skips second blur pass.\nBetter performace at the cost of lower quality.
		/// </summary>
		[Tooltip("Skips second blur pass.\nBetter performace at the cost of lower quality.")]
		public BoolParameter skipSecondPass = new BoolParameter(false);

		/// <summary>
		/// Overall intensity multiplier of the effect.
		/// </summary>
		[Tooltip("Overall intensity multiplier of the effect.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 2f);

		/// <summary>
		/// Intensity of Camera motion.
		/// </summary>
		[Tooltip("Intensity of Camera motion.")]
		public ClampedFloatParameter intensityCamera = new ClampedFloatParameter(0f, 0f, 2f);

		/// <summary>
		/// Shutter speed (fraction of a second) for motion blur calculation, larger values = less motion blur.
		/// High intensity value and low shutter speed results in overblurred image.
		/// Recommended values:
		/// Intensity 1
		/// Shutter speed: 250
		/// </summary>
		[Tooltip(
			"Shutter speed (fraction of a second) for motion blur calculation, larger values = less motion blur.\nHigh intensity value and low shutter speed results in overblurred image.\n\nRecommended values:\nIntensity 1\nShutter speed: 250"
		)]
		public ClampedIntParameter shutterSpeed = new ClampedIntParameter(250, 15, 1000);

		/// <summary>
		/// Minimum velocity treshold for the motion blur to activate.
		/// </summary>
		[Tooltip("Minimum velocity treshold for the motion blur to activate.")]
		public ClampedFloatParameter threshold = new ClampedFloatParameter(0.001f, 0f, 1f);

		/// <summary>
		/// Minimum distance between two pixels depth.
		/// </summary>
		[Tooltip("Minimum distance between two pixels depth.")]
		public ClampedFloatParameter depthSeparationTreshold = new ClampedFloatParameter(0f, 0f, 1f);

		/// <summary>
		/// Separate motion vector in both ways.\nWhen enabled depth is separated in forward and backward direction.
		/// </summary>
		[Tooltip("Separate motion vector in both ways.\nWhen enabled depth is separated in forward and backward direction.")]
		public BoolParameter depthSeparationBothWays = new BoolParameter(false);

		/// <summary>
		/// Use velocity as a factor when computing Depth Separation?
		/// </summary>
		[Tooltip("Use velocity as a factor when computing Depth Separation?")]
		public BoolParameter depthSeparationVelocity = new BoolParameter(false);

		/// <summary>
		/// Maximum velocity difference between two pixels to enable depth separation.
		/// </summary>
		[Tooltip("Maximum velocity difference between two pixels to enable depth separation.")]
		public ClampedFloatParameter depthSeparationVelocityTreshold = new ClampedFloatParameter(0.25f, 0f, 1f);

		/// <summary>
		/// Clamp maximum velocity to prevent overbluring.
		/// </summary>
		[Tooltip("Clamp maximum velocity to prevent overbluring.")]
		public ClampedFloatParameter clampVelocity = new ClampedFloatParameter(1f, 0f, 1f);

		/// <summary>
		/// Render motion blur in editor view.
		/// Works only in Playmode.
		/// </summary>
		[Tooltip("Render motion blur in editor view.\nWorks only in Playmode.")]
		public BoolParameter viewInEditor = new BoolParameter(false);

		/// <summary>
		/// Include transparent objects when computing motion vectors.
		/// This currently causes objects behind other trasparent objects not being blurred.
		/// </summary>
		[Tooltip("Include transparent objects when computing motion vectors\nThis currently causes objects behind other trasparent objects not being blurred.")]
		public BoolParameter includeTransparentObjects = new BoolParameter(false);

		/// <summary>
		/// Visualize motion vectors.
		/// </summary>
		[Tooltip("Visualize motion vectors.")]
		public DebugModeParameter debugMode = new DebugModeParameter(DebugMode.None);

		/// <summary>
		/// Is the component active?
		/// </summary>
		/// <returns>True is the component active</returns>
		public bool IsActive() => active && intensity.value > 0f;

		/// <summary>
		/// Is the component compatible with on tile rendering
		/// </summary>
		/// <returns>false</returns>
		public bool IsTileCompatible() => false;
	}
}
