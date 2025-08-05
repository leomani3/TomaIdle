using System;
using BreakInfinity;
using Isar.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : APanel
{
    [Title("References")]
    [SerializeField] private RectTransform m_damageTextParent;
    [SerializeField] private RectTransform m_EntityUIParent;

    [Title("Buttons")]
    [SerializeField] private Button m_statsMenuBtn;

	[Title("Player UI")]
    [SerializeField] private PlayerUI m_playerUI;
    
    public RectTransform DamageTextParent => m_damageTextParent;
    public RectTransform EntityUIParent => m_EntityUIParent;
    public PlayerUI PlayerUI => m_playerUI;

    private void Awake()
    {
        m_statsMenuBtn.onClick.AddListener(OnClickStatsMenuBtn);
    }

	#region Callbacks
	private void OnClickStatsMenuBtn()
	{
        if (UIManager.Instance.IsPopupOpen<StatsPopup>())
		{
			UIManager.Instance.ClosePopup<StatsPopup>();
		}
        else
        {
            UIManager.Instance.OpenPopup<StatsPopup>();
        }
	}
    #endregion
}
