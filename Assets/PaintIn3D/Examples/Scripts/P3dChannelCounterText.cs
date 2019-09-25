﻿using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to output the totals of all the specified pixel counters to a UI Text component.</summary>
	[RequireComponent(typeof(Text))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dChannelCounterText")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Channel Counter Text")]
	public class P3dChannelCounterText : MonoBehaviour
	{
		public enum ChannelType
		{
			Red,
			Green,
			Blue,
			Alpha
		}

		public enum OutputType
		{
			Percentage,
			Pixels
		}

		/// <summary>This allows you to choose which channel will be output to the UI Text.</summary>
		public ChannelType Channel { set { channel = value; } get { return channel; } } [SerializeField] private ChannelType channel;

		/// <summary>This allows you to choose which value will be output to the UI Text.</summary>
		public OutputType Output { set { output = value; } get { return output; } } [SerializeField] private OutputType output;

		/// <summary>This allows you to set the format of the text, where the {0} token will contain the number.</summary>
		public string Format { set { format = value; } get { return format; } } [Multiline] [SerializeField] private string format = "{0}";

		[System.NonSerialized]
		private Text cachedText;

		protected virtual void OnEnable()
		{
			cachedText = GetComponent<Text>();
		}

		protected virtual void Update()
		{
			switch (output)
			{
				case OutputType.Percentage:
				{
					var ratios = P3dChannelCounter.GetRatioRGBA();

					switch (channel)
					{
						case ChannelType.Red:   OutputRatio(ratios.x); break;
						case ChannelType.Green: OutputRatio(ratios.y); break;
						case ChannelType.Blue:  OutputRatio(ratios.z); break;
						case ChannelType.Alpha: OutputRatio(ratios.w); break;
					}
				}
				break;

				case OutputType.Pixels:
				{
					switch (channel)
					{
						case ChannelType.Red:   OutputSolid(P3dChannelCounter.GetSolidR()); break;
						case ChannelType.Green: OutputSolid(P3dChannelCounter.GetSolidG()); break;
						case ChannelType.Blue:  OutputSolid(P3dChannelCounter.GetSolidB()); break;
						case ChannelType.Alpha: OutputSolid(P3dChannelCounter.GetSolidA()); break;
					}
				}
				break;
			}
		}

		private void OutputRatio(float ratio)
		{
			var percentage = Mathf.Clamp01(ratio) * 100.0f;

			cachedText.text = string.Format(format, percentage);
		}

		private void OutputSolid(long solid)
		{
			cachedText.text = string.Format(format, solid);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dChannelCounterText))]
	public class P3dChannelCounterText_Editor : P3dEditor<P3dChannelCounterText>
	{
		protected override void OnInspector()
		{
			Draw("channel", "This allows you to choose which channel will be output to the UI Text.");
			Draw("output", "This allows you to choose which value will be output to the UI Text.");
			Draw("format", "This allows you to set the format of the text, where the {0} token will contain the number.");
		}
	}
}
#endif