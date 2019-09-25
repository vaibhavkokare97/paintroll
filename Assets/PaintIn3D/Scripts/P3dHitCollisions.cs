﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be added to any Rigidbody, and it will fire hit events when it hits something.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitCollisions")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Collisions")]
	public class P3dHitCollisions : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation;

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The relative speed required for a paint to occur.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [SerializeField] private float threshold = 1.0f;

		/// <summary>If there are multiple contact points, skip them?</summary>
		public bool OnlyUseFirstContact { set { onlyUseFirstContact = value; } get { return onlyUseFirstContact; } } [SerializeField] private bool onlyUseFirstContact = true;

		/// <summary>The time in seconds between each collision if you want to limit it.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay;

		/// <summary>If you need raycast information (used by components like P3dPaintDirectDecal), then this allows you to set the world space distance from the hit point a raycast will be cast from.
		/// 0 = No raycast.
		/// NOTE: This has a performance penalty, so you should disable it if not needed.</summary>
		public float RaycastDistance { set { raycastDistance = value; } get { return raycastDistance; } } [SerializeField] private float raycastDistance = 0.0001f;

		[SerializeField]
		private float cooldown;

		[System.NonSerialized]
		private P3dHitCache hitCache = new P3dHitCache();

		public P3dHitCache HitCache
		{
			get
			{
				return hitCache;
			}
		}

		[ContextMenu("Clear Hit Cache")]
		public void ClearHitCache()
		{
			hitCache.Clear();
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			CheckCollision(collision);
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}

		private void CheckCollision(Collision collision)
		{
			if (cooldown > 0.0f)
			{
				return;
			}

			// Only handle the collision if the impact was strong enough
			if (collision.relativeVelocity.magnitude > threshold)
			{
				cooldown = delay;

				// Calculate up vector ahead of time
				var finalUp  = orientation == OrientationType.CameraUp ? P3dHelper.GetCameraUp(_camera) : Vector3.up;
				var contacts = collision.contacts;

				for (var i = contacts.Length - 1; i >= 0; i--)
				{
					var contact       = contacts[i];
					var finalPosition = contact.point + contact.normal * offset;
					var finalRotation = Quaternion.LookRotation(-contact.normal, finalUp);

					hitCache.InvokePoints(gameObject, null, null, false, contact.otherCollider, finalPosition, finalRotation, 1.0f);

					if (raycastDistance > 0.0f)
					{
						var ray = new Ray(contact.point + contact.normal * raycastDistance, -contact.normal);
						var hit = default(RaycastHit);

						if (contact.otherCollider.Raycast(ray, out hit, raycastDistance * 2.0f) == true)
						{
							hitCache.InvokeRaycast(gameObject, null, null, false, hit, 1.0f);
						}
					}

					if (onlyUseFirstContact == true)
					{
						break;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dHitCollisions))]
	public class P3dHitCollisions_Editor : P3dEditor<P3dHitCollisions>
	{
		protected override void OnInspector()
		{
			Draw("threshold", "The relative speed required for a paint to occur.");
			Draw("onlyUseFirstContact", "If there are multiple contact points, skip them?");
			BeginError(Any(t => t.Delay < 0.0f));
				Draw("delay", "The time in seconds between each collision if you want to limit it.");
			EndError();
			Draw("orientation", "How should the hit point be oriented?\nNone = It will be treated as a point with no rotation.\n\nWorldUp = It will be rotated to the normal, where the up vector is world up.\n\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			BeginIndent();
				if (Any(t => t.Orientation == P3dHitCollisions.OrientationType.CameraUp))
				{
					Draw("_camera", "Orient to a specific camera?\nNone = MainCamera.");
				}
			EndIndent();
			Draw("offset", "If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.");
			Draw("raycastDistance", "If you need raycast information (used by components like P3dPaintDirectDecal), then this allows you to set the world space distance from the hit point a raycast will be cast from.\n\n0 = No raycast.\n\nNOTE: This has a performance penalty, so you should disable it if not needed.");

			Separator();

			Target.HitCache.Inspector(Target.gameObject, false, true, false);
		}
	}
}
#endif