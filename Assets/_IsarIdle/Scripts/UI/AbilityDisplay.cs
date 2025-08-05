using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour
{
    [SerializeField] private Image m_abilityIcon;
    [SerializeField] private TextMeshProUGUI m_manaCost;
    [SerializeField] private Image m_cooldownFill;
    [SerializeField] private Image m_blinkImg;
    [SerializeField] private GameObject m_shortcutParent;
    [SerializeField] private Image m_shortcutIcon;

    private Tween m_cooldownFillTween;
    private Tween m_punchScaleTween;
    private Tween m_blinkTween;

    private void Awake()
    {
        m_shortcutParent.SetActive(false);
        m_manaCost.gameObject.SetActive(false);
        m_shortcutParent.gameObject.SetActive(false);
        m_abilityIcon.gameObject.SetActive(false);
        m_cooldownFill.fillAmount = 0;
    }

    [Button]
    public void Setup(AbilityData _abilityData)
    {
        m_shortcutParent.SetActive(_abilityData != null);
        m_manaCost.gameObject.SetActive(_abilityData != null);
        m_abilityIcon.gameObject.SetActive(_abilityData != null);

        if (_abilityData == null)
            return;
        
        m_abilityIcon.sprite = _abilityData.icon;
        m_manaCost.text = _abilityData.manaCost.ToString(); 
        
        m_cooldownFill.fillAmount = 0;
    }

    public void SetShortcutIcon(Sprite _icon)
    {
        m_shortcutParent.SetActive(true);
        m_shortcutIcon.sprite = _icon;
    }

    public void SetShortcutIconActive(bool _active)
    {
        m_shortcutParent.SetActive(_active);
    }

    public void StartCooldownFill(float _cooldown)
    {
        m_cooldownFillTween.Kill();
        m_punchScaleTween.Complete();
        
        m_punchScaleTween = transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
        
        if (_cooldown <= 0)
            return;
        
        m_cooldownFill.fillAmount = 1;
        m_cooldownFillTween = m_cooldownFill.DOFillAmount(0, _cooldown).SetEase(Ease.Linear).OnComplete(OnCooldownComplete);
    }

    private void OnCooldownComplete()
    {
        m_cooldownFill.fillAmount = 0;

        m_blinkImg.color = Color.white;
        m_blinkTween.Kill();
        m_blinkTween = m_blinkImg.DOFade(0, 0.5f);
    }

    private void OnDestroy()
    {
        m_cooldownFillTween.Kill();
        m_punchScaleTween.Kill();
    }
}