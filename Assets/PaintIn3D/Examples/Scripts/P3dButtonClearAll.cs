using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D.Examples
{
	/// <summary>This component allows you to perform the Clear action. This can be done by attaching it to a clickable object, or manually from the ClearAll method.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dButtonClearAll")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Examples/Button Clear All")]
	public class P3dButtonClearAll : MonoBehaviour, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			ClearAll();
		}

		[ContextMenu("Clear All")]
		public void ClearAll()
		{
			var paintableTexture = P3dPaintableTexture.FirstInstance;

			for (var i = 0; i < P3dPaintableTexture.InstanceCount; i++)
			{
				paintableTexture.Clear();

				paintableTexture = paintableTexture.NextInstance;
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dButtonClearAll))]
	public class P3dButtonClearAll_Editor : P3dEditor<P3dButtonClearAll>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component allows you to perform the Clear action. This can be done by attaching it to a clickable object, or manually from the ClearAll method.", MessageType.Info);
		}
	}
}
#endif