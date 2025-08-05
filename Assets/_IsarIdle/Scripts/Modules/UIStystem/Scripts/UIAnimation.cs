using UnityEngine;

namespace Isar.UI
{
    public class UIAnimation : MonoBehaviour
    {
		public bool startHidden = true;
		public bool debugToggle = false;

		public AnimationGroup showAnimations = new AnimationGroup();

		private void Start()
		{
			if (startHidden)
			{
				showAnimations.Hide(true);
			}
		}

		private void Update()
		{
			if (debugToggle)
			{
				if (showAnimations.IsShown)
				{
					showAnimations.Hide();
				}
				else
				{
					showAnimations.Show();
				}
				debugToggle = !debugToggle;
			}
		}
	}
}
