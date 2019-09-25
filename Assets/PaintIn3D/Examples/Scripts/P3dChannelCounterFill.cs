﻿using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component fills the attached UI Image based on the total amount of opaque pixels that have been painted in all active and enabled <b>P3dChannelCounter</b> components in the scene.</summary>
	[RequireComponent(typeof(Image))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dChannelCounterFill")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Channel Counter Fill")]
	public class P3dChannelCounterFill : MonoBehaviour
	{
		public enum ChannelType
		{
			Red,
			Green,
			Blue,
			Alpha
		}

		/// <summary>This allows you to choose which channel will be output to the UI Image.</summary>
		public ChannelType Channel { set { channel = value; } get { return channel; } } [SerializeField] private ChannelType channel;

		public Image cachedImage;

		protected virtual void OnEnable()
		{
			cachedImage = GetComponent<Image>();
		}

		protected virtual void FixedUpdate()
		{
			var ratioRGBA = P3dChannelCounter.GetRatioRGBA();
			var amount    = 0.0f;

			switch (channel)
			{
				case ChannelType.Red:   amount = ratioRGBA.x; break;
				case ChannelType.Green: amount = ratioRGBA.y; break;
				case ChannelType.Blue:  amount = ratioRGBA.z; break;
				case ChannelType.Alpha: amount = ratioRGBA.w; break;
			}

			cachedImage.fillAmount = Mathf.Clamp01(amount);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dChannelCounterFill))]
	public class P3dChannelCounterFill_Editor : P3dEditor<P3dChannelCounterFill>
	{
		protected override void OnInspector()
		{
			Draw("channel", "This allows you to choose which channel will be output to the UI Image.");
		}
	}
}
#endif