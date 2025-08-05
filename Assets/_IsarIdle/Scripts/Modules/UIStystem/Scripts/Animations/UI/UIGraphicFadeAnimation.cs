using DG.Tweening;
using Isar.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Isar.UI
{
	[System.Serializable]
	[Animation("UI/Fade/Graphic")]
	public class UIGraphicFadeAnimation : AAnimation
	{
		[Space]
		[Header("Settings")]
		[SerializeField] Graphic targetGraphic;

		[Space]
		[SerializeField] bool useBaseAsTarget = false;
		[SerializeField][HideIf("useBaseAsTarget")] float targetAlpha = 1f;
		[SerializeField][HideIf("useBaseAsTarget")] TargetType targetType = TargetType.Absolute;
		[SerializeField] DG.Tweening.Ease ease = DG.Tweening.Ease.OutQuad;

		private float baseAlpha;

		public override string GetEditorName()
		{
			return "UI - Fade Graphic";
		}

		protected override void OnInit()
		{
			if (targetGraphic == null)
			{
				Debug.LogError("Target Graphic is not set for FadeAnimation.");
				return;
			}
			baseAlpha = targetGraphic.color.a;
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
					return Mathf.Clamp01(targetGraphic.color.a + target);
				default:
					Debug.LogError("Unknown TargetType: " + targetType);
					return 0f;
			}
		}

		protected override void OnPlay(bool instant)
		{
			if (targetGraphic == null)
			{
				Debug.LogError("Target Graphic is not set for FadeAnimation.");
				return;
			}

			float fadeTarget;
			if (useBaseAsTarget)
				fadeTarget = GetTarget(baseAlpha, TargetType.Absolute);
			else
				fadeTarget = GetTarget(targetAlpha, targetType);

			Color targetColor = targetGraphic.color;
			targetColor.a = fadeTarget;
			if (instant)
			{
				tween = null; // Clear the tween if instant
				targetGraphic.color = targetColor;
				return;
			}
			tween = targetGraphic.DOFade(fadeTarget, PlayDuration).SetEase(ease);
		}
		public override void OnAdd(Object targetObject)
		{
			if (targetObject is Component component)
			{
				targetGraphic = component.GetComponent<Graphic>();
			}
		}
	}
}
