using DG.Tweening;
using Isar.Utils;
using UnityEngine;

namespace Isar.UI
{
	[System.Serializable]
	[Animation("UI/Move")]
	public class UIMoveAnimation : AAnimation
	{
		[Space]
		[Header("Settings")]
		[SerializeField] RectTransform targetRectTransform;
		[SerializeField] bool useAnchoredPosition = false;

		[Space]
		[SerializeField] bool useBaseAsTarget = true;
		[SerializeField][HideIf("useBaseAsTarget")] Vector3 target;
		[SerializeField][HideIf("useBaseAsTarget")] TargetType targetType = TargetType.RelativeToBase;
		[SerializeField] DG.Tweening.Ease ease = DG.Tweening.Ease.OutQuad;


		private Vector3 basePosition;

		public override string GetEditorName()
		{
			return "UI - Move";
		}

		protected override void OnInit()
		{
			if (targetRectTransform == null)
			{
				Debug.LogError("Target RectTransform is not set for MoveAnimation.");
				return;
			}
			if (useAnchoredPosition)
			{
				basePosition = targetRectTransform.anchoredPosition3D;
			}
			else
			{
				basePosition = targetRectTransform.position;
			}
		}

		protected Vector3 GetTarget(Vector3 target, TargetType targetType)
		{
			switch (targetType)
			{
				case TargetType.Absolute:
					return target;
				case TargetType.RelativeToBase:
					return basePosition + target;
				case TargetType.RelativeToCurrent:
					return (useAnchoredPosition ? targetRectTransform.anchoredPosition3D : targetRectTransform.position) + target;
				default:
					Debug.LogError("Unknown TargetType: " + targetType);
					return Vector3.zero;
			}
		}

		protected override void OnPlay(bool instant)
		{
			if (targetRectTransform == null)
			{
				Debug.LogError("Target RectTransform is not set for MoveAnimation.");
				return;
			}

			Vector3 moveTarget;
			if (useBaseAsTarget)
				moveTarget = GetTarget(basePosition, TargetType.Absolute);
			else
				moveTarget = GetTarget(this.target, targetType);

			if (useAnchoredPosition)
			{
				if (instant)
				{
					tween = null; // Clear the tween if instant
					targetRectTransform.anchoredPosition3D = moveTarget;
					return;
				}
				tween = targetRectTransform.DOAnchorPos3D(moveTarget, PlayDuration).SetEase(ease);
			}
			else
			{
				if (instant)
				{
					tween = null; // Clear the tween if instant
					targetRectTransform.position = moveTarget;
					return;
				}
				tween = targetRectTransform.DOMove(moveTarget, PlayDuration).SetEase(ease);
			}
		}

		public override void OnAdd(UnityEngine.Object targetObject)
		{
			if (targetObject is Component component)
			{
				targetRectTransform = component.GetComponent<RectTransform>();
			}
		}
	}
}
