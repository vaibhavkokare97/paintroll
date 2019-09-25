using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitScreen")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Screen")]
	public class P3dHitScreen : P3dHitConnector
	{
		// This stores extra information for each finger unique to this component
		class Link
		{
			public P3dInputManager.Finger Finger;
			public float                  Distance;
		}

		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		public enum NormalType
		{
			HitNormal,
			RayDirection
		}

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>If you want the paint to continuously apply while moving the mouse, this allows you to set how many pixels are between each step (0 = no drag).</summary>
		public float Spacing { set { spacing = value; } get { return spacing; } } [SerializeField] private float spacing = 5.0f;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The layers you want the raycast to hit.</summary>
		public LayerMask Layers { set { layers = value; } get { return layers; } } [SerializeField] private LayerMask layers = Physics.DefaultRaycastLayers;

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation = OrientationType.CameraUp;

		/// <summary>Which normal should the hit point rotation be based on?</summary>
		public NormalType Normal { set { normal = value; } get { return normal; } } [SerializeField] private NormalType normal;

		/// <summary>Show a painting preview under the mouse?</summary>
		public bool ShowPreview { set { showPreview = value; } get { return showPreview; } } [SerializeField] private bool showPreview = true;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] private bool storeStates = true;

		[System.NonSerialized]
		private List<Link> links = new List<Link>();

		protected void LateUpdate()
		{
			// Use mouse hover preview?
			if (showPreview == true)
			{
				if (Input.touchCount == 0 && P3dInputManager.AnyMouseButtonSet == false && P3dInputManager.PointOverGui(Input.mousePosition) == false)
				{
					PaintAt(Input.mousePosition, true, 1.0f, this);
				}
			}

			var fingers = P3dInputManager.GetFingers();

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Index >= 0 || finger.Index == -1) // Touch or left click
				{
					Paint(finger);
				}
			}
		}

		private void Paint(P3dInputManager.Finger finger)
		{
			var link = GetLink(finger);

			if (spacing > 0.0f)
			{
				var tail = finger.SmoothPositions[0];

				if (finger.Down == true)
				{
					link.Distance = 0.0f;

					if (storeStates == true)
					{
						P3dStateManager.StoreAllStates();
					}

					PaintAt(tail, false, finger.Pressure, link);
				}

				for (var i = 1; i < finger.SmoothPositions.Count; i++)
				{
					var head  = finger.SmoothPositions[i];
					var dist  = Vector2.Distance(tail, head);
					var steps = Mathf.FloorToInt((link.Distance + dist) / spacing);

					for (var j = 0; j < steps; j++)
					{
						var remainder = spacing - link.Distance;

						tail = Vector2.MoveTowards(tail, head, remainder);

						PaintAt(tail, false, finger.Pressure, link);

						dist -= remainder;

						link.Distance = 0.0f;
					}

					link.Distance += dist;

					tail = head;
				}
			}
			else
			{
				if (showPreview == true)
				{
					if (finger.Up == true)
					{
						if (storeStates == true)
						{
							P3dStateManager.StoreAllStates();
						}

						PaintAt(finger.PositionA, false, finger.Pressure, link);
					}
					else
					{
						PaintAt(finger.PositionA, true, finger.Pressure, link);
					}
				}
				else if (finger.Down == true)
				{
					if (storeStates == true)
					{
						P3dStateManager.StoreAllStates();
					}

					PaintAt(finger.PositionA, false, finger.Pressure, link);
				}
			}

			if (finger.Up == true)
			{
				BreakHits(link);
			}
		}

		private void PaintAt(Vector2 screenPosition, bool preview, float pressure, object owner)
		{
			var camera = P3dHelper.GetCamera(_camera);

			if (camera != null)
			{
				var ray = camera.ScreenPointToRay(screenPosition);
				var hit = default(RaycastHit);

				if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layers) == true)
				{
					var finalUp       = orientation == OrientationType.CameraUp ? camera.transform.up : Vector3.up;
					var finalPosition = hit.point + hit.normal * offset;
					var finalNormal   = normal == NormalType.HitNormal ? -hit.normal : ray.direction;
					var finalRotation = Quaternion.LookRotation(finalNormal, finalUp);

					hitCache.InvokeRaycast(gameObject, null, null, preview, hit, pressure);

					DispatchHits(preview, hit.collider, finalPosition, finalRotation, pressure, owner);

					return;
				}
			}

			BreakHits(owner);
		}

		private Link GetLink(P3dInputManager.Finger finger)
		{
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				if (link.Finger == finger)
				{
					return link;
				}
			}

			var newLink = new Link();

			newLink.Finger = finger;

			links.Add(newLink);

			return newLink;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitScreen))]
	public class P3dHitScreen_Editor : P3dHitConnector_Editor<P3dHitScreen>
	{
		protected override void OnInspector()
		{
			Draw("_camera", "Orient to a specific camera?\nNone = MainCamera.");
			BeginError(Any(t => t.Layers == 0));
				Draw("layers", "The layers you want the raycast to hit.");
			EndError();
			Draw("spacing", "The time in seconds between each raycast.\n0 = every frame\n-1 = manual only");

			Separator();

			Draw("orientation", "How should the hit point be oriented?\nNone = It will be treated as a point with no rotation.\nWorldUp = It will be rotated to the normal, where the up vector is world up.\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			Draw("normal", "Which normal should the hit point rotation be based on?");
			Draw("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			Draw("showPreview", "Should the applied paint be applied as a preview?");
			Draw("storeStates", "Should painting triggered from this component be eligible for being undone?");

			base.OnInspector();

			Separator();

			Target.HitCache.Inspector(Target.gameObject, true, true, Target.ConnectHits);
		}
	}
}
#endif