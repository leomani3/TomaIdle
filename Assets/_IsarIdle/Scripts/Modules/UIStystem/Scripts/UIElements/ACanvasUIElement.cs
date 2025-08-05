using UnityEngine;

namespace Isar.UI
{
	[RequireComponent(typeof(Canvas))]
	public abstract class ACanvasUIElement: AUIElement
    {
		[SerializeField] Canvas canvas;

		protected virtual void Reset()
		{
			canvas = GetComponent<Canvas>();
		}

		protected override void SetElementActive(bool isActive)
		{
			canvas.enabled = isActive;
		}
	}
}
