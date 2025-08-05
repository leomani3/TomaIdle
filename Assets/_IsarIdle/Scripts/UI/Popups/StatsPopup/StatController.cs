using BreakInfinity;
using DG.Tweening;
using MPUIKIT;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatController : MonoBehaviour
{
	public Action<StatController> OnValueChanged;

	[SerializeField] private GameData.UpgradableStats m_upgradableStat;
	[SerializeField] private MPImage m_fillImage;
	[SerializeField] private TextMeshProUGUI m_pointsSpentText;

	[SerializeField] private Button m_spendPointBtn;
	[SerializeField] private Button m_removePointBtn;
	[SerializeField] private CanvasGroup m_spendPointCanvasGroup;
	[SerializeField] private CanvasGroup m_removePointCanvasGroup;

	private GameData.StatUpgrade m_statUpgradeRef;
	private GameConfig.StatUpgrade m_statUpgradeConfigRef;
	private bool m_updateStatOnPlayerRegister = false;

	private StatModifier<BigDouble> m_bigStatModifier = null;
	private StatModifier<float> m_floatStatModifier = null;

	public void Init()
	{
		m_fillImage.fillAmount = 0f;
		m_pointsSpentText.text = "0";
		foreach (GameData.StatUpgrade statUpgrade in GameData.Instance.statUpgrades)
		{
			if (statUpgrade.upgradableStat == m_upgradableStat)
			{
				m_statUpgradeRef = statUpgrade;
				break;
			}
		}
		foreach (GameConfig.StatUpgrade statUpgrade in GameConfig.Instance.statUpgrades)
		{
			if (statUpgrade.upgradableStat == m_upgradableStat)
			{
				m_statUpgradeConfigRef = statUpgrade;
				break;
			}
		}
		if (m_statUpgradeRef == null)
		{
			Debug.LogError($"StatController: No stat upgrade found for {m_upgradableStat} in PlayerData");
			return;
		}
		m_spendPointBtn.onClick.AddListener(SpendPoint);
		m_removePointBtn.onClick.AddListener(RemovePoint);
		UpdateVisual();
	}

	private void SpendPoint()
	{
		Debug.Log("Add Point");
		if (GameData.Instance.statPointsAvailable <= 0 || m_statUpgradeRef.currentLevel >= GameData.Instance.maxStatUpgradeLevel)
			return;

		GameData.Instance.statPointsAvailable--;
		m_statUpgradeRef.currentLevel++;

		OnValueChanged?.Invoke(this);
	}

	private void RemovePoint()
	{
		Debug.Log("Remove point");
		if (m_statUpgradeRef.currentLevel <= 0)
			return;

		GameData.Instance.statPointsAvailable++;
		m_statUpgradeRef.currentLevel--;

		UpdateVisual();
		OnValueChanged?.Invoke(this);
	}

	public void UpdateVisual()
	{
		if (m_statUpgradeRef == null)
			return;
		m_statUpgradeRef.currentLevel = Mathf.Clamp(m_statUpgradeRef.currentLevel, 0, GameData.Instance.maxStatUpgradeLevel);
		string pointsText = m_statUpgradeRef.currentLevel.ToString();
		if (pointsText != m_pointsSpentText.text)
		{
			m_pointsSpentText.transform.DOKill();
			m_pointsSpentText.transform.localScale = Vector3.one;
			m_pointsSpentText.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, 10, 0.1f);
			m_pointsSpentText.text = pointsText;
		}
		m_fillImage.DOKill();
		m_fillImage.DOFillAmount(m_statUpgradeRef.currentLevel / (float)GameData.Instance.maxStatUpgradeLevel, 0.15f).SetEase(Ease.OutQuad);
		UpdatePlayerStat();

		m_spendPointBtn.interactable = GameData.Instance.statPointsAvailable > 0 && m_statUpgradeRef.currentLevel < GameData.Instance.maxStatUpgradeLevel;
		m_spendPointCanvasGroup.alpha = m_spendPointBtn.interactable ? 1f : 0.5f;
		m_removePointBtn.interactable = m_statUpgradeRef.currentLevel > 0;
		m_removePointCanvasGroup.alpha = m_removePointBtn.interactable ? 1f : 0.5f;
	}

	private void OnDisable()
	{
		m_fillImage.DOKill();
		m_fillImage.fillAmount = m_statUpgradeRef.currentLevel / (float)GameData.Instance.maxStatUpgradeLevel;
	}

	private void OnPlayerEntityRegistered(Entity player)
	{
		EntityManager.onPlayerRegistered -= OnPlayerEntityRegistered;
		if (m_updateStatOnPlayerRegister)
		{
			UpdatePlayerStat();
		}
	}

	public void UpdatePlayerStat()
	{
		if (EntityManager.Instance == null || EntityManager.Instance.Player == null)
		{
			m_updateStatOnPlayerRegister = true;
			EntityManager.onPlayerRegistered -= OnPlayerEntityRegistered;
			EntityManager.onPlayerRegistered += OnPlayerEntityRegistered;
			return;
		}

		bool useBigDouble = false;
		switch (m_upgradableStat)
		{
			case GameData.UpgradableStats.Damage:
				useBigDouble = true;
				break;
			default:
				break;
		}

		m_updateStatOnPlayerRegister = false;
		if (useBigDouble ? m_bigStatModifier == null : m_floatStatModifier == null)
		{
			if (useBigDouble)
				m_bigStatModifier = new StatModifier<BigDouble>(m_statUpgradeConfigRef.modifierBalancingBig.Evaluate(m_statUpgradeRef.currentLevel), m_statUpgradeConfigRef.modifierType);
			else
				m_floatStatModifier = new StatModifier<float>(m_statUpgradeConfigRef.modifierBalancingFloat.Evaluate(m_statUpgradeRef.currentLevel), m_statUpgradeConfigRef.modifierType);

			StatsEM statsEM = EntityManager.Instance.Player.GetModule<StatsEM>();
			switch (m_upgradableStat)
			{
				case GameData.UpgradableStats.Damage:
					statsEM.Damages.AddModifier(m_bigStatModifier);
					break;
				case GameData.UpgradableStats.AttackSpeed:
					statsEM.AttackSpeed.AddModifier(m_floatStatModifier);
					break;
				default:
					Debug.LogWarning($"{m_upgradableStat.ToString()} stat upgrade is not implemented yet.");
					break;
			}

		}
		else
		{
			if (useBigDouble)
				m_bigStatModifier.UpdateValue(m_statUpgradeConfigRef.modifierBalancingBig.Evaluate(m_statUpgradeRef.currentLevel));
			else
				m_floatStatModifier.UpdateValue(m_statUpgradeConfigRef.modifierBalancingFloat.Evaluate(m_statUpgradeRef.currentLevel));
		}
	}
}
