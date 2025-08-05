using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Game Settings")]
    public int maxLives = 3;
	public List<StatUpgrade> statUpgrades = new List<StatUpgrade>();

	private static GameConfig _instance;
    public static GameConfig Instance => _instance ?? Load();

    private static GameConfig Load()
    {
        _instance = Resources.Load<GameConfig>("GameConfig");
#if UNITY_EDITOR
        if (_instance == null)
            Debug.LogError("GameConfig asset not found in Resources folder!");
#endif
        return _instance;
    }

	[System.Serializable]
	public class StatUpgrade
	{
		public GameData.UpgradableStats upgradableStat;
		public ModifierType modifierType = ModifierType.Percent;
		public BigBalanceFunction modifierBalancingBig;
		public FloatBalanceFunction modifierBalancingFloat;
	}
}