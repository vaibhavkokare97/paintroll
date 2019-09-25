using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component will search the specified paintable texture for pixel colors matching an active and enabled P3dColor.</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dColorCounter")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Color Counter")]
	public class P3dColorCounter : P3dPaintableTextureMonitorMask
	{
		class TempColor
		{
			public byte R;
			public byte G;
			public byte B;
			public byte A;
			public int  Solid;
		}

		/// <summary>This stores all active and enabled instances.</summary>
		public static LinkedList<P3dColorCounter> Instances = new LinkedList<P3dColorCounter>(); private LinkedListNode<P3dColorCounter> node;

		/// <summary>Counting all the pixels of a texture can be slow, so you can pick how many times the texture is downsampled before it gets counted. One downsample = half width & height or 1/4 of the pixels. NOTE: The pixel totals will be multiplied to account for this downsampling.</summary>
		public int DownsampleSteps { set { downsampleSteps = value; } get { return downsampleSteps; } } [SerializeField] private int downsampleSteps = 3;

		/// <summary>The RGBA values must be within this range of a color for it to be counted.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [Range(0.0f, 1.0f)] [SerializeField] private float threshold = 0.5f;

		[System.NonSerialized]
		private static List<TempColor> tempColors = new List<TempColor>();

		protected override void OnEnable()
		{
			base.OnEnable();

			node = Instances.AddLast(this);
		}

		protected override void OnDisable()
		{
			base.OnEnable();

			Instances.Remove(node); node = null;
		}

		protected override void UpdateMonitor(P3dPaintableTexture paintableTexture, bool preview)
		{
			if (preview == false && paintableTexture.Activated == true)
			{
				var renderTexture = paintableTexture.Current;
				var temporary     = default(RenderTexture);

				if (P3dHelper.Downsample(renderTexture, downsampleSteps, ref temporary) == true)
				{
					Calculate(temporary, 1 << downsampleSteps);

					P3dHelper.ReleaseRenderTexture(temporary);
				}
				else
				{
					Calculate(renderTexture, 1);
				}
			}
		}

		private void Calculate(RenderTexture renderTexture, int scale)
		{
			var threshold32 = (int)(threshold * 255.0f);
			var width       = renderTexture.width;
			var height      = renderTexture.height;
			var texture2D   = P3dHelper.GetReadableCopy(renderTexture);
			var pixels32    = texture2D.GetPixels32();

			P3dHelper.Destroy(texture2D);

			UpdateTotal(renderTexture, width, height, scale);

			PrepareTemp();

			for (var y = 0; y < height; y++)
			{
				var offset = y * width;

				for (var x = 0; x < height; x++)
				{
					var index = offset + x;

					if (baked == true && bakedPixels[index] == false)
					{
						continue;
					}

					var pixel32      = pixels32[index];
					var bestIndex    = -1;
					var bestDistance = threshold32;

					for (var i = 0; i < P3dColor.InstanceCount; i++)
					{
						var tempColor = tempColors[i];
						var distance  = 0;

						distance += System.Math.Abs(tempColor.R - pixel32.r);
						distance += System.Math.Abs(tempColor.G - pixel32.g);
						distance += System.Math.Abs(tempColor.B - pixel32.b);
						distance += System.Math.Abs(tempColor.A - pixel32.a);

						if (distance <= bestDistance)
						{
							bestIndex    = i;
							bestDistance = distance;
						}
					}

					if (bestIndex >= 0)
					{
						tempColors[bestIndex].Solid++;
					}
				}
			}

			// Multiply totals to account for downsampling
			Contribute(scale);
		}

		private void PrepareTemp()
		{
			var color = P3dColor.FirstInstance;

			tempColors.Clear();

			for (var i = tempColors.Count; i < P3dColor.InstanceCount; i++)
			{
				tempColors.Add(new TempColor());
			}

			for (var i = 0; i < P3dColor.InstanceCount; i++)
			{
				var tempColor   = tempColors[i];
				var tempColor32 = (Color32)color.Color;

				tempColor.R     = tempColor32.r;
				tempColor.G     = tempColor32.g;
				tempColor.B     = tempColor32.b;
				tempColor.A     = tempColor32.a;
				tempColor.Solid = 0;

				color = color.NextInstance;
			}
		}

		private void Contribute(int scale)
		{
			var color = P3dColor.FirstInstance;

			for (var i = 0; i < P3dColor.InstanceCount; i++)
			{
				color.Contribute(this, tempColors[i].Solid * scale);

				color = color.NextInstance;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CustomEditor(typeof(P3dColorCounter))]
	public class P3dColorCounter_Editor : P3dPaintableTextureMonitorMask_Editor<P3dColorCounter>
	{
		protected override void OnInspector()
		{
			base.OnInspector();

			BeginError(Any(t => t.DownsampleSteps < 0));
				Draw("downsampleSteps", "Counting all the pixels of a texture can be slow, so you can pick how many times the texture is downsampled before it gets counted. One downsample = half width & height or 1/4 of the pixels. NOTE: The pixel totals will be multiplied to account for this downsampling.");
			EndError();
			Draw("threshold", "The RGBA values must be within this range of a color for it to be counted.");
		}
	}
}
#endif