using Isar.UI;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityEM : AbilityEM
{
    [SerializeField] private AbilityData m_leftClickAbility;
    [SerializeField] private AbilityData m_rightClickAbility;
    [SerializeField] private FloatVariable m_currentManaFloatVariable;
    
    protected Dictionary<AbilityData, AbilityDisplay> m_abilityDisplays = new Dictionary<AbilityData, AbilityDisplay>();
    
    public AbilityData LeftClickAbility => m_leftClickAbility;
    public AbilityData RightClickAbility => m_rightClickAbility;

    protected override void Start()
    {
        base.Start();
        SpawnAbilityDisplays();
        
        m_currentManaFloatVariable.Value = m_currentMana;
    }

    protected override void Update()
    {
        base.Update();
        m_currentManaFloatVariable.Value = m_currentMana;
    }

    private void SpawnAbilityDisplays()
    {
        GamePanel gamePanel = UIManager.Instance.GetPanel<GamePanel>();

		//main abilities
        m_abilityDisplays.Add(m_mainAbility, gamePanel.PlayerUI.SetAbility(2, m_mainAbility));
        
        //click abilities
        m_abilityDisplays.Add(m_leftClickAbility, gamePanel.PlayerUI.SetAbility(1, m_leftClickAbility, GameAssets.Instance.leftClickSprite));
        m_abilityDisplays.Add(m_rightClickAbility, gamePanel.PlayerUI.SetAbility(3, m_rightClickAbility, GameAssets.Instance.rightClickSprite));
        
        //Secondary 
        // foreach (AbilityData secondaryAbility in m_secondaryAbilities)
        // {
        //     _abilityDisplay = Instantiate(m_abilityDisplayPrefab, gamePanel.PlayerUI.AbilityDisplayParent);
        //     _abilityDisplay.Setup(secondaryAbility);
        //     gamePanel.PlayerUI.SetAbility(1, secondaryAbility, GameAssets.Instance.leftClickSprite);
        //     m_abilityDisplays.Add(secondaryAbility, _abilityDisplay);
        // }    
    }

    public override bool UseAbility(AbilityData _abilityData, AbilityContext _context)
    {
        if (base.UseAbility(_abilityData, _context))
        {
            m_abilityDisplays[_abilityData].StartCooldownFill(_abilityData == m_mainAbility ? 1 / StatsEM.AttackSpeed.Value : _abilityData.cooldown);

            m_currentManaFloatVariable.Value = m_currentMana;
            return true;
        }
        
        return false;
    }
}