using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private BigDoubleVariable m_currentHealthDataVariable;
    [SerializeField] private BigDoubleVariable m_maxHealthDataVariable;
    [SerializeField] private FloatVariable m_currentManaDataVariable;
    [SerializeField] private FloatVariable m_maxManaDataVariable;
    [SerializeField] private RoundFillDisplay m_healthDisplay;
    [SerializeField] private RoundFillDisplay m_manaDisplay;
    [SerializeField] private Transform m_abilityDisplayParent;
    [SerializeField] private List<AbilityDisplay> m_abilityDisplays;
    
    [Title("Experience")]
    [SerializeField] private TextMeshProUGUI m_experienceText;
    [SerializeField] private Slider m_experienceSlider;
    
    public Transform AbilityDisplayParent => m_abilityDisplayParent;

    private void Awake()
    {
        m_currentHealthDataVariable.OnValueChanged += UpdateHealth;
        m_currentHealthDataVariable.ForceNotify();
            
        m_maxHealthDataVariable.OnValueChanged += UpdateHealth;
        m_maxHealthDataVariable.ForceNotify();
        
        m_currentManaDataVariable.OnValueChanged += UpdateMana;
        m_currentManaDataVariable.ForceNotify();
        
        m_maxManaDataVariable.OnValueChanged += UpdateMana;
        m_maxManaDataVariable.ForceNotify();
        
        GameData.Instance.onExperienceChanged += UpdateExperience;
        UpdateExperience();
    }

    public AbilityDisplay SetAbility(int _slotIndex, AbilityData _abilityData, Sprite _shortcutIcon = null)
    {
        if (_slotIndex < 0 || _slotIndex >= m_abilityDisplays.Count)
            return null;
        
        m_abilityDisplays[_slotIndex].Setup(_abilityData);

        m_abilityDisplays[_slotIndex].SetShortcutIconActive(_shortcutIcon != null);
        if (_shortcutIcon != null)
            m_abilityDisplays[_slotIndex].SetShortcutIcon(_shortcutIcon);
        
        return m_abilityDisplays[_slotIndex];
    }
    
    //Todo : hp regen, mana regen, auto xp gain
    private void UpdateHealth()
    {
        m_healthDisplay.SetMainText($"{m_currentHealthDataVariable.Value.Format()}/{m_maxHealthDataVariable.Value.Format()}");
        m_healthDisplay.SetFill((float)(m_currentHealthDataVariable.Value.ToDouble() / m_maxHealthDataVariable.Value.ToDouble()), false);
    }

    private void UpdateMana()
    {
     
        m_manaDisplay.SetMainText($"{m_currentManaDataVariable.Value.ToString("0")}/{m_maxManaDataVariable.Value.ToString("0")}");
        m_manaDisplay.SetFill(m_currentManaDataVariable.Value / m_maxManaDataVariable.Value, false);
    }

    private void UpdateExperience()
    {
        m_experienceText.text = $"{GameData.Instance.GetCurrentXPIntoLevel().Format()}/{GameData.Instance.GetXPToNextLevel().Format()}";
        m_experienceSlider.value = GameData.Instance.GetProgressToNextLevel01();
    }
    
    // private void OnDestroy()
    // {
    //     m_currentHealthDataVariable.OnValueChanged -= UpdateHealth;
    //     m_maxHealthDataVariable.OnValueChanged -= UpdateHealth;
    //     
    //     m_currentManaDataVariable.OnValueChanged -= UpdateMana;
    //     m_maxManaDataVariable.OnValueChanged -= UpdateMana;
    //
    //     GameData.Instance.onExperienceChanged -= UpdateExperience;
    // }
}