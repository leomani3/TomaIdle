using DG.Tweening;
using Isar.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Isar.UI
{
	[System.Serializable]
	[Animation("UI/Fade/Canvas Group")]
	public class UICanvasFadeAnimation : AAnimation
	{
		[Space]
		[Header("Settings")]
		[SerializeField] CanvasGroup targetCanvasGroup;

		[Space]
		[SerializeField] bool useBaseAsTarget = false;
		[SerializeField][HideIf("useBaseAsTarget")] float targetAlpha = 1f;
		[SerializeField][HideIf("useBaseAsTarget")] TargetType targetType = TargetType.Absolute;
		[SerializeField] DG.Tweening.Ease ease = DG.Tweening.Ease.OutQuad;

		private float baseAlpha;

		public override string GetEditorName()
		{
			return "UI - Fade Canvas Group";
		}

		protected override void OnInit()
		{
			if (targetCanvasGroup == null)
			{
				Debug.LogError("Target Canvas Group is not set for FadeAnimation.");
				return;
			}
			baseAlpha = targetCanvasGroup.alpha;
		}

		protected float GetTarget(float target, TargetType targetType)
		{
			switch (targetType)
			{
				case TargetType.Absolute:
					return Mathf.Clamp01(target);
				case TargetType.RelativeToBase:
					return Mathf.Clamp01(baseAlpha + target);
				case TargetType.RelativeToCurrent:
					return Mathf.Clamp01(targetCanvasGroup.alpha + target);
				default:
					Debug.LogError("Unknown TargetType: " + targetType);
					return 0f;
			}
		}

		protected override void OnPlay(bool instant)
		{
			if (targetCanvasGroup == null)
			{
				Debug.LogError("Target Canvas Group is not set for FadeAnimation.");
				return;
			}

			float fadeTarget;
			if (useBaseAsTarget)
				fadeTarget = GetTarget(baseAlpha, TargetType.Absolute);
			else
				fadeTarget = GetTarget(targetAlpha, targetType);

			if (instant)
			{
				tween = null; // Clear the tween if instant
				targetCanvasGroup.alpha = fadeTarget;
				return;
			}
			tween = targetCanvasGroup.DOFade(fadeTarget, PlayDuration).SetEase(ease);
		}

		public override void OnAdd(Object targetObject)
		{
			if (targetObject is Component component)
			{
				targetCanvasGroup = component.GetComponent<CanvasGroup>();
			}
		}
	}
}
