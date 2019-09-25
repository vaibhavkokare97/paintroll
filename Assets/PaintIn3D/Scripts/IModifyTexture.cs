using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This interface allows you to create components that modify the paint texture.</summary>
	public interface IModifyTexture
	{
		void ModifyTexture(ref Texture texture);
	}
}