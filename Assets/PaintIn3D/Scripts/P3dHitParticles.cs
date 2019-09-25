using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component can be added to any ParticleSystem with collisions enabled, and it will fire hits when the particles collide with something.</summary>
	[RequireComponent(typeof(ParticleSystem))]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dHitParticles")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Hit/Hit Particles")]
	public class P3dHitParticles : MonoBehaviour
	{
		public enum OrientationType
		{
			WorldUp,
			CameraUp
		}

		public enum NormalType
		{
			ParticleVelocity,
			CollisionNormal
		}

		/// <summary>Should the particles paint preview paint?</summary>
		public bool Preview { set { preview = value; } get { return preview; } } [SerializeField] private bool preview;

		/// <summary>How should the hit point be oriented?
		/// WorldUp = It will be rotated to the normal, where the up vector is world up.
		/// CameraUp = It will be rotated to the normal, where the up vector is world up.</summary>
		public OrientationType Orientation { set { orientation = value; } get { return orientation; } } [SerializeField] private OrientationType orientation;

		/// <summary>Orient to a specific camera?
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>Which normal should the hit point rotation be based on?</summary>
		public NormalType Normal { set { normal = value; } get { return normal; } } [SerializeField] private NormalType normal;

		/// <summary>If you want the raycast hit point to be offset from the surface a bit, this allows you to set by how much in world space.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		[System.NonSerialized]
		private ParticleSystem cachedParticleSystem;

		[System.NonSerialized]
		private static List<ParticleCollisionEvent> particleCollisionEvents = new List<ParticleCollisionEvent>();

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

		protected virtual void OnEnable()
		{
			cachedParticleSystem = GetComponent<ParticleSystem>();
		}

		protected virtual void OnParticleCollision(GameObject hitGameObject)
		{
			// Get the collision events array
			var count = cachedParticleSystem.GetSafeCollisionEventSize();

			// Expand collisionEvents list to fit all particles
			for (var i = particleCollisionEvents.Count; i < count; i++)
			{
				particleCollisionEvents.Add(new ParticleCollisionEvent());
			}

			count = cachedParticleSystem.GetCollisionEvents(hitGameObject, particleCollisionEvents);

			// Calculate up vector ahead of time
			var finalUp = orientation == OrientationType.CameraUp ? P3dHelper.GetCameraUp(_camera) : Vector3.up;

			// Paint all locations
			for (var i = 0; i < count; i++)
			{
				var collisionEvent = particleCollisionEvents[i];
				var finalPosition  = collisionEvent.intersection + collisionEvent.normal * offset;
				var finalNormal    = normal == NormalType.CollisionNormal ? collisionEvent.normal : -collisionEvent.velocity;
				var finalRotation  = finalNormal != Vector3.zero ? Quaternion.LookRotation(finalNormal, finalUp) : Quaternion.identity;

				hitCache.InvokePoints(gameObject, null, null, false, collisionEvent.colliderComponent as Collider, finalPosition, finalRotation, 1.0f);
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomEditor(typeof(P3dHitParticles))]
	public class P3dHitParticles_Editor : P3dEditor<P3dHitParticles>
	{
		protected override void OnInspector()
		{
			Draw("preview", "Should the particles paint preview paint?");
			Draw("orientation", "How should the hit point be oriented?\nNone = It will be treated as a point with no rotation.\nWorldUp = It will be rotated to the normal, where the up vector is world up.\nCameraUp = It will be rotated to the normal, where the up vector is world up.");
			BeginIndent();
				if (Any(t => t.Orientation == P3dHitParticles.OrientationType.CameraUp))
				{
					Draw("_camera", "Orient to a specific camera?\nNone = MainCamera.");
				}
			EndIndent();
			Draw("normal", "Which normal should the hit point rotation be based on?");

			Separator();

			Target.HitCache.Inspector(Target.gameObject, false, true, false);
		}
	}
}
#endif