using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Isar.UI
{
	[System.Serializable]
	[Animation("UI/Sub UIElements")]
	public class UISubUIElements : AAnimation
	{
		public MethodToCall methodToCall = MethodToCall.Show;
		public List<AUIElement> UIElements = new List<AUIElement>();

		public override string GetEditorName()
		{
			return "UI - Sub UI Elements";
		}

		protected override void OnInit()
		{
			foreach (var element in UIElements)
			{
				if (element != null)
				{
					element.Init();
				}
			}
		}

		protected override void OnPlay(bool instant)
		{
			Sequence seq = DOTween.Sequence();
			switch (methodToCall)
			{
				case MethodToCall.Show:
					foreach (var element in UIElements)
					{
						if (element != null)
						{
							element.Open(instant);
							seq.Insert(0, element.GetDisplayTween());
						}
					}
					break;
				case MethodToCall.Hide:
					foreach (var element in UIElements)
					{
						if (element != null)
						{
							element.Close(instant);
							seq.Insert(0, element.GetDisplayTween());
						}
					}
					break;
				default:
					break;
			}
			tween = seq.Play();
		}

		public enum MethodToCall
		{
			Show,
			Hide,
		}
	}
}
