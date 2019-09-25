using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component continuously fires hit events using the current Transform position.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitNearby")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Nearby")]
	public class P3dHitNearby : P3dHitConnector
	{
		/// <summary>The time in seconds between each raycast.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.05f;

		/// <summary>Should the applied paint be applied as a preview?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.</summary>
		public float Pressure { set { pressure = value; } get { return pressure; } } [Range(0.0f, 1.0f)] [SerializeField] private float pressure = 1.0f;

		[System.NonSerialized]
		private float current;

		protected virtual void FixedUpdate()
		{
			if (delay > 0.0f)
			{
				current += Time.fixedDeltaTime;

				if (current >= delay)
				{
					current %= delay;

					DispatchHits(preview, null, transform.position, transform.rotation, pressure, this);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitNearby))]
	public class P3dHitNearby_Editor : P3dHitConnector_Editor<P3dHitNearby>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Delay <= 0.0f));
				Draw("delay", "The time in seconds between each raycast.");
			EndError();
			Draw("preview", "Should the applied paint be applied as a preview?");
			Draw("pressure", "This allows you to control the pressure of the painting. This could be controlled by a VR trigger or similar for more advanced effects.");

			base.OnInspector();

			Separator();

			Target.HitCache.Inspector(Target.gameObject, false, true, Target.ConnectHits);
		}
	}
}
#endif