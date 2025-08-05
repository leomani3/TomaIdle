using UnityEngine;
using System.Collections.Generic;
using System;
using BreakInfinity;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Config/GameData")]
public class GameData : ScriptableObject
{
    [Header("Runtime Data")]
    public int highScore;

	[Header("Player Data")]
	[ES3NonSerializable] public Action<int> onPlayerLevelUp;
	[ES3NonSerializable] public Action onExperienceChanged;
	public BigDouble totalExperienceEarned;
	[ES3NonSerializable] public BigBalanceFunction experienceNeeded;

	[Header("Stats")]
	public int statPointsAvailable = 0;
	public List<StatUpgrade> statUpgrades = new List<StatUpgrade>();
	public int maxStatUpgradeLevel = 30;//Move this if it will not change


	[System.Serializable]
	public class StatUpgrade
	{
		public UpgradableStats upgradableStat;
		public int currentLevel = 0;
	}

	/// Todo: Determine what stats we want to upgrade
	public enum UpgradableStats
	{
		Damage,
		AttackSpeed,
		MaxHealth,
		MaxMana,
		Regen
	}

	public void GainExperience(BigDouble experienceGained)
	{
		int previousLevel = GetCurrentLevel();

		totalExperienceEarned += experienceGained;

		int newLevel = GetCurrentLevel();

		for (int level = previousLevel + 1; level <= newLevel; level++)
		{
			LevelUp(level);
		}

		onExperienceChanged?.Invoke();
	}

	private void LevelUp(int newLevel)
	{
		//Todo: Calculate stat point according to level later to adapt to balance changes
		statPointsAvailable++;
		onPlayerLevelUp?.Invoke(newLevel);
	}

	public int GetCurrentLevel()
	{
		int level = 0;
		BigDouble remainingXP = totalExperienceEarned;

		const int maxLevel = 9999;

		while (level < maxLevel)
		{
			BigDouble xpRequired = experienceNeeded.Evaluate(level);
			if (xpRequired <= 0f)
				break;

			if (remainingXP >= xpRequired)
			{
				remainingXP -= xpRequired;
				level++;
			}
			else
			{
				break;
			}
		}

		return level;
	}

	public BigDouble GetXPToNextLevel()
	{
		int level = GetCurrentLevel();
		return experienceNeeded.Evaluate(level);
	}

	public BigDouble GetCurrentXPIntoLevel()
	{
		int level = 0;
		BigDouble remainingXP = totalExperienceEarned;

		const int maxLevel = 9999;

		while (level < maxLevel)
		{
			BigDouble xpRequired = experienceNeeded.Evaluate(level);
			if (xpRequired <= 0f)
				break;

			if (remainingXP >= xpRequired)
			{
				remainingXP -= xpRequired;
				level++;
			}
			else
			{
				break;
			}
		}

		return remainingXP;
	}

	public float GetProgressToNextLevel01()
	{
		BigDouble xpToNext = GetXPToNextLevel();
		if (xpToNext <= 0f) return 1f;

		return Mathf.Clamp01((float)GetCurrentXPIntoLevel().Divide(xpToNext).ToDouble());
	}

	private static GameData _instance;
    public static GameData Instance => _instance ?? Load();

	public void Save()
	{
		ES3.Save("GameData", this);
		Debug.Log("GameData saved using ES3.");
	}

	private static GameData Load()
	{
		_instance = Resources.Load<GameData>("GameData");
#if UNITY_EDITOR
		if (_instance == null)
			Debug.LogError("GameData asset not found in Resources folder!");
#endif

		if (ES3.KeyExists("GameData"))
		{
			ES3.LoadInto("GameData", _instance);
			Debug.Log("GameData loaded using ES3.");
		}

		if (_instance.statUpgrades == null || _instance.statUpgrades.Count == 0)
		{
			foreach (UpgradableStats stat in System.Enum.GetValues(typeof(UpgradableStats)))
			{
				_instance.statUpgrades.Add(new StatUpgrade { upgradableStat = stat });
			}
		}

		return _instance;
	}
}