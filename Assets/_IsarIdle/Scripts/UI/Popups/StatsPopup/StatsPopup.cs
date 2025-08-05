using DG.Tweening;
using Isar.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPopup : APopup
{
	[SerializeField] private Button m_closeBtn;
	[SerializeField] private List<StatController> m_statControllers = new List<StatController>();
	[SerializeField] private TextMeshProUGUI m_statPointText;

	public override void Init()
	{
		base.Init();
		m_closeBtn.onClick.AddListener(OnClickCloseBtn);
		GameData.Instance.onPlayerLevelUp += OnPlayerLevelUp;
		foreach (StatController statUpgrader in m_statControllers)
		{
			statUpgrader.Init();
			statUpgrader.OnValueChanged += OnStatControllerValueChanged;
		}
	}

	private void OnClickCloseBtn()
	{
		UIManager.Instance.ClosePopup(this);
	}

	private void OnDestroy()
	{
		GameData.Instance.onPlayerLevelUp -= OnPlayerLevelUp;
	}

	protected override void SetElementActive(bool isActive)
	{
		UpdateVisual();
		base.SetElementActive(isActive);
	}

	private void OnPlayerLevelUp(int level)
	{
		if (!IsOpen) return;
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		string statPointsText = $"<color=#77ccff><b>{GameData.Instance.statPointsAvailable}</b></color> available points";
		if (statPointsText != m_statPointText.text)
		{
			m_statPointText.transform.DOKill();
			m_statPointText.transform.localScale = Vector3.one;
			m_statPointText.transform.DOPunchScale(Vector3.one * 0.1f, 0.15f, 10, 0.1f);
			m_statPointText.text = statPointsText;
		}
		foreach (StatController statUpgrader in m_statControllers)
		{
			statUpgrader.UpdateVisual();
		}
	}

	private void OnStatControllerValueChanged(StatController statController)
	{
		UpdateVisual();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && IsOpen)
		{
			UIManager.Instance.ClosePopup(this);
		}
	}
}
