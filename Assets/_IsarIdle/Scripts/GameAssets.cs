using UnityEngine;

[CreateAssetMenu(menuName = "Config/GameAssets")]
public class GameAssets : ScriptableObject
{
    [Header("Sprites")]
    public Sprite leftClickSprite;
    public Sprite rightClickSprite;

    private static GameAssets _instance;
    public static GameAssets Instance => _instance ?? Load();

    private static GameAssets Load()
    {
        _instance = Resources.Load<GameAssets>("GameAssets");
#if UNITY_EDITOR
        if (_instance == null)
            Debug.LogError("GameAssets asset not found in Resources folder!");
#endif
        return _instance;
    }
}