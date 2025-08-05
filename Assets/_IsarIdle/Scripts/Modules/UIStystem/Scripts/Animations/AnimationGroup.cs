using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Isar.UI
{

	[System.Serializable]
	public class AnimationGroup
	{
		public Action OnAnimationComplete;
		public Action OnShowComplete;
		public Action OnHideComplete;

		[SerializeField]
		protected float showDuration = 0f;
		[SerializeField]
		protected float hideDuration = 0f;
		[SerializeField]
		[SerializeReference]
		protected List<AAnimation> showAnimations = new List<AAnimation>() { };
		[SerializeReference]
		protected List<AAnimation> hideAnimations = new List<AAnimation>() { };
		public List<AAnimation> ShowAnimations => showAnimations;
		public List<AAnimation> HideAnimations => hideAnimations;

		[SerializeField]
		protected bool isInAnimation = false;
		[SerializeField]
		protected bool isInShowAnimation = false;
		[SerializeField]
		protected bool isInHideAnimation = false;

		protected bool isShown = true;
		protected bool isInit = false;


		public bool IsInAnimation => isInAnimation;
		public bool IsInShowAnimation => isInShowAnimation;
		public bool IsInHideAnimation => isInHideAnimation;
		public bool IsShown => isShown;
		public float ShowDuration => showDuration;
		public float HideDuration => hideDuration;
		Tween tween = null;

		private void Init()
		{
			isInit = true;
			foreach (AAnimation animation in showAnimations)
				animation.Init();
			foreach (AAnimation animation in hideAnimations)
				animation.Init();
		}

		private void DoAnimation(bool instant, bool show)
		{
			if (!isInit)
				Init();
			else if ((isShown == show) && !instant)
			{
				return;
			}
			Kill();
			isShown = show;
			List<AAnimation> animationsToPlay = show ? showAnimations : hideAnimations;
			isInAnimation = true;
			if (show)
				isInShowAnimation = true;
			else
				isInHideAnimation = true;
			Sequence seq = DOTween.Sequence();//Todo: reuse tweens?
			foreach (var animation in animationsToPlay)
			{
				animation.Play(instant);
				if (!instant)
				{
					Tween tween = animation.GetAnimationTween();
					if (tween != null)
					{
						seq.Insert(animation.PlayDelay, tween);//Insert the animation at its delay time
					}
				}
			}
			if (!instant)
			{
				tween = seq;
				seq.OnComplete(() =>
				{
					isInAnimation = false;
					isInShowAnimation = false;
					isInHideAnimation = false;
					OnAnimationComplete?.Invoke();
					if (show)
						OnShowComplete?.Invoke();
					else
						OnHideComplete?.Invoke();
				});
				seq.Play();
			}
			else
			{
				tween = null;
				isInAnimation = false;
				isInShowAnimation = false;
				isInHideAnimation = false;
				OnAnimationComplete?.Invoke();
				if (show)
					OnShowComplete?.Invoke();
				else
					OnHideComplete?.Invoke();

			}
		}

		public void Show(bool instant = false)
		{
			DoAnimation(instant, true);
		}

		public void Hide(bool instant = false)
		{
			DoAnimation(instant, false);
		}

		public void Kill()
		{
			if (!isInAnimation)
				return;
			if (tween != null && tween.IsActive() && tween.IsPlaying())
			{
				tween.Kill();
			}
			isInAnimation = false;
			isInShowAnimation = false;
			isInHideAnimation = false;
			List<AAnimation> animations = isShown ? showAnimations : hideAnimations;
			foreach (var animation in animations)
			{
				animation.Kill();
			}
		}

		public Tween GetTween()
		{
			return tween;
		}
	}
}
