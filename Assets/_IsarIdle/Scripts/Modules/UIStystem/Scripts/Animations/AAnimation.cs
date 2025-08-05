using DG.Tweening;
using System;
using UnityEngine;

namespace Isar.UI
{
	public class AnimationAttribute : Attribute
	{
		public string DisplayName { get; private set; }
		public AnimationAttribute(string path)
		{
			DisplayName = path;
		}
	}

	[System.Serializable]
	public abstract class AAnimation
	{
		[field: SerializeField, Min(0f)]
		public float PlayDelay { get; protected set; } = 0f;
		[field: SerializeField, Min(0f)]
		public float PlayDuration { get; protected set; } = 0.5f;

		protected bool isInit = false;
		protected Tween tween;

		public void Init()
		{
			OnInit();
			isInit = true;
		}
		protected abstract void OnInit();

		public void Play(bool instant = false)
		{
			if (!isInit)
				Init();
			OnPlay(instant);
		}
		protected abstract void OnPlay(bool instant);

		public virtual void Kill()
		{
			if (tween != null && tween.IsActive() && tween.IsPlaying())
			{
				tween.Kill();
			}
		}

		public abstract string GetEditorName();
		public virtual Tween GetAnimationTween()
		{
			return tween;
		}

		public virtual void OnAdd(UnityEngine.Object targetObject) { }
	}

	public enum TargetType
	{
		Absolute,
		RelativeToBase,
		RelativeToCurrent,
	}
}
