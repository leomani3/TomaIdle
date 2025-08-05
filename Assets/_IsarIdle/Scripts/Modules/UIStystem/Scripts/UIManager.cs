using System;
using System.Collections.Generic;
using Isar.Utils;
using UnityEngine;

namespace Isar.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] bool initOnAwake = true;
		[SerializeField] bool DebugToggleMainPanel = false;
		[SerializeField] bool DebugToggleMainPanelInstant = false;
		[SerializeField] bool DebugToggleMainPopup = false;
		[SerializeField] bool DebugToggleMainPopupInstant = false;
		[SerializeField] APanel defaultPanel = null;
		[SerializeField] APopup testPopup = null;
		Dictionary<Type, APanel> panels = new Dictionary<Type, APanel>();
		Dictionary<Type, APopup> popups = new Dictionary<Type, APopup>();

		APanel mainPanel = null;
		HashSet<ACanvasUIElement> openUIElements = new HashSet<ACanvasUIElement>();

		HashSet<ACanvasUIElement> mainPopups = new HashSet<ACanvasUIElement>();
		HashSet<ACanvasUIElement> topUIElements = new HashSet<ACanvasUIElement>();
		HashSet<ACanvasUIElement> behindUIElements = new HashSet<ACanvasUIElement>();

		public override void Awake()
		{
			base.Awake();
			if (initOnAwake)
			{
				Init();
			}
		}

		private void Update()
		{
			if (DebugToggleMainPanel || DebugToggleMainPanelInstant)
			{
				if (mainPanel != null)
				{
					ClosePanel(mainPanel, DebugToggleMainPanelInstant);
				}
				else
				{
					OpenPanel(defaultPanel, DebugToggleMainPanelInstant);
				}
				DebugToggleMainPanel = false;
				DebugToggleMainPanelInstant = false;
			}
			if (DebugToggleMainPopup || DebugToggleMainPopupInstant)
			{
				if (testPopup.IsOpen)
				{
					ClosePopup(testPopup, DebugToggleMainPopupInstant);
				}
				else
				{
					OpenPopup(testPopup, DebugToggleMainPopupInstant);
				}
				DebugToggleMainPopup = false;
				DebugToggleMainPopupInstant = false;
			}
		}

		public void Init()
        {
            panels.Clear();
            APanel[] allPanels = transform.GetComponentsInChildren<APanel>(true);
            APopup[] allPopups = transform.GetComponentsInChildren<APopup>(true);

            foreach (APanel panel in allPanels)
            {
                Type type = panel.GetType();
                if (panels.TryAdd(type, panel))
				{
					panel.Init();
				}
				else
				{
					Debug.LogError($"Panel of type {type} already exists in UIManager. Please ensure each panel type is unique.");
				}
			}

			foreach (APopup popup in allPopups)
			{
				Type type = popup.GetType();
				if (popups.TryAdd(type, popup))
				{
					popup.Init();
				}
				else
				{
					Debug.LogError($"Popup of type {type} already exists in UIManager. Please ensure each panel type is unique.");
				}
			}

			if (defaultPanel != null)
				OpenPanel(defaultPanel, true, OpenMode.Main);
		}

		public T GetPopup<T>() where T : APopup
		{
			return GetPopup(typeof(T)) as T;
		}

		public APopup GetPopup(APanel panel)
		{
			return GetPopup(panel.GetType());
		}

		public APopup GetPopup(Type type)
		{
			if (popups.TryGetValue(type, out APopup panel))
			{
				return panel;
			}
			else
			{
				Debug.LogError($"Popup of type {type} not found in UIManager.");
			}
			return null;
		}

		public T OpenPopup<T>(bool instant = false, OpenMode openMode = OpenMode.Main) where T : APopup
		{
			return OpenPopup(typeof(T), instant, openMode) as T;
		}

		public APopup OpenPopup(APopup popup, bool instant = false, OpenMode openMode = OpenMode.Main)
		{
			return OpenPopup(popup.GetType(), instant, openMode);
		}

		public APopup OpenPopup(Type type, bool instant = false, OpenMode openMode = OpenMode.Main)
		{
			if (popups.TryGetValue(type, out APopup popup))
			{
				if (IsPopupOpen(popup))
				{
					if (instant && popup.IsInAnimation)
					{
						popup.Open(true);
					}
					else
					{
						Debug.LogWarning($"Popup of type {type} is already open. Cannot open it again.");
					}
					return popup;
				}

				switch (openMode)
				{
					case OpenMode.Main:
						int targetIndex = -1;
						if (mainPanel != null)
						{
							targetIndex = mainPanel.transform.GetSiblingIndex() + 1;
						}
						foreach (var mainPopup in mainPopups)
						{
							if (mainPopup.transform.GetSiblingIndex() > targetIndex)
							{
								targetIndex = mainPopup.transform.GetSiblingIndex();
							}
						}
						if (targetIndex == -1)
						{
							if (behindUIElements.Count == 0)
							{
								popup.transform.SetSiblingIndex(1);
							}
							else if (topUIElements.Count == 0)
							{
								popup.transform.SetAsLastSibling();
							}
							else
							{
								targetIndex = int.MaxValue;
								foreach (var item in topUIElements)
								{
									if (item.transform.GetSiblingIndex() < targetIndex)
									{
										targetIndex = item.transform.GetSiblingIndex();
									}
								}
								popup.transform.SetSiblingIndex(targetIndex);
							}
						}
						else
						{
							popup.transform.SetSiblingIndex(targetIndex);
						}
						mainPopups.Add(popup);
						break;
					case OpenMode.Behind:
						popup.transform.SetAsFirstSibling();
						behindUIElements.Add(popup);
						break;
					case OpenMode.OnTop:
						popup.transform.SetAsLastSibling();
						topUIElements.Add(popup);
						break;
					default:
						break;
				}
				openUIElements.Add(popup);

				popup.Open(instant);

				return popup;
			}
			else
			{
				Debug.LogError($"Popup of type {type} not found in UIManager.");
			}
			return null;
		}


		public void ClosePopup<T>(bool instant = false) where T : APopup
		{
			ClosePopup(typeof(T), instant);
		}

		public void ClosePopup(APopup panel, bool instant = false)
		{
			ClosePopup(panel.GetType(), instant);
		}
		public void ClosePopup(Type type, bool instant = false)
		{
			if (popups.TryGetValue(type, out APopup popup))
			{
				if (!IsPopupOpen(popup))
				{
					if (instant && popup.IsInAnimation)
					{
						popup.Close(true);
					}
					else
					{
						Debug.LogWarning($"Popup of type {type} is not open. Cannot close it.");
					}
					return;
				}
				openUIElements.Remove(popup);
				mainPopups.Remove(popup);
				behindUIElements.Remove(popup);
				topUIElements.Remove(popup);
				popup.Close(instant);
			}
			else
			{
				Debug.LogError($"Popup of type {type} not found in UIManager.");
			}
		}

		public bool IsPopupOpen<T>() where T : APopup
		{
			return IsPopupOpen(typeof(T));
		}

		public bool IsPopupOpen(APopup popup)
		{
			return IsPopupOpen(popup.GetType());
		}

		public bool IsPopupOpen(Type type)
		{
			if (popups.TryGetValue(type, out APopup popup))
			{
				return openUIElements.Contains(popup);
			}
			else
			{
				Debug.LogError($"Popup of type {type} not found in UIManager.");
			}
			return false;
		}

		public T GetPanel<T>() where T : APanel
		{
			return GetPanel(typeof(T)) as T;
		}

		public APanel GetPanel(APanel panel)
		{
			return GetPanel(panel.GetType());
		}

		public APanel GetPanel(Type type)
		{
			if (panels.TryGetValue(type, out APanel panel))
			{
				return panel;
			}
			else
			{
				Debug.LogError($"Panel of type {type} not found in UIManager.");
			}
			return null;
		}

		public T OpenPanel<T>(bool instant = false, OpenMode openMode = OpenMode.Main) where T : APanel
		{
			return OpenPanel(typeof(T), instant, openMode) as T;
		}

		public APanel OpenPanel(APanel panel, bool instant = false, OpenMode openMode = OpenMode.Main)
		{
			return OpenPanel(panel.GetType(), instant, openMode);
		}

		public APanel OpenPanel(Type type, bool instant = false, OpenMode openMode = OpenMode.Main)
		{
			if (panels.TryGetValue(type, out APanel panel))
			{
				if (IsPanelOpen(panel))
				{
					if (instant && panel.IsInAnimation)
					{
						panel.Open(true);
					}
					else
					{
						Debug.LogWarning($"Panel of type {type} is already open. Cannot open it again.");
					}
					return panel;
				}

				switch (openMode)
				{
					case OpenMode.Main:
						int targetIndex = -1;
						if (mainPanel != null)
						{
							targetIndex = mainPanel.transform.GetSiblingIndex() + 1;
							ClosePanel(mainPanel, instant);
						}
						mainPanel = panel;
						if (targetIndex == -1)
						{
							if (behindUIElements.Count == 0)
							{
								panel.transform.SetAsFirstSibling();
							}
							else if (topUIElements.Count == 0)
							{
								panel.transform.SetAsLastSibling();
							}
							else
							{
								targetIndex = int.MaxValue;
								foreach (var item in topUIElements)
								{
									if (item.transform.GetSiblingIndex() < targetIndex)
									{
										targetIndex = item.transform.GetSiblingIndex();
									}
								}
								panel.transform.SetSiblingIndex(targetIndex);
							}
						}
						else
						{
							panel.transform.SetSiblingIndex(targetIndex);
						}
						break;
					case OpenMode.Behind:
						panel.transform.SetAsFirstSibling();
						behindUIElements.Add(panel);
						break;
					case OpenMode.OnTop:
						panel.transform.SetAsLastSibling();
						topUIElements.Add(panel);
						break;
					default:
						break;
				}
				openUIElements.Add(panel);

				panel.Open(instant);

				return panel;
			}
			else
			{
				Debug.LogError($"Panel of type {type} not found in UIManager.");
			}
			return null;
		}

		public void ClosePanel<T>(bool instant = false) where T : APanel
		{
			ClosePanel(typeof(T), instant);
		}

		public void ClosePanel(APanel panel, bool instant = false)
		{
			ClosePanel(panel.GetType(), instant);
		}

		public void ClosePanel(Type type, bool instant = false)
		{
			if (panels.TryGetValue(type, out APanel panel))
			{
				if (!IsPanelOpen(panel))
				{
					if (instant && panel.IsInAnimation)
					{
						panel.Close(true);
					}
					else
					{
						Debug.LogWarning($"Panel of type {type} is not open. Cannot close it.");
					}
					return;
				}
				openUIElements.Remove(panel);
				if (mainPanel == panel)
					mainPanel = null;
				else
				{
					behindUIElements.Remove(panel);
					topUIElements.Remove(panel);
				}
				panel.Close(instant);
			}
			else
			{
				Debug.LogError($"Panel of type {type} not found in UIManager.");
			}
		}

		public bool IsPanelOpen<T>() where T : APanel
		{
			return IsPanelOpen(typeof(T));
		}

		public bool IsPanelOpen(APanel panel)
		{
			return IsPanelOpen(panel.GetType());
		}

		public bool IsPanelOpen(Type type)
		{
			if (panels.TryGetValue(type, out APanel panel))
			{
				return openUIElements.Contains(panel);
			}
			else
			{
				Debug.LogError($"Panel of type {type} not found in UIManager.");
			}
			return false;
		}

		public enum OpenMode
		{
			Main,
			Behind,
			OnTop
		}
	}
}
