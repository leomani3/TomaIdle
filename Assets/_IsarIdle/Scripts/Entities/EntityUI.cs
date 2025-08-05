using BreakInfinity;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [SerializeField] private Slider m_slider;
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private bool _hideIfFull;
    [SerializeField] private Transform m_healthBarTfm;
    
    private HealthEM m_healthEM;

    private void Awake()
    {
        m_slider.value = 1;
    }

    private void LateUpdate()
    {
        if (m_healthEM != null)
        {
            m_healthBarTfm.position = CameraManager.Instance.Camera.WorldToScreenPoint(m_healthEM.transform.position.OffsetY(3f));
        }
    }

    public void Bind(HealthEM _healthEM)
    {
        m_healthEM = _healthEM;
        m_healthEM.OnHealthChanged += OnHealthChanged;
        
        m_canvasGroup.alpha = m_healthEM.CurrentHealth == m_healthEM.MaxHealth ? 0 : 1;
    }

    private void OnHealthChanged(BigDouble currentHealth, BigDouble maxHealth)
    {
        m_slider.value = (float)(currentHealth.ToDouble() / maxHealth.ToDouble());
        m_canvasGroup.alpha = m_healthEM.CurrentHealth == m_healthEM.MaxHealth ? 0 : 1;
    }

    private void OnDestroy()
    {
        if (m_healthEM != null)
        {
            m_healthEM.OnHealthChanged -= OnHealthChanged;
            m_healthEM = null;
        }
    }

    private void OnDisable()
    {
        if (m_healthEM != null)
        {
            m_healthEM.OnHealthChanged -= OnHealthChanged;
            m_healthEM = null;
        }
    }
}