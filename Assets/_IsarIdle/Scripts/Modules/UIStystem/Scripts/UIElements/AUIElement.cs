using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Isar.UI
{
    public abstract class AUIElement : MonoBehaviour
    {
		[SerializeField] protected AnimationGroup displayAnimations = new AnimationGroup();

		public bool IsOpen { get; protected set; } = false;
		public bool IsInAnimation => displayAnimations.IsInAnimation;
		public bool IsInit { get; protected set; } = false;

		public virtual void Init()
		{
			if (IsInit)
				return;
			IsInit = true;
			displayAnimations.Hide(true);
			IsOpen = false;
			displayAnimations.OnShowComplete += OnShowDone;
			displayAnimations.OnHideComplete += OnHideDone;

			SetElementActive(false);
		}

		protected virtual void SetElementActive(bool isActive)
		{
			gameObject.SetActive(isActive);
		}

		public void Open(bool instant)
		{
			SetElementActive(true);
			if (IsOpen)
			{
				if (instant && IsInAnimation)
				{
					displayAnimations.Show(true);
				}
				return;
			}
			displayAnimations.Show(instant);
			IsOpen = true;
		}

		public void Close(bool instant)
		{
			if (!IsOpen)
			{
				if (instant && IsInAnimation)
				{
					displayAnimations.Hide(true);
				}
				return;
			}
			displayAnimations.Hide(instant);
			IsOpen = false;
		}

		protected virtual void OnShowDone()
		{
		}

		protected virtual void OnHideDone()
		{
			SetElementActive(false);
		}

		public Tween GetDisplayTween()
		{
			return displayAnimations.GetTween();
		}

		public void StopAnimation()
		{
			if (!IsInAnimation) return;
			if (IsOpen)
			{
				displayAnimations.Show(true);
			}
			else
			{
				displayAnimations.Hide(true);
			}
		}
	}
}
